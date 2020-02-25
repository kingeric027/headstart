using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Humanizer;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;
using ErrorCodes = Marketplace.Models.ErrorCodes;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Marketplace.Common.Helpers.Tools
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

        public static ApiMetaData GetMetaData<TController, TAttribute>(string refPath)
            where TController : Controller
            where TAttribute : Attribute, IApiAuthAttribute
        {
            return new ApiMetaData
            {
                Sections = GetSections<TController, TAttribute>(refPath).ToList(),
                Models = (
                    from t in _customTypes
                    where !t.IsEnum
                    let m = GetModelFromType(t, true, true)
                    orderby m.IsPartial, m.Name
                    select m).ToList(),
                Enums = (
                    from t in _customTypes
                    where t.IsEnum
                    let e = GetEnumFromType(t)
                    orderby e.Name
                    select e).ToList(),
                ErrorCodes = ErrorCodes.All.Values
                    .Select(e => new ErrorCode { FullCode = e.Code, Description = e.DefaultMessage })
                    .ToList(),
                Roles = Enum.GetNames(typeof(ApiRole)).OrderBy(r => r).ToList()
            };
        }

        private static readonly HashSet<Type> _customTypes = new HashSet<Type>();

        private static IEnumerable<ApiSection> GetSections<TController, TAttribute>(string refPath)
            where TController : Controller
            where TAttribute : Attribute, IApiAuthAttribute
        {
            var isComment = false;
            ApiSection section = null;
            var filePath = refPath; //Path.Combine($"..\\..\\..\\..\\..\\..\\resources", "Reference.md");
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

                        var res = GetResources<TController, TAttribute>(id).ToList();
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

        private static IEnumerable<ApiResource> GetResources<TController, TAttribute>(string sectionID) where TAttribute : Attribute, IApiAuthAttribute
        {
            var t = typeof(TController);
            var resource = from c in Assembly.GetExecutingAssembly().ExportedTypes
                where c.IsSubclassOf(typeof(TController))
                where !c.IsAbstract
                where !c.HasAttribute<DocIgnoreAttribute>(false)
                let section = c.GetAttribute<DocSection>()
                where section != null && section.ID == sectionID
                let name = GetResourceName(c)
                let endpoints = GetEndpoints<TAttribute>(c, name).ToList()
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

        private static string GetResourceName(Type type)
        {
            var name = type.Name.Replace("Controller", "");
            switch (name)
            {
                default:
                    return name.Pluralize();
            }
        }

        private static IEnumerable<ApiEndpoint> GetEndpoints<T>(Type c, string resource) where T : Attribute, IApiAuthAttribute
        {
            var result = from m in c.GetMethods()
                         let verb =
                             m.HasAttribute<HttpGetAttribute>() ? System.Net.Http.HttpMethod.Get :
                             m.HasAttribute<HttpPostAttribute>() ? System.Net.Http.HttpMethod.Post :
                             m.HasAttribute<HttpPutAttribute>() ? System.Net.Http.HttpMethod.Put :
                             m.HasAttribute<HttpPatchAttribute>() ? new System.Net.Http.HttpMethod("PATCH") :
                             m.HasAttribute<HttpDeleteAttribute>() ? System.Net.Http.HttpMethod.Delete :
                             null
                         where verb != null
                         let route = GetRoute(m)
                         let requestType = m.GetParameters().Select(p => p.ParameterType).FirstOrDefault(IsModelType)
                         let responseType = GetResponseType(m)
                         orderby GetEndpointSortOrder(m.Name)
                         select new ApiEndpoint
                         {
                             Name = m.Name,
                             SubResource = m.GetAttribute<DocSubResourceAttribute>()?.Name,
                             Description = GetEndpointDescription(m, resource),
                             Comments = GetEndpointComments(m),
                             MethodInfo = m,
                             Route = route,
                             HttpVerb = verb,
                             PathArgs = GetArgs(requestType ?? responseType, m, route, true).ToList(),
                             QueryArgs = GetArgs(requestType ?? responseType, m, route, false).ToList(),
                             RequestModel = GetModelFromType(requestType, false, true),
                             ResponseModel = GetModelFromType(responseType, true, false),
                             HttpStatus = GetHttpStatus(responseType, verb),
                             RequiredRoles = GetRequiredRoles<T>(m),
                             IsList = IsListPage(responseType?.UnwrapGeneric(typeof(Task<>))),
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

        private static int RankRole(ApiRole role)
        {
            return
                (role == ApiRole.FullAccess) ? 1 :
                (role.ToString().EndsWith("Admin")) ? 2 :
                (role.ToString().EndsWith("Reader")) ? 4 :
                3;
        }

        private static int GetHttpStatus(Type returnType, System.Net.Http.HttpMethod verb)
        {
            if (returnType == null || returnType == typeof(void))
                return 204;
            if (verb == System.Net.Http.HttpMethod.Post && returnType.GetProperty("ID") != null)
                return 201;
            return 200;
        }

        private static IEnumerable<ApiParameter> GetArgs(Type modelType, MethodInfo methodInfo, string path, bool inPath)
        {
            foreach (var param in methodInfo.GetParameters())
            {
                if (IsModelType(param.ParameterType))
                    continue;

                if (path.Contains($"{param.Name}") != inPath)
                    continue;

                CacheCustomTypes(param.ParameterType);

                if (param.ParameterType.WithoutGenericArgs() == typeof(ListArgs<>))
                {
                    foreach (var arg in GetArgs(modelType, ListArgsReflector.Method, path, inPath))
                    {
                        arg.IsListArg = true;
                        yield return arg;
                    }
                }
                else
                {
                    // for list endpoints that don't have ListArgs but do have some matching params (page, pageSize), use param descriptions from ListArgs.
                    ParameterInfo listArg = null;
                    if (IsListPage(methodInfo.ReturnType))
                    {
                        listArg = ListArgsReflector.Method.GetParameters().FirstOrDefault(p => p.Name == param.Name);
                    }

                    yield return new ApiParameter
                    {
                        ParamInfo = param,
                        Name = param.Name,
                        Description = GetDescription(modelType, listArg ?? param, true),
                        Required = !param.IsOptional,
                        SimpleType = GetSimpleType(param.ParameterType)
                    };
                }
            }
        }

        private static bool IsListPage(Type type)
        {
            var t = type?.WithoutGenericArgs();
            return t == typeof(ListPage<>) || t == typeof(ListPageWithFacets<>);
        }

        private static string GetEndpointDescription(MethodInfo m, string resource)
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

        private static IList<string> GetEndpointComments(MethodInfo m)
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

        private static Type GetResponseType(MethodInfo m)
        {
            var type = m.ReturnType?.UnwrapGeneric(typeof(Task<>));
            if (type == typeof(void))
                return null;
            if (type == typeof(Task))
                return null;
            return type;
        }

        private static string GetRoute(MethodInfo m)
        {
            var attr = m.GetCustomAttribute<RouteAttribute>();
            //var attr = m.GetAttribute<RouteAttribute>();
            if (attr?.Template != null && attr.Template.StartsWith("~"))
                return attr.Template.TrimStart('~').TrimStart('/'); // ignore RoutePrefix if route starts with "~"

            var prefixAttr = m.DeclaringType.GetCustomAttribute<RouteAttribute>();

            return ((prefixAttr == null ? "" : prefixAttr.Template + "/") + (attr == null ? "" : attr.Template)).Trim('/');
        }

        private static ApiEnum GetEnumFromType(Type type)
        {
            return new ApiEnum
            {
                Name = type.Name,
                Description = type.GetAttribute<DocCommentsAttribute>()?.Comments.JoinString(" "),
                Values = (
                    from v in Enum.GetValues(type).Cast<Enum>()
                        // https://stackoverflow.com/questions/1799370/getting-attributes-of-enums-value
                    let m = v.GetType().GetMember(v.ToString())[0]
                    where !m.HasAttribute<DocIgnoreAttribute>()
                    select new ApiEnumValue
                    {
                        Name = v.ToString(),
                        Description = m.GetAttribute<DocCommentsAttribute>()?.Comments.JoinString(" "),
                    }).ToList()
            };
        }

        private static ApiModel GetModelFromType(Type type, bool includeReadOnly, bool includeWriteOnly)
        {
            if (type == null || type == typeof(void))
                return null;

            CacheCustomTypes(type);

            var name = type.Name;
            var isPartial = false;

            if (type.WithoutGenericArgs() == typeof(Partial<>))
            {
                type = type.GetGenericArguments().First();
                name = "Partial" + type.Name;
                isPartial = true;
            }

            var innerModel = GetModelFromType(type.GetGenericArguments().FirstOrDefault(), includeReadOnly, includeWriteOnly);
            var props = (
                from p in type.GetProperties()
                where p.PropertyType != typeof(System.Object)
                where !p.HasAttribute<DocIgnoreAttribute>()
                where !p.HasAttribute<JsonIgnoreAttribute>()
                let prop = new ApiProperty
                {
                    Name = p.Name,
                    PropInfo = p,
                    SimpleType = GetSimpleType(p),
                    Required = p.HasAttribute<RequiredAttribute>(),
                    Description = GetDescription(p, includeWriteOnly),
                    ReadOnly = p.HasAttribute<ApiReadOnlyAttribute>(),
                    WriteOnly = p.HasAttribute<ApiWriteOnlyAttribute>(),
                    SampleData = GetSampleData(p, includeReadOnly, includeWriteOnly)
                }
                where includeReadOnly || !prop.ReadOnly
                where includeWriteOnly || !prop.WriteOnly
                select prop).ToList();

            return new ApiModel
            {
                Name = name,
                Type = type,
                InnerModel = innerModel,
                //InheritsFrom = (
                //    from t2 in types
                //    where t2 == t.BaseType || t.GetInterfaces().Contains(t2)
                //    orderby t2.IsInterface ? 2 : 1 // list concreate base type first
                //    select t2).ToList(),
                Properties = props,
                IsReadOnly = type.HasAttribute<ApiReadOnlyAttribute>(),
                IsWriteOnly = type.HasAttribute<ApiWriteOnlyAttribute>(),
                IsPartial = isPartial,
                Sample = props.ToDictionary(p => p.Name, p => p.SampleData)
            };
        }

        private static object GetSampleData(PropertyInfo prop, bool includeReadOnly, bool includeWriteOnly)
        {
            var attr = prop.GetAttribute<DocSampleDataAttribute>();
            return attr?.Value ?? GetSampleData(prop.PropertyType, includeReadOnly, includeWriteOnly);
        }

        private static object GetSampleData(Type type, bool includeReadOnly, bool includeWriteOnly)
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
                return new[] { GetSampleData(type.GetCollectionItemType(), includeReadOnly, includeWriteOnly) };

            if (IsModelType(type))
                return GetModelFromType(type, includeReadOnly, includeWriteOnly)?.Sample;

            return new object();
        }

        private static bool IsModelType(Type type) =>
            type.IsClass &&
            type.Assembly == Assembly.GetExecutingAssembly() && type.WithoutGenericArgs() != typeof(ListArgs<>);

        private static string GetSimpleType(Type type)
        {
            type = type.WithoutNullable();

            if (type.IsCollection())
                return "array";

            if (type.IsEnum)
                return "string";

            // DateTimeOffset has typecode object for some reason
            if (type == typeof(DateTimeOffset))
                return "date";

            switch (Type.GetTypeCode(type.WithoutNullable()))
            {
                case TypeCode.String:
                    return "string";
                case TypeCode.Boolean:
                    return "boolean";
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return "integer";
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return "float";
                case TypeCode.DateTime:
                    return "date";
                default:
                    return "object";
            }
        }

        private static string GetSimpleType(PropertyInfo pi)
        {
            var typeAttr = pi.GetAttribute<DocType>();
            if (typeAttr != null)
                return typeAttr.TypeName;

            return GetSimpleType(pi.PropertyType);
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

            if (IsListPage(modelType))
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

        private static int GetEndpointSortOrder(string name)
        {
            switch (name)
            {
                case "Get": return 1;
                case "List": return 2;
                case "Create": return 3;
                case "Save": return 4;
                case "Delete": return 4;
                default: return 5;
            }
        }

        private static void CacheCustomTypes(Type type, bool nestedInPartial = false)
        {
            if (type == null)
                return;

            if (type.IsCollection())
            {
                CacheCustomTypes(type.GetCollectionItemType(), nestedInPartial);
                return;
            }

            if (type.IsGenericType)
            {
                var isPartial = (type.WithoutGenericArgs() == typeof(Partial<>));

                if (isPartial)
                    _customTypes.Add(type);

                foreach (var innerType in type.GetGenericArguments())
                    CacheCustomTypes(innerType, false);

                return;
            }

            //TODO: need to filter out the simple types from the System.Object namespace
            // but can't just check the executing assembly because Models might exist in other assemblies
            if (type.Assembly.FullName.Contains("System"))
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
                CacheCustomTypes(prop.PropertyType, nestedInPartial);
        }
    }
}
