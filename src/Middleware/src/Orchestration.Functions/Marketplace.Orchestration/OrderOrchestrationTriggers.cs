using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Marketplace.Common.Commands;
using Marketplace.Common.Queries;
using Marketplace.Common;
using System.Threading.Tasks;
using System.Linq;
using Marketplace.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using ordercloud.integrations.extensions;
using OrderCloud.SDK;

namespace Marketplace.Orchestration
{
    public class OrderOrchestrationTrigger
    {
        private readonly IOrderOrchestrationCommand _orderOrchestrationCommand;
        private readonly LogQuery _log;
        private readonly AppSettings _appSettings;
        private readonly IOrderCloudIntegrationsFunctionToken _token;

        public OrderOrchestrationTrigger(AppSettings appSettings, IOrderCloudIntegrationsFunctionToken token, IOrderOrchestrationCommand orderOrchestrationCommand, ISyncCommand sync, LogQuery log)
        {
            _orderOrchestrationCommand = orderOrchestrationCommand;
            _log = log;
            _appSettings = appSettings;
            _token = token;
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

        [FunctionName("GetSupplierOrder")]
        public async Task<object> GetSupplierOrder([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "{supplierId}/{orderId}")]
            HttpRequest req, string supplierId, string orderId, ILogger log)
        {
            log.LogInformation($"Supplier Order GET Request: {supplierId} {orderId}");
            try
            {
                var user = await _token.Authorize(req, new [] { ApiRole.OrderAdmin, ApiRole.OrderReader });
                return await Task.FromResult(new {SupplierId = supplierId, OrderId = orderId});
            }
            catch (OrderCloudIntegrationException oex)
            {
                return await Task.FromResult(oex.ApiError);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ApiError()
                {
                    ErrorCode = "500",
                    Message = ex.Message
                });
            }
            
        }
    }
}
