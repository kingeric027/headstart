using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Humanizer;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.OpenApiTools;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Helpers.Extensions
{
    public static class SwaggerExtensions
    {
        public static JArray BuildStringListEnum(this ApiModel resource, string pName)
        {
            // stopped here.. make this generic by the attribute type, so the two types can be evaluated to have an enum array or not
            var sortableProperties = resource.Properties
                .Where(p => {
                    return p.PropInfo.CustomAttributes.Any(a => a.AttributeType == typeof(SortableAttribute));
                })
                .Select(p => {
                    var attributes = p.PropInfo.GetAttributes<Attribute>();
                    var sortableAttr = attributes.OfType<SortableAttribute>().FirstOrDefault();
                    var propertyObj = new {
                        p.Name,
                        sortableAttr?.Priority
                    };
                    return propertyObj;
                })
                .OrderBy(p => p.Priority == null).ThenBy(p => p.Priority);

            var searchableProperties = resource.Properties
                .Where(p => { 
                    return p.PropInfo.CustomAttributes.Any(a => a.AttributeType == typeof(SearchableAttribute));
                })
                .Select(p => {
                    var attributes = p.PropInfo.GetAttributes<Attribute>();
                    var searchableAttr = attributes.OfType<SearchableAttribute>().FirstOrDefault();
                    var propertyObj = new {
                        p.Name,
                        searchableAttr?.Priority
                    };
                    return propertyObj;
                })
                .OrderBy(p => p.Priority == null)
                .ThenBy(p => p.Priority);

            return pName == "sortBy" ? 
                new JArray(sortableProperties.Select(p => p.Name)) : 
                new JArray(searchableProperties.Select(p => p.Name));
        }

        public static int SortOrder(this string verb)
        {
            switch (verb.ToLower())
            {
                case "get":
                    return 1;
                case "list":
                    return 2;
                case "post":
                case "create":
                    return 3;
                case "save":
                case "put":
                    return 4;
                case "delete":
                    return 5;
                case "patch":
                    return 6;
                default: return 7;
            }
        }

        public static string ParameterSimpleName(this Type type)
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

        public static string TypeFormatName(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType().TypeFormatName();

            if (type.IsNullable())
                return type.GetGenericArguments()[0].TypeFormatName();

            type = type
                .UnwrapGeneric(typeof(Task<>))
                .UnwrapGeneric(typeof(Partial<>));

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments().Select(t => t.TypeFormatName());
                return $"{type.Name.Split('`')[0]}<{string.Join(", ", args)}>";
            }

            switch (type.Name)
            {
                case "Date":
                    return "date";
                case "Byte":
                    return "byte";
                case "Binary":
                    return "binary";
                case "DateTimeOffset":
                    return "date-time";
                case "Int32":
                    return "int32";
                case "Int64":
                    return "int64";
                case "Decimal":
                    return "float";
                case "Double":
                    return "double";
                default:
                    return null;
            }
        }

        public static string PropertySimpleName(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType().PropertySimpleName();

            if (type.IsNullable())
                return type.GetGenericArguments()[0].PropertySimpleName();

            type = type
                .UnwrapGeneric(typeof(Task<>))
                .UnwrapGeneric(typeof(Partial<>));

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments().Select(s => s.PropertySimpleName());
                return $"{type.Name.Split('`')[0]}<{string.Join(", ", args)}>";
            }

            switch (type.Name)
            {
                case "DateTimeOffset":
                    return "string";
                case "String":
                    return "string";
                case "Int32":
                    return "integer";
                case "Decimal":
                case "Double":
                    return "number";
                case "Boolean":
                    return "boolean";
                case "Object":
                case "JObject":
                case "JRaw":
                case "XP":
                    return "object";
                case "HttpResponseMessage":
                case "IHttpActionResult":
                case "Void":
                    return null;
                default:
                    return type.Name;
            }
        }
        
        public static bool IsBasicType(this string type)
        {
            string[] basicTypes = { "integer", "number", "string", "boolean", "array", "object" };
            return basicTypes.Any(type.Contains);
        }

        public static bool IsValidFormatName(this string type)
        {
            if (type == null) return false;
            string[] validFormatNames =
                {"int32", "int64", "float", "double", "byte", "binary", "date", "date-time", "password"};
            return validFormatNames.Any(type.Contains);
        }

        public static string ControllerFriendlyName(this Type type)
        {
            var name = type.Name.Replace("Controller", "");
            switch (name)
            {
                default:
                    return name.Pluralize();
            }
        }

        public static int RoleRank(this ApiRole role)
        {
            return
                (role == ApiRole.FullAccess) ? 1 :
                (role.ToString().EndsWith("Admin")) ? 2 :
                (role.ToString().EndsWith("Reader")) ? 4 :
                3;
        }

        public static int HttpStatusCode(this Type returnType, HttpMethod verb)
        {
            if (returnType == null || returnType == typeof(void))
                return 204;
            if (verb == System.Net.Http.HttpMethod.Post && returnType.GetProperty("ID") != null)
                return 201;
            return 200;
        }

        public static bool IsListPage(this Type type)
        {
            var t = type?.WithoutGenericArgs();
            return t == typeof(ListPage<>) || t == typeof(ListPageWithFacets<>);
        }

        public static bool IsModelType(this Type type)
        {
            var response = type.IsClass 
                           && (type.Assembly == Assembly.GetAssembly(typeof(OrderCloudModel))
                           || type.HasAttribute<SwaggerModel>(true))
                           && type.WithoutGenericArgs() != typeof(ListArgs<>);
            return response;
        }

        public static Type ResponseType(this MethodInfo m)
        {
            var type = m.ReturnType?.UnwrapGeneric(typeof(Task<>));
            if (type == typeof(void))
                return null;
            return type == typeof(Task) ? null : type;
        }

        public static string SimpleType(this PropertyInfo pi)
        {
            var typeAttr = pi.GetAttribute<DocType>();
            if (typeAttr != null)
                return typeAttr.TypeName;

            return pi.PropertyType.ParameterSimpleName();
        }
    }
}
