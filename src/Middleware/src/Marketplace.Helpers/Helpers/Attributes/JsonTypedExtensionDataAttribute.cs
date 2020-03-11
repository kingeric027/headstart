using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonTypedExtensionDataAttribute : Attribute
    {
    }
}
