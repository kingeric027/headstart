using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Marketplace.Common.Commands;
using Marketplace.Common.Queries;
using Marketplace.Common;
using System.Threading.Tasks;
using System.Linq;

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
        public async Task SyncOrders([TimerTrigger("0 */30 14-23 * * *")]TimerInfo myTimer, [OrchestrationClient]DurableOrchestrationClient client, ILogger logger)
        {
            // run every 30 minutes between 9am and 6pm CDT
            // determine if different schedule or order range is needed for production
            logger.LogInformation("Starting function");
            try
            {
                logger.LogInformation("Going to get suppliers");
                var suppliersToSync = await _orderOrchestrationCommand.GetSuppliersNeedingSync();
                logger.LogInformation($"Retrieved suppliers {suppliersToSync.Count()}");
                foreach (var supplier in suppliersToSync)
                {
                    logger.LogInformation($"Starting supplier {supplier.ID}");
                    await client.StartNewAsync("ShipmentSyncOrchestration", input: supplier);
                    logger.LogInformation($"Finished supplier {supplier.ID}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
