using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OrderCloud.SDK;

namespace Marketplace.Common.Models.Marketplace
{
    public class MarketplaceOrderSubmitResponse : OrderSubmitResponse<OrderSubmitResponseXp>
    {

    }

    public class OrderSubmitResponseXp
    {
        public List<ProcessResult> ProcessResults { get; set; }
    }

    public class ProcessResult
    {
        public ProcessType Type { get; set; }
        public List<ProcessResultAction> Activity { get; set; } = new List<ProcessResultAction>();
    }

    public class ProcessResultAction
    {
        public ProcessType ProcessType { get; set; }
        public string Description { get; set; }
        public bool Success { get; set; }
        public ProcessResultException Exception { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProcessType
    {
        Forwarding,
        Notification,
        Accounting,
        Tax
    }

}
