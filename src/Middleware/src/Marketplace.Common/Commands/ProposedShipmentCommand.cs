using Marketplace.Common.Services;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IProposedShipmentCommand
    {
        Task<MarketplaceListPage<ProposedShipment>> ListProposedShipments(string orderId, VerifiedUserContext userContext);
    }
    public class ProposedShipmentCommand : IProposedShipmentCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;
        public ProposedShipmentCommand(IFreightPopService freightPopService, IOrderCloudClient oc, IOCShippingIntegration ocShippingIntegration)
        {
            _freightPopService = freightPopService;
            _oc = oc;
            _ocShippingIntegration = ocShippingIntegration;
        }

        public async Task<MarketplaceListPage<ProposedShipment>> ListProposedShipments(string orderId, VerifiedUserContext userContext)
        {
            var order = await _oc.Orders.GetAsync(OrderDirection.Outgoing, orderId, userContext.AccessToken);

            // update to accomodate lots of lineitems
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, orderId, accessToken: userContext.AccessToken);
            var superOrder = new SuperOrder
            {
                Order = order,
                LineItems = lineItems.Items.ToList(),
            };

            var proposedShipments = await _ocShippingIntegration.GetProposedShipmentsForSuperOrderAsync(superOrder);

            return new MarketplaceListPage<ProposedShipment>()
            {
                Items = proposedShipments,
                Meta = new MarketplaceListPageMeta
                {
                    Page = 1,
                    PageSize = 100,
                    TotalCount = proposedShipments.Count(),
                }
            };
        }
    }
}
