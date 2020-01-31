using OrderCloud.SDK;
using Marketplace.Common.Services.FreightPop;
using System;
using System.Collections.Generic;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class AddressValidationRateRequestMapper
    {
        public static RateRequestBody Map(Address obj)
        {
            // using random number generation on the lineitems to prevent auth errors in freightpop
            // likely needed due to duplicate rate request limiting
            Random random = new Random();
            var dummyValidLineItemProduct = new LineItemProduct
            {
                ID = random.Next(10000, 1000000).ToString(),
                Name = "0607 - Unisex-Polo Again",
                Description = "Soft pique unisex polo with athletic fit.  Double ridge tipped collar  curved placket with two buttons and one logo snap.  Tagless.  Twill taped neckline and side vents  back yoke  and drop tail. 60% Cotton/40% Polyester",
                QuantityMultiplier = 1,
                ShipWeight = random.Next(1, 5),
                ShipHeight = random.Next(1, 5),
                ShipWidth = random.Next(1, 5),
                ShipLength = random.Next(1, 5),
                xp = new { }
            };
            var dummyValidLineItemVariant = new LineItemVariant
            {
                ID = "L0708695774557-black-medium",
                Name = null,
                Description = null,
                ShipWeight = null,
                ShipHeight = null,
                ShipWidth = null,
                ShipLength = null,
                xp = null
            };
            var dummyValidAddress = new Address
            {
                ID = null,
                DateCreated = null,
                CompanyName = null,
                FirstName = "Paul",
                LastName = "Coll Jr",
                Street1 = "1610 NW 130th St",
                Street2 = "",
                City = "Miami",
                State = "FL",
                Zip = "33167",
                Country = "US",
                Phone = "12231234",
                AddressName = "Paul Coll Residential",
                xp = null
            };
            var dummyLineItem = new LineItem
            {
                ID = random.Next(10000, 1000000).ToString(),
                ProductID = random.Next(10000, 1000000).ToString(),
                Quantity = 1,
                DateAdded = DateTime.Parse("2020-01-16T23:16:33.187+00:00"),
                QuantityShipped = 0,
                UnitPrice = 11.24M,
                LineTotal = 11.24M,
                CostCenter = null,
                DateNeeded = null,
                ShippingAccount = null,
                ShippingAddressID = null,
                ShipFromAddressID = "warehouse1",
                Product = dummyValidLineItemProduct,
                Variant = dummyValidLineItemVariant,
                ShippingAddress = dummyValidAddress,
                ShipFromAddress = obj,
                SupplierID = "Target",
                Specs = null
            };
            var lineItemList = new List<LineItem>
            {
                dummyLineItem
            };
            return RateRequestBodyMapper.Map(lineItemList);
        }
    }
}