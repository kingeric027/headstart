﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Humanizer;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;
using ApiReadOnlyAttribute = Marketplace.Helpers.Attributes.ApiReadOnlyAttribute;
using ApiWriteOnlyAttribute = Marketplace.Helpers.Attributes.ApiWriteOnlyAttribute;
using ErrorCode = Marketplace.Helpers.OpenApiTools.ErrorCode;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Marketplace.Helpers.OpenApiTools
{
    public class ApiReflector
    {
        private static class ListArgsReflector
        {
            public static MethodInfo Method { get; } = typeof(ListArgsReflector).GetMethod(nameof(ListArgsMethod), BindingFlags.Static | BindingFlags.NonPublic);

            private static void ListArgsMethod(
                [DocComments("Word or phrase to search for.")]
                string search = null,
                [DocType.String, DocComments("Comma-delimited list of fields to search on.")]
                string searchOn = null,
                [DocType.String, DocComments("Comma-delimited list of fields to sort by.")]
                string sortBy = null,
                [DocComments("Page of results to return. Default: 1")]
                int page = 1,
                [DocComments("Number of results to return per page. Default: 20, max: 100.")]
                int pageSize = 20,
                [DocComments("Any additional key/value pairs passed in the query string are interpreted as filters. Valid keys are top-level properties of the returned model or 'xp.???'")]
                object filters = null)
            { }
        }

        public static ApiMetaData GetMetaData<TController, TAttribute, TModel>(string refPath, IDictionary<string, IErrorCode> errors)
            where TController : Controller
            where TAttribute : Attribute, IApiAuthAttribute
        {
            return new ApiMetaData
            {
                Sections = GetSections<TController, TAttribute, TModel>(refPath).ToList(),
                Models = (
                    from t in _customTypes
                    where !t.IsEnum
                    let m = GetModelFromType<TModel>(t, true, true)
                    orderby m.IsPartial, m.Name
                    select m).ToList(),
                Enums = (
                    from t in _customTypes
                    where t.IsEnum
                    let e = GetEnumFromType(t)
                    orderby e.Name
                    select e).ToList(),
                ErrorCodes = errors.Values
                    .Select(e => new ErrorCode { FullCode = e.Code, Description = e.DefaultMessage})
                    .ToList(),
                Roles = Enum.GetNames(typeof(ApiRole)).OrderBy(r => r).ToList()
            };
        }

        private static readonly HashSet<Type> _customTypes = new HashSet<Type>();

        private static IEnumerable<ApiSection> GetSections<TController, TAttribute, TModel>(string refPath)
            where TController : Controller
            where TAttribute : Attribute, IApiAuthAttribute
        {
            var isComment = false;
            ApiSection section = null;
            var filePath = refPath; 
            foreach (var line in File.ReadLines(filePath))
            {
                var s = line.Trim();
                if (isComment)
                {
                    if (s.EndsWith("-->"))
                        isComment = false;
                }
                else if (s.StartsWith("<!--"))
                {
                    isComment = true;
                }
                else if (s.StartsWith("#"))
                {
                    var name = s.TrimStart('#').Trim();
                    var id = name.Dehumanize();

                    if (id.Length > 0)
                    {
                        if (section != null)
                            yield return section;

                        var res = GetResources<TController, TAttribute, TModel>(id).ToList();
                        section = new ApiSection
                        {
                            ID = id,
                            Name = name,
                            Description = new List<string>(),
                            Resources = res
                        };
                    }
                }
                else if (s.Length > 0)
                {
                    section?.Description.Add(s);
                }
            }

            if (section != null)
                yield return section;
        }

        private static IEnumerable<ApiResource> GetResources<TController, TAttribute, TModel>(string sectionID) where TAttribute : Attribute, IApiAuthAttribute
        {
               var resource = from c in Assembly.GetAssembly(typeof(TController)).GetExportedTypes()
                where c.IsSubclassOf(typeof(TController))
                where !c.IsAbstract
                where !c.HasAttribute<DocIgnoreAttribute>(false)
                let section = c.GetAttribute<DocSection>()
                where section != null && section.ID == sectionID
                let name = c.ControllerFriendlyName()
                let endpoints = GetEndpoints<TController, TAttribute, TModel>(c, name).ToList()
                where endpoints.Any()
                orderby section.ListOrder, name
                select new ApiResource
                {
                    Name = name,
                    Description = c.GetAttribute<DocNameAttribute>()?.Name ?? name.Humanize(LetterCasing.Title),
                    ControllerType = c,
                    Comments = c.GetAttributes<DocCommentsAttribute>()?.SelectMany(a => a.Comments).ToList(),
                    Endpoints = endpoints
                };
            return resource;
        }

        private static IEnumerable<ApiEndpoint> GetEndpoints<TController, TAttribute, TModel>(Type c, string resource) where TAttribute : Attribute, IApiAuthAttribute
        {
            var temp = from m in c.GetMethods()
                let verb =
                    m.HasAttribute<HttpGetAttribute>() ? System.Net.Http.HttpMethod.Get :
                    m.HasAttribute<HttpPostAttribute>() ? System.Net.Http.HttpMethod.Post :
                    m.HasAttribute<HttpPutAttribute>() ? System.Net.Http.HttpMethod.Put :
                    m.HasAttribute<HttpPatchAttribute>() ? new System.Net.Http.HttpMethod("PATCH") :
                    m.HasAttribute<HttpDeleteAttribute>() ? System.Net.Http.HttpMethod.Delete : null
                where verb != null
                let routex = GetRoute(m)
                let requestTypex = m.GetParameters().Select(p => p.ParameterType)
                    .FirstOrDefault(p => p.IsModelType<TModel>())
                select new { routex, requestTypex};

               var result = from m in c.GetMethods()
                let verb =
                    m.HasAttribute<HttpGetAttribute>() ? System.Net.Http.HttpMethod.Get :
                    m.HasAttribute<HttpPostAttribute>() ? System.Net.Http.HttpMethod.Post :
                    m.HasAttribute<HttpPutAttribute>() ? System.Net.Http.HttpMethod.Put :
                    m.HasAttribute<HttpPatchAttribute>() ? new System.Net.Http.HttpMethod("PATCH") :
                    m.HasAttribute<HttpDeleteAttribute>() ? System.Net.Http.HttpMethod.Delete : null
                where verb != null
                let route = GetRoute(m)
                let requestType = m.GetParameters().Select(p => p.ParameterType).FirstOrDefault(p => p.IsModelType<TModel>())
                let responseType = m.ResponseType()
                orderby m.Name.SortOrder()
                select new ApiEndpoint
                {
                    Name = m.Name,
                    SubResource = m.GetAttribute<DocSubResourceAttribute>()?.Name,
                    Description = GetEndpointDescription(m, resource),
                    Comments = GetEndpointComments(m),
                    MethodInfo = m,
                    Route = route,
                    HttpVerb = verb,
                    PathArgs = GetArgs<TController, TModel>(requestType ?? responseType, m, route, true).ToList(),
                    QueryArgs = GetArgs<TController, TModel>(requestType ?? responseType, m, route, false).ToList(),
                    RequestModel = GetModelFromType<TModel>(requestType, false, true),
                    ResponseModel = GetModelFromType<TModel>(responseType, true, false),
                    HttpStatus = responseType.HttpStatusCode(verb),
                    RequiredRoles = GetRequiredRoles<TAttribute>(m),
                    IsList = responseType.UnwrapGeneric(typeof(Task<>)).IsListPage(),
                    HasListArgs = m.GetParameters().Any(p => p.ParameterType.WithoutGenericArgs() == typeof(ListArgs<>))
                };
            return result;
        }

        private static IList<string> GetRequiredRoles<T>(MethodInfo m) where T : Attribute, IApiAuthAttribute
        {
            var roles = m.GetAttributes<T>().SelectMany(r => r.ApiRoles).ToList();
            if (roles.Any() && !roles.Contains(ApiRole.FullAccess))
                roles.Insert(0, ApiRole.FullAccess);

            return roles.Select(r => r.ToString()).ToList();
        }

        private static IEnumerable<ApiParameter> GetArgs<TController, TModel>(Type modelType, MethodInfo methodInfo, string path, bool inPath)
        {
            foreach (var param in methodInfo.GetParameters())
            {
                if ((param.ParameterType.IsModelType<TModel>()))
                    continue;

                if (path.Contains($"{param.Name}") != inPath)
                    continue;

                CacheCustomTypes<TModel>(param.ParameterType);

                if (param.ParameterType.WithoutGenericArgs() == typeof(ListArgs<>))
                {
                    foreach (var arg in GetArgs<TController, TModel>(modelType, ListArgsReflector.Method, path, inPath))
                    {
                        arg.IsListArg = true;
                        yield return arg;
                    }
                }
                else
                {
                    // for list endpoints that don't have ListArgs but do have some matching params (page, pageSize), use param descriptions from ListArgs.
                    ParameterInfo listArg = null;
                    if (methodInfo.ReturnType.IsListPage())
                    {
                        listArg = ListArgsReflector.Method.GetParameters().FirstOrDefault(p => p.Name == param.Name);
                    }

                    yield return new ApiParameter
                    {
                        ParamInfo = param,
                        Name = param.Name,
                        Description = GetDescription(modelType, listArg ?? param, true),
                        Required = !param.IsOptional,
                        SimpleType = param.ParameterType.ParameterSimpleName()
                    };
                }
            }
        }

        private static string GetEndpointDescription(MemberInfo m, string resource)
        {
            // check if a custom [DocName] was provided
            if (m.HasAttribute<DocNameAttribute>())
                return m.GetAttribute<DocNameAttribute>().Name;

            var s = m.Name.TrimStart("Get", "List", "Create", "Delete", "Patch", "Save");

            resource = $"{resource.Singularize(false)} {s}".Trim().Humanize(LetterCasing.LowerCase);

            if (m.Name.StartsWith("List"))
                resource = resource.Pluralize();

            if (resource.StartsWith("me "))
            {
                resource = resource.TrimStart("me ");
                if (m.Name.StartsWith("List"))
                    resource += " visible to this user";
            }

            // "a" or "an", depending on if resource starts with a vowel
            var a = "aeio".Contains(resource[0]) ? "an" : "a";

            if (m.Name.StartsWith("Get"))
                return $"Get a single {resource}.";
            if (m.Name.StartsWith("List"))
                return $"Get a list of {resource}.";
            if (m.Name.StartsWith("Create"))
                return $"Create a new {resource}.";
            if (m.Name.StartsWith("Save"))
                return $"Create or update {a} {resource}.";
            if (m.Name.StartsWith("Delete"))
                return $"Delete {a} {resource}.";
            if (m.Name.StartsWith("Patch"))
                return $"Partially update {a} {resource}.";

            var words = m.Name.Humanize(LetterCasing.Sentence).Split(' ').ToList();
            if (words.Count == 1)
                words.Add(resource);
            a = "aeio".Contains(words[1][0]) ? "an" : "a";
            words.Insert(1, a); // first word should be a verb, put "a" or "an" after it

            return string.Join(" ", words) + ".";
        }

        private static IList<string> GetEndpointComments(MemberInfo m)
        {
            var comments = m.GetAttributes<DocCommentsAttribute>()?.SelectMany(a => a.Comments).ToList() ?? new List<string>();
            switch (m.Name)
            {
                case "Create":
                    comments.Insert(0, "If ID is provided and an object with that ID already exists, a 409 (conflict) error is returned.");
                    break;
                case "Save":
                    comments.Insert(0, "If an object with the same ID already exists, it will be overwritten.");
                    break;
            }

            return comments;
        }

        private static string GetRoute(MemberInfo m)
        {
            var attr = m.GetCustomAttribute<RouteAttribute>();
            if (attr?.Template != null && attr.Template.StartsWith("~"))
                return attr.Template.TrimStart('~').TrimStart('/'); // ignore RoutePrefix if route starts with "~"

            var prefixAttr = m.DeclaringType?.GetCustomAttribute<RouteAttribute>();

            return ((prefixAttr == null ? "" : prefixAttr.Template + "/") + (attr == null ? "" : attr.Template)).Trim('/');
        }

        private static ApiEnum GetEnumFromType(Type type)
        {
            var e = new ApiEnum
            {
                Name = type.Name,
                Description = type.GetAttribute<DocCommentsAttribute>()?.Comments.JoinString(" "),
                Values = (
                    from v in Enum.GetValues(type).Cast<Enum>()
                    let m = v.GetType().GetMember(v.ToString())[0]
                    where !m.HasAttribute<DocIgnoreAttribute>()
                    select new ApiEnumValue
                    {
                        Name = v.ToString(),
                        Description = m.GetAttribute<DocCommentsAttribute>()?.Comments.JoinString(" "),
                    }).ToList()
            };
            return e;
        }

        private static ApiModel GetModelFromType<TModel>(Type type, bool includeReadOnly, bool includeWriteOnly)
        {
            if (type == null || type == typeof(void))
                return null;

            CacheCustomTypes<TModel>(type);

            var name = type.Name;
            var isPartial = false;

            if (type.WithoutGenericArgs() == typeof(Partial<>))
            {
                type = type.GetGenericArguments().First();
                name = "Partial" + type.Name;
                isPartial = true;
            }

            var innerModel = GetModelFromType<TModel>(type.GetGenericArguments().FirstOrDefault(), includeReadOnly, includeWriteOnly);
            
            var props = (
                from p in type.GetProperties()
                where p.PropertyType != typeof(object)
                where !p.HasAttribute<DocIgnoreAttribute>()
                where !p.HasAttribute<JsonIgnoreAttribute>()
                let prop = new ApiProperty
                {
                    Name = p.Name,
                    PropInfo = p,
                    SimpleType = p.SimpleType(),
                    Required = p.HasAttribute<RequiredAttribute>(),
                    Description = GetDescription(p, includeWriteOnly),
                    ReadOnly = p.HasAttribute<ApiReadOnlyAttribute>(),
                    WriteOnly = p.HasAttribute<ApiWriteOnlyAttribute>(),
                    SampleData = GetSampleData<TModel>(p, includeReadOnly, includeWriteOnly)
                }
                where includeReadOnly || !prop.ReadOnly
                where includeWriteOnly || !prop.WriteOnly
                select prop).DistinctBy(v => v.Name).ToList();
            
            return new ApiModel
            {
                Name = name,
                Type = type,
                InnerModel = innerModel,
                Properties = props,
                IsReadOnly = type.HasAttribute<ApiReadOnlyAttribute>(),
                IsWriteOnly = type.HasAttribute<ApiWriteOnlyAttribute>(),
                IsPartial = isPartial,
                Sample = props.ToDictionary(p => p.Name, p => p.SampleData)
            };
        }

        private static object GetSampleData<TController>(PropertyInfo prop, bool includeReadOnly, bool includeWriteOnly)
        {
            var attr = prop.GetAttribute<DocSampleDataAttribute>();
            return attr?.Value ?? GetSampleData<TController>(prop.PropertyType, includeReadOnly, includeWriteOnly);
        }

        private static object GetSampleData<TModel>(Type type, bool includeReadOnly, bool includeWriteOnly)
        {
            type = type.WithoutNullable();

            if (type == typeof(string))
                return "";

            if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
                return new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.FromHours(-6));

            if (type.IsNumeric())
                return 0;

            if (type == typeof(ListPageMeta))
                return new ListPageMeta { Page = 1, PageSize = 20, TotalCount = 25 };

            if (type == typeof(ListPageMetaWithFacets))
                return new ListPageMetaWithFacets
                {
                    Page = 1,
                    PageSize = 20,
                    TotalCount = 25,
                    Facets = new[] {
                        new ListFacet { XpPath = "size", Name = "Size", Values = new[] {
                            new ListFacetValue { Value = "Large", Count = 15 },
                            new ListFacetValue { Value = "Medium", Count = 10 },
                            new ListFacetValue { Value = "Small", Count = 7 }
                        } },
                        new ListFacet { XpPath = "color", Name = "Color", Values = new[] {
                            new ListFacetValue { Value = "Red", Count = 15 },
                            new ListFacetValue { Value = "Blue", Count = 10 }
                        } },
                    }
                };

            if (type == typeof(JRaw)) // xp
                return new object();

            if (type.IsEnum)
                return Enum.GetNames(type).FirstOrDefault();

            if (type.IsValueType)
                return Activator.CreateInstance(type);


            if (type.IsCollection())
                return new[] { GetSampleData<TModel>(type.GetCollectionItemType(), includeReadOnly, includeWriteOnly) };

            if (type.IsModelType<TModel>())
                return GetModelFromType<TModel>(type, includeReadOnly, includeWriteOnly)?.Sample;

            return new object();
        }

        private static string GetDescription(Type modelType, ParameterInfo param, bool isWrite)
        {
            modelType = modelType ?? param.Member.ReflectedType;
            return GetDescription(modelType, param.ParameterType, param.Name, param.GetAttributes<Attribute>(), isWrite);
        }

        private static string GetDescription(PropertyInfo prop, bool isWrite)
        {
            return GetDescription(prop.ReflectedType, prop.PropertyType, prop.Name, prop.GetAttributes<Attribute>(), isWrite);
        }

        private static string GetDescription(Type modelType, Type memberType, string memberName, IEnumerable<Attribute> attributes, bool isWrite)
        {
            // check if a DocComments attribute is applied to the member or member type
            var attr = attributes.OfType<DocCommentsAttribute>().FirstOrDefault() ?? memberType.GetAttribute<DocCommentsAttribute>();
            var result = attr?.Comments.FirstOrDefault();
            if (result != null)
                return result;

            memberType = memberType.WithoutNullable();
            modelType = modelType.GetCollectionItemType() ?? modelType;

            if (modelType.IsListPage())
                modelType = modelType.GetGenericArguments()[0];

            //TODO: evaluate the trimming of names here
            var resourceName = modelType.Name
                .Humanize(LetterCasing.LowerCase)
                .Singularize(false)
                .TrimEnd(" controller")
                .TrimStart("buyer ", "admin ", "supplier ", "me ");

            if (memberName == "xp")
                return $"Container for extended (custom) properties of the {resourceName}.";

            if (memberName == "ID") { }
            else if (memberName.EndsWith("ID"))
            {
                resourceName = memberName.Substring(0, memberName.Length - 2).Humanize(LetterCasing.LowerCase);
                memberName = "ID";
            }
            else
            {
                memberName = memberName.Humanize(LetterCasing.Sentence);
            }

            var s = $"{memberName} of the {resourceName}.";

            if (isWrite)
            {
                if (attributes.OfType<RequiredAttribute>().Any())
                    s += " Required.";

                var mla = attributes.OfType<MaxLengthAttribute>().FirstOrDefault();
                if (mla != null)
                    s += $" Max length {mla.Length} characters.";

                var ra = attributes.OfType<RangeAttribute>().FirstOrDefault();
                if (ra is MinValueAttribute mva)
                    s += $" Must be at least {ra.Minimum}.";
                else if (ra != null)
                    s += $" Must be between {ra.Minimum} and {ra.Maximum}.";
            }

            if (memberType.IsEnum)
                s += $" Possible values: {string.Join(", ", Enum.GetNames(memberType))}.";

            return s;
        }

        private static void CacheCustomTypes<TModel>(Type type, bool nestedInPartial = false)
        {
            if (type == null)
                return;

            if (type.IsCollection())
            {
                CacheCustomTypes<TModel>(type.GetCollectionItemType(), nestedInPartial);
                return;
            }

            if (type.IsGenericType)
            {
                var isPartial = (type.WithoutGenericArgs() == typeof(Partial<>));

                if (isPartial)
                    _customTypes.Add(type);

                foreach (var innerType in type.GetGenericArguments())
                    CacheCustomTypes<TModel>(innerType, false);

                return;
            }

            if (!type.IsModelType<TModel>()) //
                return;

            if (!type.IsGenericType)
            {
                _customTypes.Add(type);

                // if a model has a nested model, and we know the parent needs a partial, make sure nested model has partial too.
                // Example: make sure there's a PartialInventory we can use on a PartialProduct.
                if (nestedInPartial && !type.IsEnum)
                    _customTypes.Add(typeof(Partial<>).MakeGenericType(type));
            }

            foreach (var prop in type.GetProperties())
                CacheCustomTypes<TModel>(prop.PropertyType, nestedInPartial);
        }
    }
}