using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public class OrchestrationProductFacet : OrchestrationObject
    {
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
