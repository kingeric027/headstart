using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static string JoinString<T>(this IEnumerable<T> items, string separator)
        {
            return string.Join(separator, items);
        }
    }
}
