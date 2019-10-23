using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public class OrchestrationSpec : IOrchestrationObject
    {

        [Required, MaxLength(100, ErrorMessage = "Must be a minimum of 1 and maximum of 100 characters")]
        public string ID { get; set; }
        public int? ListOrder { get; set; }
        [Required]
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public bool? Required { get; set; }
        public bool? AllowOpenText { get; set; }
        public string DefaultOptionID { get; set; }
        public bool DefinesVariant { get; set; }
        public OrchestrationSpecXp xp { get; set; } = new OrchestrationSpecXp();
    }

    public class OrchestrationSpecXp
    {
        [Required]
        public SpecUI UI { get; set; } = new SpecUI();
    }

    public class SpecUI
    {
        public string ControlType { get; set; } = "text";
    }
}
