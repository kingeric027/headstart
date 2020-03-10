using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Helpers.OpenApiTools;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Marketplace.Helpers.OpenApiTools
{
    //This class generates a spec that complies with OpenAPI spec (formerly known as swagger spec)
    // as described here https://swagger.io/specification/ Mainly for sdk-code-gen purposes
    //Useful resource: http://editor.swagger.io/

    public class OpenApiGeneratorX<TController, TAttribute, TModel> where TController : Controller
        where TAttribute : Attribute, IApiAuthAttribute
    {
        private ApiMetaData _data;
        private JObject _spec;
        public OpenApiGeneratorX()
        {
        }

        public JObject Specification()
        {
            return this._spec;
        }

        public OpenApiGeneratorX<TController, TAttribute, TModel> CollectMetaData(string refPath, IDictionary<string, IErrorCode> errors)
        {
            this._data = ApiReflector.GetMetaData<TController, TAttribute, TModel>(refPath, errors);
            return this;
        }

        public OpenApiGeneratorX<TController, TAttribute, TModel> DefineSpec(SwaggerConfig config)
        {
            this._spec = new JObject()
                .AddMetaData(config)
                .AddResourceTags(_data)
                .AddComponents(_data)
                .AddPathObjects(_data);
            return this;
        }
    }
}
