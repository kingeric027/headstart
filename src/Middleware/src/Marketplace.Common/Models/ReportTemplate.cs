using Cosmonaut.Attributes;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    [SwaggerModel]
    [CosmosCollection("reporttemplates")]
    public class ReportTemplate : CosmosObject
    {
        public string TemplateID { get; set; }
        [CosmosPartitionKey]
        public string ClientID { get; set; }
        public string ReportType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Headers { get; set; }
        public Filters Filters { get; set; }
        public bool AvailableToSuppliers { get; set; }
    }

    public class Filters
    {
        public List<string> BuyerID { get; set; }
        public List<string> State { get; set; }
        public List<string> Country { get; set; }
    }
}
