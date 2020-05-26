﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Marketplace.Models.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Common.Helpers
{
    public static class OrchestrationSerializer
    {
        public static JsonSerializer Serializer =>
            new JsonSerializer()
            {
                ContractResolver = new OrchestrationJsonSerializer()
            };

        private class OrchestrationJsonSerializer : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                if (member.GetCustomAttribute(typeof(OrchestrationIgnoreAttribute)) == null) return property;
                property.ShouldSerialize = o => false;
                property.ShouldDeserialize = o => false;
                return property;
            }
        }
    }

    public class MarketplaceSerializer : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            // don't serialize properties with [ApiReadOnly]
            //if (member.GetCustomAttribute(typeof(ApiReadOnlyAttribute)) != null)
            //    prop.ShouldDeserialize = o => false;
            if (member.GetCustomAttribute(typeof(ApiWriteOnlyAttribute)) != null || member.GetCustomAttribute(typeof(ApiIgnoreAttribute)) != null)
                prop.ShouldSerialize = o => false;
            return prop;
        }
    }
}
