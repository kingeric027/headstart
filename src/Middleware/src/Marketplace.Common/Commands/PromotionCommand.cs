using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IPromotionCommand
    {
        Task AutoApplyPromotions(string orderID);
    }

    public class PromotionCommand : IPromotionCommand
    {
        private readonly IOrderCloudClient _oc;
        public PromotionCommand(IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public async Task AutoApplyPromotions(string orderID)
        {
            try
            {
                await _oc.Orders.ValidateAsync(OrderDirection.Incoming, orderID);
            }
            catch (Exception ex)
            {
                await RemoveOrderPromotions(orderID);
            }

            var autoEligablePromos = await _oc.Promotions.ListAsync(filters: "xp.Automatic=true");
            foreach (var promo in autoEligablePromos.Items)
            {
                try
                {
                    await _oc.Orders.AddPromotionAsync(OrderDirection.Incoming, orderID, promo.Code);
                }
                catch
                {
                    continue;
                }
            }
        }

        private async Task RemoveOrderPromotions(string orderID)
        {
            var curPromos = await _oc.Orders.ListPromotionsAsync(OrderDirection.Incoming, orderID);
            var removeQueue = new List<Task>();
            foreach (var promo in curPromos.Items)
            {
                removeQueue.Add(_oc.Orders.RemovePromotionAsync(OrderDirection.Incoming, orderID, promo.Code));
            }
            await Task.WhenAll(removeQueue);
        }
    }
}
