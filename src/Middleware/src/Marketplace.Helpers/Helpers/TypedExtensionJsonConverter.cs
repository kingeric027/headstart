using System;
using System.Linq;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Marketplace.Helpers
{
    public class TypedExtensionJsonConverter<TObject> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(TObject).IsAssignableFrom(objectType);
        }

        JsonProperty GetExtensionJsonProperty(JsonObjectContract contract)
        {
            try
            {
                return contract.Properties.Single(p => p.AttributeProvider.GetAttributes(typeof(JsonTypedExtensionDataAttribute), false).Any());
            }
            catch (InvalidOperationException ex)
            {
                throw new JsonSerializationException(
                    $"Exactly one property with JsonTypedExtensionDataAttribute is required for type {contract.UnderlyingType}", ex);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var jObj = JObject.Load(reader);
            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(objectType);
            var extensionJsonProperty = GetExtensionJsonProperty(contract);

            var extensionJProperty = (JProperty)null;
            for (var i = jObj.Count - 1; i >= 0; i--)
            {
                var property = (JProperty)jObj.AsList()[i];
                if (contract.Properties.GetClosestMatchProperty(property.Name) != null) continue;
                if (extensionJProperty == null)
                {
                    extensionJProperty = new JProperty(extensionJsonProperty.PropertyName, new JObject());
                    jObj.Add(extensionJProperty);
                }
                ((JObject)extensionJProperty.Value).Add(property.RemoveFromLowestPossibleParent());
            }

            var value = existingValue ?? contract.DefaultCreator();
            using (var subReader = jObj.CreateReader())
                serializer.Populate(subReader, value);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());
            var extensionJsonProperty = GetExtensionJsonProperty(contract);

            JObject jObj;
            using (new PushValue<bool>(true, () => Disabled, (canWrite) => Disabled = canWrite))
            {
                jObj = JObject.FromObject(value, serializer);
            }

            var extensionValue = (jObj[extensionJsonProperty.PropertyName] as JObject).RemoveFromLowestPossibleParent();
            if (extensionValue != null)
            {
                for (var i = extensionValue.Count - 1; i >= 0; i--)
                {
                    var property = (JProperty)extensionValue.AsList()[i];
                    jObj.Add(property.RemoveFromLowestPossibleParent());
                }
            }

            jObj.WriteTo(writer);
        }

        [ThreadStatic]
        static bool disabled;

        // Disables the converter in a thread-safe manner.
        bool Disabled
        {
            get => disabled;
            set => disabled = value;
        }

        public override bool CanWrite => !Disabled;

        public override bool CanRead => !Disabled;
    }

    public struct PushValue<T> : IDisposable
    {
        readonly Action<T> setValue;
        readonly T oldValue;

        public PushValue(T value, Func<T> getValue, Action<T> setValue)
        {
            if (getValue == null || setValue == null)
                throw new ArgumentNullException();
            this.setValue = setValue;
            this.oldValue = getValue();
            setValue(value);
        }

        #region IDisposable Members

        // By using a disposable struct we avoid the overhead of allocating and freeing an instance of a finalizable class.
        public void Dispose()
        {
            setValue?.Invoke(oldValue);
        }

        #endregion
    }
}
