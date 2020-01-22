using Flurl.Http;
using Marketplace.Common.Models;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services
{
    public interface IOCShippingIntegration
    {
        Task<List<ProposedShipment>> GetProposedShipmentsForSuperOrderAsync(SuperOrder superOrder);
    }
    class OCShippingIntegration : IOCShippingIntegration
    {
        IFreightPopService _freightPopService;
        public OCShippingIntegration(IFreightPopService freightPopService)
        {
            _freightPopService = freightPopService;
        }

        public async Task<List<ProposedShipment>> GetProposedShipmentsForSuperOrderAsync(SuperOrder superOrder)
        {
            var proposedShipmentRequests = ProposedShipmentRequestsMapper.Map(superOrder);
            proposedShipmentRequests = proposedShipmentRequests.Select(proposedShipmentRequest =>
            {
                proposedShipmentRequest.RateResponseTask = _freightPopService.GetRatesAsync(proposedShipmentRequest.RateRequestBody);
                return proposedShipmentRequest;
            }).ToList();

            var tasks = proposedShipmentRequests.Select(p => p.RateResponseTask);
            await Task.WhenAll(tasks);

            var proposedShipments = new List<ProposedShipment>();
            foreach(var proposedShipmentRequest in proposedShipmentRequests)
            {
                proposedShipments.Add(ProposedShipmentMapper.Map(proposedShipmentRequest));
            }

            return proposedShipments;
        }
    }
}

