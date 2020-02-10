using System.Collections.Generic;
using Marketplace.Common.Services.FreightPop.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class AddressValidationRateRequestMapper
    {
        public static RateRequestBody Map(Address obj)
        {
            var dummyValidLineItemProduct = new LineItemProduct
            {
                ShipWeight = 1,
                ShipHeight = 1,
                ShipWidth = 1,
                ShipLength = 1
            };
            var dummyValidAddress = new Address
            {
                Street1 = "1610 NW 130th St",
                City = "Miami",
                State = "FL",
                Zip = "33167",
                Country = "US",
            };
            var dummyLineItem = new LineItem
            {
                Quantity = 1,
                Product = dummyValidLineItemProduct,
                ShippingAddress = dummyValidAddress,
                ShipFromAddress = obj,
            };
            var lineItemList = new List<LineItem>
            {
                dummyLineItem
            };
            return RateRequestBodyMapper.Map(lineItemList);
        }

        public static RateRequestBody Map(BuyerAddress obj)
        {
            var dummyValidLineItemProduct = new LineItemProduct
            {
                ShipWeight = 1,
                ShipHeight = 1,
                ShipWidth = 1,
                ShipLength = 1
            };
            var dummyValidAddress = new Address
            {
                Street1 = "1610 NW 130th St",
                City = "Miami",
                State = "FL",
                Zip = "33167",
                Country = "US",
            };
            var dummyLineItem = new LineItem
            {
                Quantity = 1,
                Product = dummyValidLineItemProduct,
                ShippingAddress = dummyValidAddress,
                ShipFromAddress = new Address()
                {
                    AddressName = obj.AddressName,
                    City = obj.City,
                    CompanyName = obj.CompanyName,
                    Country = obj.Country,
                    FirstName = obj.FirstName,
                    ID = obj.ID,
                    LastName = obj.LastName,
                    Phone = obj.Phone,
                    Street1 = obj.Street1,
                    Street2 = obj.Street2,
                    State = obj.State,
                    Zip = obj.Zip
                }
            };
            var lineItemList = new List<LineItem>
            {
                dummyLineItem
            };
            return RateRequestBodyMapper.Map(lineItemList);
        }
    }
}