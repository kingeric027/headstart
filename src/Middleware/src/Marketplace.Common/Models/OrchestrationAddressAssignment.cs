using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
    public class OrchestrationAddressAssignment : OrchestrationObject
    {
        [Required]
        public string AddressID { get; set; }
        [Required]
        public string UserGroupID { get; set; }
        public bool IsShipping { get; set; }
        public bool IsBilling { get; set; }
    }
}
