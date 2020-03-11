using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Tests
{
    public static class TestHelpers
    {
        /// <summary>
        /// use to access values on dynamics
        /// </summary>
        /// <param name="source">the thing to drill into</param>
        /// <param name="accessor">typically the name of the property, use dot notation to access nested values</param>
        /// <returns></returns>
        public static dynamic GetDynamicVal(dynamic source, string accessor)
        {
            var props = accessor.Split('.');
            foreach (var prop in props)
            {
                source = source.GetType().GetProperty(prop).GetValue(source, null);
            }
            return source;
        }
    }
}
