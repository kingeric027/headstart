using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents.SystemFunctions;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class AddressMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static Address Map(BuyerAddress b)
        {
            return new Address
            {
                ID = b.ID,
                DateCreated = b.DateCreated,
                CompanyName = b.CompanyName,
                FirstName = b.FirstName,
                LastName = b.LastName,
                Street1 = b.Street1,
                Street2 = b.Street2,
                City = b.City,
                State = b.State,
                Zip = b.Zip,
                Country = b.Country,
                Phone = b.Phone,
                AddressName = b.AddressName,
                xp = b.xp
            };
        }
    }
}
