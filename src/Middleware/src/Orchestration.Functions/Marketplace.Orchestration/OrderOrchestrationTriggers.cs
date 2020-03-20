using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Common.Commands;
using Marketplace.Common.Queries;
using Marketplace.Common;
using OrderCloud.SDK;
using Marketplace.Common.Services.ShippingIntegration.Models;
using System.Threading.Tasks;

namespace Marketplace.Orchestration
{
    public class OrderOrchestrationTrigger
    {
        private readonly IOrderOrchestrationCommand _orderOrchestrationCommand;
        private readonly LogQuery _log;
        private readonly AppSettings _appSettings;
        private DurableOrchestrationContext _orchestrationContext;

        public OrderOrchestrationTrigger(AppSettings appSettings, IOrderOrchestrationCommand orderOrchestrationCommand, ISyncCommand sync, LogQuery log)
        {
            _orderOrchestrationCommand = orderOrchestrationCommand;
            _log = log;
            _appSettings = appSettings;
        }

        [FunctionName("OrderShipmentTimeTrigger")]
        public async Task SyncOrders([TimerTrigger("0 */1 14-23 * * *")]TimerInfo myTimer, [OrchestrationClient]DurableOrchestrationClient client, ILogger logger)
        {
            // run every 10 minutes between 9am and 6pm CDT
            // determine if different schedule or order range is needed for production
            try
            {
                var suppliersToSync = await _orderOrchestrationCommand.GetSuppliersNeedingSync();
                foreach (var supplier in suppliersToSync)
                {
                    await client.StartNewAsync("ShipmentSyncOrchestration", input: supplier);
                }
            } catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
