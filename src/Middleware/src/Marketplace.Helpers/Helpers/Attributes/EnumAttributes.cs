using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Marketplace.Helpers.Helpers.Attributes
{
    public class OperatorSymbol : Attribute
    {

        public OperatorSymbol(string symbol)
        {
            this.Symbol = symbol;
        }


        public string Symbol { get; set; }
    }
}
