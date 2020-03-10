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
        public void RunFullQuery([TimerTrigger("0 */10 14-23 * * *")]TimerInfo myTimer, ILogger logger)
        {
            // run every 10 minutes between 9am and 6pm CDT
            // only currently running on orders made in the last day
            _shipmentQuery.SyncShipments(logger);
        }

        [FunctionName("ShipmentQueryLatestOrder")]
        public void RunIndividualOrder([TimerTrigger("0 0 0 1 1 *")]TimerInfo myTimer, ILogger logger)
        {
            // only runs once a year on jan 1, used because we can manually trigger from azure ui
            _shipmentQuery.SyncLatestOrder(logger);
        }
    }
}
