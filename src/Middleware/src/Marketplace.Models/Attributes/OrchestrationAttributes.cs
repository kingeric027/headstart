using System;

namespace Marketplace.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class OrchestrationIgnoreAttribute : Attribute { }
}
