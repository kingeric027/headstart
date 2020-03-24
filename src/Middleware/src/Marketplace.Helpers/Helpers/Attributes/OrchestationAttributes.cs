using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Helpers.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class OrchestrationIgnoreAttribute : Attribute { }
}
