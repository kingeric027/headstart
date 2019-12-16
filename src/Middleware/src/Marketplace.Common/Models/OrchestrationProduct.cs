using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
    public class OrchestrationProduct : OrchestrationObject
    {
        [Required]
        public string DefaultSupplierID { get; set; }
        public string DefaultPriceScheduleID { get; set; }
        [Required, MaxLength(100, ErrorMessage = "Must be a minimum of 1 and maximum of 100 characters")]
        public string Name { get; set; }
        [MaxLength(2000, ErrorMessage = "Must be a maximum of 2,000 characters")]
        public string Description { get; set; }
        [Required]
        public int? QuantityMultiplier { get; set; }
        public decimal? ShipWeight { get; set; }
        public decimal? ShipHeight { get; set; }
        public decimal? ShipWidth { get; set; }
        public decimal? ShipLength { get; set; }
        public bool? Active { get; set; } = true;
        public string ShipFromAddressID { get; set; }
        public OrchestrationProductXp xp { get; set; } = new OrchestrationProductXp();
    }

    public class OrchestrationProductXp
    {
        public dynamic Data { get; set; }
        public IEnumerable<ImageRef> Images { get; set; } = new List<ImageRef>();
        public Dictionary<string, List<string>> Facets = new Dictionary<string, List<string>>();
    }

    public class ImageRef
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public List<string> Tags { get; set; }
    }
}
