using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public class OrchestrationProductFacet : IOrchestrationObject
    {
        [Required, MaxLength(100, ErrorMessage = "Must be a minimum of 1 and maximum of 100 characters")]
        public string ID { get; set; }
        public string Name { get; set; }
        public string XpPath { get; set; }
        public int? ListOrder { get; set; }
        public int? MinCount { get; set; }
        public OrchestrationProductFacetXp xp { get; set; } = new OrchestrationProductFacetXp();
    }

    public class OrchestrationProductFacetXp
    {
    }
}
