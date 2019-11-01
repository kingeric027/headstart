using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public class OrchestrationProductAssignment : OrchestrationObject
    {

        public string ProductID { get; set; }
        public string BuyerID { get; set; }
        public string UserGroupID { get; set; }
        public string PriceScheduleID { get; set; }
    }
}
