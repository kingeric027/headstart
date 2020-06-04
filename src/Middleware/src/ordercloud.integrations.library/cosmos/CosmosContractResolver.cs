using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ordercloud.integrations.library
{
	public class CosmosContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            // Let the base class create all the JsonProperties using the short names
            // Replace the ID Property with the Underlying property name -
            var properties = base.CreateProperties(type, memberSerialization);
            foreach (var property in properties)
            {
                if (property.PropertyName == "ID")
                {
                    property.PropertyName = property.UnderlyingName; // typically "InteropID"
                }
            }
            return properties
                .Where(prop => prop.AttributeProvider.GetAttributes(true).All(atr => atr.GetType() != typeof(CosmosIgnoreAttribute)))
                .ToList();
        }
    }
}
