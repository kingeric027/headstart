using System;
using System.Collections.Generic;
using System.Text;
using Flurl.Util;
using Marketplace.Helpers.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marketplace.Helpers.Extensions
{
    public static class ModelExtensions
    {
        public static T PatchWith<T>(this T model, Partial<T> delta)
        {
            return InnerPatch(model, delta.Values);
        }

        public class ReplaceArrayConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType.IsCollection();

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // ignore existingValue and just create a new collection
                return JsonSerializer.CreateDefault().Deserialize(reader, objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                JsonSerializer.CreateDefault().Serialize(writer, value);
            }
        }

        public class MergeXpConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var delta = serializer.Deserialize<JToken>(reader);
                if (delta == null || delta.Type == JTokenType.Null)
                    return null;

                var xp = (existingValue == null) ? null : JToken.Parse(existingValue.ToString()) as JObject;
                if (xp == null || xp.Type == JTokenType.Null)
                    return new JRaw(delta);

                xp.Merge(delta, new JsonMergeSettings
                {
                    MergeNullValueHandling = MergeNullValueHandling.Merge,
                    MergeArrayHandling = MergeArrayHandling.Replace
                });
                return new JRaw(xp);
            }

            public override bool CanConvert(Type objectType) => objectType == typeof(JRaw);
        }

        private static T InnerPatch<T>(T model, JObject delta)
        {
            var ser = JsonSerializer.CreateDefault();
            ser.DefaultValueHandling = DefaultValueHandling.Include;
            ser.Converters.Add(new ReplaceArrayConverter());
            ser.Converters.Add(new MergeXpConverter());

            using (var reader = delta.CreateReader())
            {
                ser.Populate(reader, model);
            }
            return model;
        }
    }
}
