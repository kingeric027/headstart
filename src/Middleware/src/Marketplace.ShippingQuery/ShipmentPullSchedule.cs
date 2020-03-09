using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Marketplace.Common.Services.ShippingIntegration;

namespace Marketplace.ShippingQuery
{
    public class ShipmentPullSchedule
    {
        private readonly IShipmentQuery _shipmentQuery;

        public ShipmentPullSchedule(IShipmentQuery shipmentQuery)
        {
            _shipmentQuery = shipmentQuery;
        }

        [FunctionName("ShipmentQuery")]
        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            // "0 */5 * * * *"
            // "0 0 15-23 * * *"
            // running test once every hour from 9am to 5pm CST
            _shipmentQuery.SyncShipments();
        }
    }
}
