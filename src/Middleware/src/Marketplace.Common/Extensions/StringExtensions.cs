using System;
using System.Collections.Generic;
using System.Text;
using CreditCardValidator;

namespace Marketplace.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToCreditCardDisplay(this string value)
        {
            var result = $"{value.Substring(value.Length - 4, 4)}";
            return result;
        }
    }
}
