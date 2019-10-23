using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public class OrchestrationSpecProductAssignment : OrchestrationObject
    {

        public string SpecID { get; set; }
        public string ProductID { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultOptionID { get; set; }
    }
}
