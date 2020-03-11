using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Helpers.Extensions
{
    public static class CollectionExtensions
    {
        public static string JoinString<T>(this IEnumerable<T> items, string separator)
        {
            return string.Join(separator, items);
        }

        public static string JoinString<T>(this IEnumerable<T> items, string separator, Func<T, object> transform)
        {
            return string.Join(separator, items.Select(transform));
        }

        public static IServiceCollection AddServicesByConvention(this IServiceCollection services, Assembly asm,
            string @namespace = null)
        {
            var mappings =
                from impl in asm.GetTypes()
                let iface = impl.GetInterface($"I{impl.Name}")
                where iface != null
                where @namespace == null || iface.Namespace == @namespace
                select new { iface, impl };

            foreach (var m in mappings)
                services.AddTransient(m.iface, m.impl);

            return services;
        }
    }
}
