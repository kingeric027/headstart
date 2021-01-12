using System.Diagnostics;
using OrderCloud.SDK;

namespace ordercloud.integrations.library
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

        public static double ShipWeightOrDefault(this LineItem li, double defaultValue)
        {
            if (li.Variant?.ShipWeight != null)
                return li.Variant.ShipWeight.To<double>();
            return li.Product.ShipWeight.IsNullOrZero() ? defaultValue : li.Product.ShipWeight.To<double>();
        }

        public static double ShipLengthOrDefault(this LineItem li, double defaultValue)
        {
            if (li.Variant?.ShipLength != null)
                return li.Variant.ShipLength.To<double>();
            return li.Product.ShipLength.IsNullOrZero() ? defaultValue : li.Product.ShipLength.To<double>();
        }
        public static double ShipHeightOrDefault(this LineItem li, double defaultValue)
        {
            if (li.Variant?.ShipHeight != null)
                return li.Variant.ShipHeight.To<double>();
            return li.Product.ShipHeight.IsNullOrZero() ? defaultValue : li.Product.ShipHeight.To<double>();
        }
        public static double ShipWidthOrDefault(this LineItem li, double defaultValue)
        {
            if (li.Variant?.ShipWidth != null)
                return li.Variant.ShipWidth.To<double>();
            return li.Product.ShipWidth.IsNullOrZero() ? defaultValue : li.Product.ShipWidth.To<double>();
        }
    }
}
