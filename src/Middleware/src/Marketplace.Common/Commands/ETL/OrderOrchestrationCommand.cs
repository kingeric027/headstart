using System;
using System.Collections.Generic;
using System.Text;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.ETL
{
    public interface IOrderOrchestrationCommand
    {

    }
    public class OrderOrchestrationCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;

        public OrderOrchestrationCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                
            });
        }
    }
}
