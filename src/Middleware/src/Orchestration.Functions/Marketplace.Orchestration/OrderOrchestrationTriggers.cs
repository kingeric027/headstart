using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Marketplace.Common.Commands;
using Marketplace.Common.Queries;
using Marketplace.Common;
using System.Threading.Tasks;
using System.Linq;
using Marketplace.Common.Commands.SupplierSync;
using Marketplace.Common.Helpers;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace Marketplace.Orchestration
{
    public class OrderOrchestrationTrigger
    {
        private readonly IOrderOrchestrationCommand _orderOrchestrationCommand;
        private readonly LogQuery _log;
        private readonly AppSettings _appSettings;
        private readonly IProductUpdateCommand _productUpdateCommand;
       

        public OrderOrchestrationTrigger(AppSettings appSettings, IOrderCloudIntegrationsFunctionToken token, 
            IOrderOrchestrationCommand orderOrchestrationCommand, ISupplierSyncCommand supplier, LogQuery log, IProductUpdateCommand productUpdateCommand)
        {
            _orderOrchestrationCommand = orderOrchestrationCommand;
            _productUpdateCommand = productUpdateCommand;
            _log = log;
            _appSettings = appSettings;
        }

        [FunctionName("FreightPOPUICreatedShipmentsSync")]
        public async Task SyncFreightPOPUICreatedShipments([TimerTrigger("0 */30 14-23 * * *")]TimerInfo myTimer, [OrchestrationClient]DurableOrchestrationClient client, ILogger logger)
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
                    await client.StartNewAsync("FreightPOPUICreatedShipmentsSupplierSync", input: supplier);
                    logger.LogInformation($"Finished supplier {supplier.ID}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        [FunctionName("FedexShipManagerCreatedShipmentsSync")]
        public async Task SyncFedexShipManagerCreatedShipments([TimerTrigger("0 15 9 * * *")]TimerInfo myTimer, [OrchestrationClient]DurableOrchestrationClient client, ILogger logger)
        {
            // run every day at 9:15am, FreightPOP third party shipment sync occurs
            // once a day at 9pm, so it only makes sense for us to run this once a day
            // and I feel that 9:15am is a good time since this should trigger emails
            logger.LogInformation("Starting function");
            try
            {
                    await client.StartNewAsync("FedexShipManagerCreatedShipmentSyncProcess", "");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        [FunctionName("EmailProductUpdates")]
        public async Task EmailProductUpdates([TimerTrigger("0 15 9 * * *")] TimerInfo myTimer, [OrchestrationClient] DurableOrchestrationClient client, ILogger logger)
        {
            // run every day at 9:15am
            logger.LogInformation("Starting function");
            try
            {
                await _productUpdateCommand.SendAllProductUpdateEmails();   
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
