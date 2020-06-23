using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.library
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CosmosIgnoreAttribute : Attribute { }
}
