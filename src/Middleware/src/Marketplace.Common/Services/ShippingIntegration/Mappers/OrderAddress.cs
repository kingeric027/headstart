﻿using ordercloud.integrations.freightpop;
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
                Company = obj.CompanyName,
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