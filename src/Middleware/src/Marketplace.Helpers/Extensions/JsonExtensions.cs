using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marketplace.Helpers.Extensions
{
    public static class JsonExtensions
    {
        public static TJToken RemoveFromLowestPossibleParent<TJToken>(this TJToken node) where TJToken : JToken
        {
            if (node == null)
                return null;
            var contained = node.AncestorsAndSelf().FirstOrDefault(t => t.Parent is JContainer && t.Parent.Type != JTokenType.Property);
            contained?.Remove();
            // Also detach the node from its immediate containing property -- Remove() does not do this even though it seems like it should
            if (node.Parent is JProperty property)
                property.Value = null;
            return node;
        }

        public static IList<JToken> AsList(this IList<JToken> container) { return container; }

        public static JRaw ToJRaw(this object obj)
        {
            return new JRaw(JsonConvert.SerializeObject(obj));
        }

        public static JRaw Compress(this JRaw jraw)
        {
            if (jraw == null) return null;
            try { return new JRaw(JToken.Parse(jraw.ToString()).ToString(Formatting.None)); }
            catch { return jraw; }
        }
    }
}
