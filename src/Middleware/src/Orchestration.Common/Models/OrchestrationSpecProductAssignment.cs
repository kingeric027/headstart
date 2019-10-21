using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Orchestration.Common.Models
{
    public class OrchestrationSpecProductAssignment : IOrchestrationObject
    {

        [Required, MaxLength(100, ErrorMessage = "Must be a minimum of 1 and maximum of 100 characters")]
        public string ID { get; set; }
        public string SpecID { get; set; }
        public string ProductID { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultOptionID { get; set; }
    }
}
