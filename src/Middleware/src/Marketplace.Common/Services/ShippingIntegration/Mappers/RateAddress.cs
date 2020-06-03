using ordercloud.integrations.freightpop;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class RateAddressMapper
    {
        public static RateAddress Map(Address obj)
        {
            var rateAddress = new RateAddress
            {
                // why does the rate address only have one street and how can we ensure that 
                // all of the necessary information gets included here (concatenate 1 and 2?)
                Street = obj.Street1,
                City = obj.City,
                State = obj.State,
                Country = obj.Country,
                Zip = obj.Zip
            };
            return rateAddress;
        }
    }
}