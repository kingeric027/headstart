using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
    public class OrchestrationCostCenter : OrchestrationObject
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public OrchestrationCostCenterXp xp { get; set; } = new OrchestrationCostCenterXp();
    }

    public class OrchestrationCostCenterXp
    {

    }
}
