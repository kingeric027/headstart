using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public class OrchestrationPriceSchedule : IOrchestrationObject
    {

        [Required, MaxLength(100, ErrorMessage = "Must be a minimum of 1 and maximum of 100 characters")]
        public string ID { get; set; }
        [MaxLength(100), Required]
        public string Name { get; set; }
        public bool? ApplyTax { get; set; }
        public bool? ApplyShipping { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public bool? UseCumulativeQuantity { get; set; }
        public bool? RestrictedQuantity { get; set; }
        public List<OrchestrationPriceBreak> PriceBreaks { get; set; }

        public OrchestrationPriceScheduleXp xp { get; set; } = new OrchestrationPriceScheduleXp();
    }

    public class OrchestrationPriceScheduleXp
    {

    }

    public class OrchestrationPriceBreak
    {
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
