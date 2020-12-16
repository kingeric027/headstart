using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.library.extensions
{
    public static class DecimalExtensions
    {
        public static bool IsNullOrZero(this decimal? value)
        {
            return value == null || value == 0; 
        }

        public static double ValueOrDefault(this decimal? value, double defaultValue)
        {
            return value.IsNullOrZero() ? defaultValue : value.To<double>();
        }
    }
}
