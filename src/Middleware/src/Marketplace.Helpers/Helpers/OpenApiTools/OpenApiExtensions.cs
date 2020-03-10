using System.Linq;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Helpers.OpenApiTools;
using Marketplace.Helpers.Models;
using Newtonsoft.Json.Linq;

namespace Marketplace.Helpers.OpenApiTools
{
    public static class OpenApiExtensions
    {
        public static JObject AddMetaData(this JObject spec, SwaggerConfig config)
        {
            //Set version number, name, description etc.. 
            spec.Add("openapi", "3.0.0");
            spec.Add("info", new JObject
            {
                { "title", config.Title },
                { "description", config.Description },
                { "version", config.Version },
                { "contact", new JObject(
                    new JProperty("name", config.Name),
                    new JProperty("url", config.Url),
                    new JProperty("email", config.ContactEmail)) }
            });
            spec.Add("servers", new JArray
            {
                new JObject(
                    new JProperty("url", config.Host),
                    new JProperty("description", config.Description))
            });
            return spec;
        }

        public static JObject AddComponents(this JObject spec, ApiMetaData data)
        {
            spec.Add("components", new JObject(
                new JProperty("securitySchemes", new SecurityObject(data).ToJObject()),
                new JProperty("schemas", new SchemaObject(data).ToJObject())));
            return spec;
        }

        public static JObject AddResourceTags(this JObject spec, ApiMetaData data)
        {
            var tagsArray = new JArray();

            foreach (var section in data.Sections)
            {
                tagsArray.Add(new JObject(
                    new JProperty("name", section.Name),
                    new JProperty("description", string.Join("\n", section.Description)),
                    new JProperty("x-id", section.ID)));

                foreach (var resource in section.Resources)
                {
                    tagsArray.Add(new JObject(
                        new JProperty("name", resource.Description),
                        new JProperty("description", string.Join("\n", resource.Comments)),
                        new JProperty("x-section-id", section.ID)));
                }
            }
            spec.Add("tags", tagsArray);
            return spec;
        }

        public static JObject AddPathObjects(this JObject spec, ApiMetaData data)
        {
            var paths =
                from s in data.Sections
                from r in s.Resources
                from e in r.Endpoints
                orderby e.Route
                select new
                {
                    Path = $"/{e.Route.TrimStart("v1")}",
                    Verb = e.HttpVerb.Method.ToLower(),
                    EndpointObj = new EndpointObject(r, e, data).ToJObject()
                };

            var jp = new JObject();
            foreach (var p in paths.GroupBy(p => p.Path))
            {
                var je = new JObject();
                foreach (var e in p.OrderBy(x => x.Verb.SortOrder()))
                    je.Add(e.Verb, e.EndpointObj);
                jp.Add(p.Key, je);
            }
            spec.Add("paths", jp);
            return spec;
        }
    }
}
