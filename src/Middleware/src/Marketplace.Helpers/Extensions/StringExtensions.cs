using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Marketplace.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static string ToSafeId(this string obj, params string[] replacements)
        {
            var edited = replacements.Aggregate(obj, (current, r) => current.Replace(r, ""));
            return Regex.Replace(edited, @"[^a-zA-Z0-9-_]+", "_").ToLower();
        }
        public static string Truncate(this string key, int length)
        {
            return key.Substring(0, key.Length > length ? length : key.Length);
        }

        private static object TryGetXp(this string property, dynamic xp)
        {
            var x = (IDictionary<string, object>)xp;
            return x.ContainsKey(property) ? x[property] : null;
        }

        public static string TrimStart(this string s, params string[] remove)
        {
            foreach (var r in remove)
                while (s.StartsWith(r))
                    s = s.Substring(r.Length);
            return s;
        }

        public static string TrimEnd(this string s, params string[] remove)
        {
            foreach (var r in remove)
                while (s.EndsWith(r))
                    s = s.Substring(0, s.Length - r.Length);
            return s;
        }
    }
}
