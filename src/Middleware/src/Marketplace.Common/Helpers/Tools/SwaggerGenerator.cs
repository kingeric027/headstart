using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Common.Helpers.Tools
{
    public class SwaggerGenerator
    {
        private readonly List<string> definitionArray = new List<string>();

        public static JObject GenerateSwaggerSpec<TController, TAttribute>(string refPath, string version)
            where TController : Controller
            where TAttribute : Attribute, IApiAuthAttribute
        {
            var data = ApiReflector.GetMetaData<TController, TAttribute>(refPath);
            return new SwaggerGenerator().MakeSwaggerObject(version, data);
        }

        private JObject MakeSwaggerObject(string version, ApiMetaData data)
        {
            //This is the main JObject that holds all our spec data
            var swagger = new JObject();
            //Add meta data, like version, host etc.
            swagger = MakeSwaggerMetaData(swagger);
            //Add the oauth settings
            swagger = MakeSecurityData(swagger, data);
            //Construct all the data types and definitions
            swagger = MakeDefinitionObjects(swagger, data);
            //Construct all the path and endpoints
            swagger.Add("paths", MakePathObjects(version, data));

            return swagger;
        }

        private JObject MakePathObjects(string version, ApiMetaData data)
        {
            var paths =
                from r in data.Resources
                from e in r.Endpoints
                orderby e.Route
                select new
                {
                    Path = e.Route.TrimStart(version),
                    Verb = e.HttpVerb.Method.ToLower(),
                    EndpointObj = BuildEndpointObject(r, e)
                };

            var jp = new JObject();
            foreach (var p in paths.GroupBy(p => p.Path))
            {
                var je = new JObject();
                foreach (var e in p.OrderBy(x => GetVerbSortOrder(x.Verb)))
                    je.Add(e.Verb, e.EndpointObj);
                jp.Add(p.Key, je);
            }
            return jp;
        }

        private JObject BuildEndpointObject(ApiResource res, ApiEndpoint method)
        {
            var endpointObj = new JObject
            {
                {"responses", MakeResponses(method)},
                {"operationId", method.Name},
                {"tags", new JArray(res.Name.Singularize(false))}
            };
            //Add responses obj
            //Add operationId and tags
            //Add parameters obj
            endpointObj = AddParams(endpointObj, method);

            //Add scopes needed to access this endpoint
            if (method.RequiredRoles != null)
                endpointObj.Add("security", new JArray(new JObject(new JProperty("oauth2", method.RequiredRoles))));
            return endpointObj;
        }

        private static int GetVerbSortOrder(string verb)
        {
            switch (verb.ToLower())
            {
                case "get": return 1;
                case "post": return 2;
                case "put": return 3;
                case "delete": return 4;
                case "patch": return 5;
                default: return 6;
            }
        }

        private JObject AddParams(JObject endpointObj, ApiEndpoint endpoint)
        {
            var paramArray = new JArray();

            foreach (var p in endpoint.PathArgs)
            {
                var paramObj = new JObject
                {
                    {"name", p.Name},
                    {"in", "path"},
                    {"description", p.Description},
                    {"required", p.Required}
                };
                var type = p.SimpleType;
                if (type == "date") type = "string";
                paramObj.Add("type", type);
                paramArray.Add(paramObj);
            }

            foreach (var p in endpoint.QueryArgs)
            {
                var paramObj = new JObject
                {
                    {"name", p.Name},
                    {"in", "query"},
                    {"description", p.Description},
                    {"required", p.Required}
                };
                var type = p.SimpleType;
                if (type == "date") type = "string";
                paramObj.Add("type", p.Name == "filters" ? "object" : type);
                if (p.Name == "sortBy" || p.Name == "searchOn")
                {
                    paramObj.Add("items", new JObject
                    {
                        {"type", "string"}
                    });
                }
                if (p.Name != "defaultSearchOn" && p.Name != "defaultSortBy")
                    paramArray.Add(paramObj);
            }

            if (endpoint.RequestModel != null)
            {
                var paramObj = new JObject
                {
                    {"name", endpoint.RequestModel.Name.Camelize()},
                    {"in", "body"},
                    {"required", true},
                    {"description", ""}
                };
                //Body params are always required
                var type = GetSimpleTypeName(endpoint.RequestModel.Type);
                paramObj.Add("schema", new JObject(new JProperty("$ref", "#/definitions/" + type)));
                paramArray.Add(paramObj);
            }

            endpointObj.Add("parameters", paramArray);
            return endpointObj;
        }

        private JObject MakeResponses(ApiEndpoint endpoint)
        {
            var responses = new JObject();
            var responseObj = new JObject { { "description", string.Join(".", endpoint.Comments) } };

            var returnType = GetSimpleTypeName(endpoint.MethodInfo.ReturnType
                .UnwrapGeneric(typeof(Task<>))
                .UnwrapGeneric(typeof(ListPage<>))
                .UnwrapGeneric(typeof(ListPageWithFacets<>)));

            if (endpoint.IsList)
                returnType = "List" + returnType;

            if (returnType != null && returnType != "Task")
            {
                var schemaObj = new JObject {{"$ref", $"#/definitions/{returnType}"}};
                responseObj.Add("schema", schemaObj);
            }

            responses.Add(endpoint.HttpStatus.ToString(), responseObj);
            return responses;
        }

        private JObject MakeDefinitionObjects(JObject swaggerObj, ApiMetaData data)
        {
            var definitions = new Dictionary<string, JToken>();

            foreach (var model in data.Models)
            {
                var type = model.Name;
                var defObject = new JObject {{"type", "object"}};
                var properties = new JObject();
                //Make properties
                foreach (var prop in model.Properties)
                    properties.Add(prop.Name, MakePropObject(prop.Type, null, out _));
                defObject.Add("properties", properties);
                definitions.Add(type, defObject);
                definitionArray.Add(type);
            }

            AddDefObjectFromType("Meta", typeof(ListPageMeta), definitions);
            AddDefObjectFromType("MetaWithFacets", typeof(ListPageMetaWithFacets), definitions);

            //Find all methods that return ListPage and create all the different types of List objects 
            foreach (var endpoint in data.Resources.SelectMany(r => r.Endpoints).Where(e => e.IsList))
            {
                var itemType = GetSimpleTypeName(endpoint.ResponseModel.Type
                    .UnwrapGeneric(typeof(ListPage<>))
                    .UnwrapGeneric(typeof(ListPageWithFacets<>)));

                var listTypeName = "List" + itemType;
                //Check if this list type doesn't exist yet 
                if (definitions.ContainsKey(listTypeName)) continue;

                var listObj = new JObject {{"type", "object"}};

                var listProperties = new JObject
                {
                    {
                        "Items", new JObject(
                            new JProperty("type", "array"),
                            new JProperty("items", new JObject(new JProperty("$ref", "#/definitions/" + itemType)))
                        )
                    }
                };
                var metaDef = (endpoint.ResponseModel.Type.WithoutGenericArgs() == typeof(ListPageWithFacets<>)) ? "MetaWithFacets" : "Meta";
                listProperties.Add("Meta", new JObject(new JProperty("$ref", "#/definitions/" + metaDef)));
                listObj.Add("properties", listProperties);
                definitions.Add(listTypeName, listObj);
            }

            var sorted = new JObject();
            foreach (var def in definitions.OrderBy(d => d.Key))
                sorted.Add(def.Key, def.Value);

            swaggerObj.Add("definitions", sorted);
            return swaggerObj;
        }

        private void AddDefObjectFromType(string name, Type type, Dictionary<string, JToken> defs)
        {
            var obj = new JObject { { "type", "object" } };
            var props = new JObject();
            foreach (var prop in type.GetProperties())
            {
                props.Add(prop.Name, MakePropObject(prop.PropertyType, null, out var isDefRef));
                if (isDefRef)
                {
                    var nestedType = prop.PropertyType.GetCollectionItemType() ?? prop.PropertyType;
                    AddDefObjectFromType(nestedType.Name, nestedType, defs);
                }
            }
            obj.Add("properties", props);
            defs.Add(name, obj);
        }

        private JObject MakePropObject(Type propType, JObject propObject, out bool isDefRef)
        {
            if (propObject == null)
                propObject = new JObject();
            var SimpleName = GetSimpleTypeName(propType);

            if (propType.IsCollection())
            {
                propObject.Add("type", "array");
                return MakePropObject(propType.GetCollectionItemType(), propObject, out isDefRef);
            }

            var objectToAdd = "type";
            if (propObject["type"] != null)
                objectToAdd = "items";

            if (propType.IsEnum || (propType.IsGenericType && propType.GenericTypeArguments[0].IsEnum))
            {
                isDefRef = false;
                if (objectToAdd == "items") propObject.Add("items", new JObject(new JProperty("type", "string")));
                else propObject.Add("type", "string");
            }
            else if (IsBasicType(SimpleName))
            {
                isDefRef = false;
                if (objectToAdd == "items") propObject.Add("items", new JObject(new JProperty("type", SimpleName)));
                else propObject.Add("type", SimpleName);
            }
            else
            {
                isDefRef = true;
                if (objectToAdd == "items") propObject.Add("items", new JObject(new JProperty("$ref", "#/definitions/" + SimpleName)));
                else propObject.Add("$ref", "#/definitions/" + SimpleName);
            };

            return propObject;
        }

        private bool IsBasicType(string type)
        {
            string[] basicTypes = { "integer", "number", "string", "boolean", "array", "object" };
            return basicTypes.Any(type.Contains);
        }

        private string GetSimpleTypeName(Type type)
        {
            if (type.IsArray)
                return GetSimpleTypeName(type.GetElementType());

            if (type.IsNullable())
                return GetSimpleTypeName(type.GetGenericArguments()[0]);

            type = type
                .UnwrapGeneric(typeof(Task<>))
                .UnwrapGeneric(typeof(Partial<>));

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments().Select(GetSimpleTypeName);
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

        private JObject MakeSwaggerMetaData(JObject swaggerObj)
        {
            //Set version number, name, description etc.. 
            swaggerObj.Add("swagger", "2.0");

            //TODO: make values dynamic
            //TODO: is 3.0 update needed?
            var infoObj = new JObject
            {
                {"title", "Marketplace API"},
                {"description", ""},
                {"version", "1.0"},
                {
                    "contact", new JObject(new JProperty("name", "Marketplace"),
                        new JProperty("url", "https://ordercloud.io"),
                        new JProperty("email", "oheywood@four51.com"))
                }
            };
            swaggerObj.Add("info", infoObj);

            swaggerObj.Add("host", "marketplace-api-qa.azurewebsites.net");
            swaggerObj.Add("basePath", "");
            swaggerObj.Add("schemes", new JArray("https"));

            swaggerObj.Add("consumes", new JArray("application/json", "text/plain; charset=utf-8"));
            swaggerObj.Add("produces", new JArray("application/json"));

            return swaggerObj;
        }

        private JObject MakeSecurityData(JObject swaggerObj, ApiMetaData data)
        {
            //Get list of security profiles/roles
            string[] excludedProfiles = { "DevCenter", "IsDevProfile", "ID", "Name" };
            var scopes = new JObject();
            foreach (var role in data.Roles)
            {
                if (!excludedProfiles.Any(role.Contains)) scopes.Add(role, "");
            }

            var securityObj = new JObject
            {
                {
                    "oauth2", new JObject(
                        new JProperty("type", "oauth2"),
                        new JProperty("tokenUrl", "https://auth.ordercloud.io/oauth/token"),
                        new JProperty("flow", "password"),
                        new JProperty("scopes", scopes)
                    )
                }
            };

            swaggerObj.Add("security", new JArray(new JObject(new JProperty("oauth2", new JArray()))));
            swaggerObj.Add("securityDefinitions", securityObj);
            return swaggerObj;
        }
    }
}
