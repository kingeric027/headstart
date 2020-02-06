using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class AccountDetailsMapper
    {
        public static AccountDetails Map(Supplier supplier, Address address)
        {
            return new AccountDetails
            {
                AccountNumber = supplier.ID,
                Address = address.Street1,
                City = address.City,
                Country = address.Country,

                // placeholder
                Email = "test@test.com",
                Name = supplier.Name,
                
                // placeholder
                Phone = "1231231232",
                State = address.State,
                Zip = address.Zip
            };
        }
    }
}