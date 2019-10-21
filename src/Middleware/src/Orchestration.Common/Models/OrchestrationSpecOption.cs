using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OrderCloud.SDK;

namespace Orchestration.Common.Models
{
    public class OrchestrationSpecOption : IOrchestrationObject
    {

        [System.ComponentModel.DataAnnotations.Required, MaxLength(100, ErrorMessage = "Must be a minimum of 1 and maximum of 100 characters")]
        public string ID { get; set; }
        public string Value { get; set; }
        public int? ListOrder { get; set; }
        public bool? IsOpenText { get; set; }
        public PriceMarkupType? PriceMarkupType { get; set; }
        public decimal? PriceMarkup { get; set; }
        public OrchestrationSpecOptionXp xp { get; set; } = new OrchestrationSpecOptionXp();
    }

    public class OrchestrationSpecOptionXp
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string SpecID { get; set; }
    }
}
