using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Common.Models
{
    public class OrchestrationCatalogAssignment : OrchestrationObject
    {
        public string CatalogID { get; set; }
        public string ProductID { get; set; }
    }
}
