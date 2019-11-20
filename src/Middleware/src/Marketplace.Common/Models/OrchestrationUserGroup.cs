using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
    public class OrchestrationUserGroup : OrchestrationObject
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public OrchestrationUserGroupXp xp { get; set; } = new OrchestrationUserGroupXp();
    }

    public class OrchestrationUserGroupXp
    {

    }
}
