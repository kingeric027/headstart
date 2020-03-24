using System;
using System.Reflection;
using Marketplace.Helpers.Helpers.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Marketplace.Helpers
{
    public class DynamicConverter : ExpandoObjectConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return
                objectType == typeof(object) ||
                objectType == typeof(JObject) ||
                base.CanConvert(objectType);
        }
    }
    public class OrchestrationSerializer : DefaultContractResolver
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
