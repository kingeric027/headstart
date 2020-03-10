using System;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Helpers.Mappers
{
    public static class MapperHelper
    {
        public static object TryGetXp(dynamic xp, string property)
        {
            var x = (IDictionary<string, object>)xp;
            return x.ContainsKey(property) ? x[property] : null;
        }

        public static Dictionary<string, List<string>> TryFacetXp(dynamic xp)
        {
            var list = new Dictionary<string, List<string>>();
            var x = (IDictionary<string, object>)xp;
            if (!x.ContainsKey("Facets")) return list;

            if (x["Facets"] is IDictionary<string, object> o)
            {
                foreach (var p in o)
                {
                    if (p.Value != null)
                        list.Add(p.Key, (p.Value as List<object>).Select(a => (string)a).ToList());
                }
            }

            return list;
        }
    }
}
