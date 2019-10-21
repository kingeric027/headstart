using System;
using System.Collections.Generic;
using System.Text;

namespace Orchestration.Common.Models
{
    public class OrchestrationSupplier : ICosmosObject
    {
        public string id { get; set; }
        public DateTimeOffset timeStamp { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Name { get; set; }
        public string CatalogID { get; set; }
        public string UserID { get; set; }
    }
}
