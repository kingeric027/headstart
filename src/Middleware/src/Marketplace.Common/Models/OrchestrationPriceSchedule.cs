﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public class OrchestrationPriceSchedule : OrchestrationObject
    {

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
