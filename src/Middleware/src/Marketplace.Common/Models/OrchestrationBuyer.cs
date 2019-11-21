using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
    public class OrchestrationBuyer : OrchestrationObject
    {
        [Required]
        public string Name { get; set; }
        public string DefaultCatalogID { get; set; }
        public bool Active { get; set; }
        public OrchestrationBuyerXp xp { get; set; } = new OrchestrationBuyerXp();
    }

    public class OrchestrationBuyerXp
    {
    }
}
