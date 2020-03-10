using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Helpers.Exceptions.Models
{
    public class InvalidPropertyError
    {
        public InvalidPropertyError(Type type, string name)
        {
            Property = $"{type.Name}.{name}";
        }
        public string Property { get; set; }
    }

    public class BlobConfigurationError
    {
        public BlobConfigurationError(string name)
        {
            Property = $"{name}";
        }

        public string Property { get; set; }
    }
}
