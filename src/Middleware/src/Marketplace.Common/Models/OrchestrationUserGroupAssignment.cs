using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
    public class OrchestrationUserGroupAssignment : OrchestrationObject
    {
        [Required]
        public string UserGroupID { get; set; }
        [Required]
        public string UserID { get; set; }
    }
}
