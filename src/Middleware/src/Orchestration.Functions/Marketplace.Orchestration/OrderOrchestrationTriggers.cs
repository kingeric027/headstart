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

namespace Marketplace.ShippingQuery
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
        public async void SyncOrders([TimerTrigger("0 */10 14-23 * * *")]TimerInfo myTimer, [OrchestrationClient]DurableOrchestrationClient client, ILogger logger)
        {
            // run every 30 minutes between 9am and 6pm CDT
            // determine if different schedule or order range is needed for production
            try
            {
                var suppliersToSync = await _orderOrchestrationCommand.GetSuppliersNeedingSync();
                foreach (var supplier in suppliersToSync)
                {
                    var supplierOrderCloudClient = new OrderCloudClient(new OrderCloudClientConfig
                    {
                        ApiUrl = _appSettings.OrderCloudSettings.ApiUrl,
                        AuthUrl = _appSettings.OrderCloudSettings.AuthUrl,
                        ClientId = supplier.xp.ApiClientID,
                        ClientSecret = _appSettings.OrderCloudSettings.ClientSecret,
                        Roles = new[]
                            {
                                 ApiRole.FullAccess
                            }
                    });
                    await supplierOrderCloudClient.AuthenticateAsync();
                    var supplierToken = supplierOrderCloudClient.TokenResponse.AccessToken;
                    await client.StartNewAsync("ShipmentSyncOrchestration", supplierToken);
                }
            } catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
