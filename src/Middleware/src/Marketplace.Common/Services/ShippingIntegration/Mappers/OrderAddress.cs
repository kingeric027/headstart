using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.FreightPop.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class OrderAddressMapper
    {
        public static OrderAddress Map(Address obj)
        {
            var rateAddress = new OrderAddress
            {
                // why does the rate address only have one street and how can we ensure that 
                // all of the necessary information gets included here (concatenate 1 and 2?)
                Street1 = obj.Street1,
                Street2 = obj.Street2,
                Phone = obj.Phone,
                City = obj.City,
                State = obj.State,
                Country = obj.Country,
                Zip = obj.Zip
            };
            return rateAddress;
        }
    }
}