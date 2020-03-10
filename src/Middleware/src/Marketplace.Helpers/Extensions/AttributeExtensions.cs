using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Helpers.Helpers.Attributes;

namespace Marketplace.Helpers.Extensions
{
    public static class AttributeExtension
    {
        public static string ToSymbol(this Enum en)
        {
            var memInfo = en.GetType().GetMember(en.ToString());

            if (memInfo.Length <= 0)
                return en.ToString();

            var attrs = memInfo[0].GetCustomAttributes(typeof(OperatorSymbol), false);

            return attrs.Length > 0 ? ((OperatorSymbol) attrs[0]).Symbol : en.ToString();
        }
    }
}
