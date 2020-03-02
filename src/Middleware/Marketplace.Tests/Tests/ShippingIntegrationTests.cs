using System.Collections.Generic;
using NUnit.Framework;
using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Newtonsoft.Json;

namespace Marketplace.Tests
{

    public class ShippingIntegrationTests
    {
        [Test]
        public void ShouldReturnCheapestShippingOptionsForEachDeliveryDays()
        {
            // Arrange
            var testRates = GetShippingRatesFullList();

            // Act
            var result = ProposedShipmentOptionsMapper.Map(testRates);

            // Assert
            var expectedProposedShipments = GetProposedShipmentCheapestList();

            var expectedProposedShipmentString = JsonConvert.SerializeObject(expectedProposedShipments);
            var resultString = JsonConvert.SerializeObject(result);
            Assert.AreEqual(resultString, expectedProposedShipmentString);
        }

        private List<ShippingRate> GetShippingRatesFullList()
        {
            return new List<ShippingRate>()
            {
                new ShippingRate() {
                    Id = "FedexParcel-fab38772-74c0-4956-8856-28f2f2489ee7",
                    AccountName = "Four51 FedEx",
                    Carrier = "Fed-Ex",
                    Currency = "USD",
                    DeliveryDate = null,
                    DeliveryDays = 4,
                    QuoteId = "Q593435",
                    CarrierQuoteId = null,
                    Service = "FEDEX_GROUND",
                    ListCost = 235.05M,
                    TotalCost = 235.05M
                },
                new ShippingRate() {
                    Id = "FedexParcel-63607797-48e1-41ec-b435-0c762fd63724",
                    AccountName = "Four51 FedEx",
                    Carrier = "Fed-Ex",
                    Currency = "USD",
                    DeliveryDate = null,
                    DeliveryDays = 3,
                    QuoteId = "Q593435",
                    CarrierQuoteId = null,
                    Service = "FEDEX_EXPRESS_SAVER",
                    ListCost = 829.45M,
                    TotalCost = 750.6M
                },
                new ShippingRate() {
                    Id = "FedexParcel-79d89afe-0644-47f9-8759-a2e667606fa2",
                    AccountName = "Four51 FedEx",
                    Carrier = "Fed-Ex",
                    Currency = "USD",
                    DeliveryDate = null,
                    DeliveryDays = 2,
                    QuoteId = "Q593435",
                    CarrierQuoteId = null,
                    Service = "FEDEX_2_DAY",
                    ListCost = 945.00M,
                    TotalCost = 855.15M
                },
                new ShippingRate() {
                    Id = "FedexParcel-a9d32df6-1db3-4a99-8583-4f6040f17250",
                    AccountName = "Four51 FedEx",
                    Carrier = "Fed-Ex",
                    Currency = "USD",
                    DeliveryDate = null,
                    DeliveryDays = 2,
                    QuoteId = "Q593435",
                    CarrierQuoteId = null,
                    Service = "FEDEX_2_DAY_AM",
                    ListCost = 1060.16M,
                    TotalCost = 959.36M
                },
                new ShippingRate() {
                    Id = "FedexParcel-4c22e232-0da8-4a29-a826-9a47e3a8eaa0",
                    AccountName = "Four51 FedEx",
                    Carrier = "Fed-Ex",
                    Currency = "USD",
                    DeliveryDate = null,
                    DeliveryDays = 1,
                    QuoteId = "Q593435",
                    CarrierQuoteId = null,
                    Service = "STANDARD_OVERNIGHT",
                    ListCost = 1222.66M,
                    TotalCost = 1106.41M
                },
                new ShippingRate() {
                    Id = "FedexParcel-affa77c8-660e-43dd-904d-17a3fad18d42",
                    AccountName = "Four51 FedEx",
                    Carrier = "Fed-Ex",
                    Currency = "USD",
                    DeliveryDate = null,
                    DeliveryDays = 1,
                    QuoteId = "Q593435",
                    CarrierQuoteId = null,
                    Service = "PRIORITY_OVERNIGHT",
                    ListCost = 1251.06M,
                    TotalCost = 1132.11M
                },
                new ShippingRate() {
                    Id = "FedexParcel-945e1a69-fc4d-4a8e-b9d0-e9e911071777",
                    AccountName = "Four51 FedEx",
                    Carrier = "Fed-Ex",
                    Currency = "USD",
                    DeliveryDate = null,
                    DeliveryDays = 1,
                    QuoteId = "Q593435",
                    CarrierQuoteId = null,
                    Service = "FIRST_OVERNIGHT",
                    ListCost = 1305.48M,
                    TotalCost = 1305.48M
                }
            };
        }

        private List<ProposedShipmentOption> GetProposedShipmentCheapestList()
        {
            return new List<ProposedShipmentOption>()
            {
                new ProposedShipmentOption() {
                    Cost = (decimal)235.05,
                    EstimatedDeliveryDays = 4,
                    ID = "FedexParcel-fab38772-74c0-4956-8856-28f2f2489ee7",
                    Name = "FEDEX_GROUND",
                },
                new ProposedShipmentOption() {
                    ID = "FedexParcel-63607797-48e1-41ec-b435-0c762fd63724",
                    EstimatedDeliveryDays = 3,
                    Name= "FEDEX_EXPRESS_SAVER",
                    Cost = (decimal)750.6,
                },
                new ProposedShipmentOption() {
                    ID = "FedexParcel-79d89afe-0644-47f9-8759-a2e667606fa2",
                    EstimatedDeliveryDays = 2,
                    Name = "FEDEX_2_DAY",
                    Cost = (decimal)855.15,
                },        
                new ProposedShipmentOption() {
                    ID = "FedexParcel-4c22e232-0da8-4a29-a826-9a47e3a8eaa0",
                    EstimatedDeliveryDays = 1,
                    Name = "STANDARD_OVERNIGHT",
                    Cost = (decimal)1106.41,
                },
            };
        }
    }
}
