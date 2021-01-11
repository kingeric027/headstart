using System.Collections.Generic;

namespace Headstart.Common.Extensions
{
    public static class Extensions
    {
        public static bool HasItem<t>(this IList<t> itemList)
        {
            if (itemList == null || itemList.Count == 0)
            { return false; }

            return true;
        }
        public static bool HasItem<t>(this IReadOnlyList<t> itemList)
        {
            if (itemList == null || itemList.Count == 0)
            { return false; }

            return true;
        }
        public static bool HasItem<t>(this List<t> itemList)
        {
            if (itemList == null || itemList.Count == 0)
            { return false; }

            return true;
        }
    }
}
