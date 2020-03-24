using System;
using System.Reflection;
using Marketplace.Helpers.Helpers.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Marketplace.Helpers
{
    public static class OrchestrationSerializer
    {
        public static JsonSerializer Serializer =>
            new JsonSerializer() {
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
}
