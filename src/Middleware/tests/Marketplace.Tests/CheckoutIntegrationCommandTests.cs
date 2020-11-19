using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services;
using ordercloud.integrations.avalara;
using OrderCloud.SDK;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models;
using Marketplace.Common;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.Common;
using SendGrid.Helpers.Mail;
using System.Dynamic;
using NSubstitute.Extensions;
using AutoFixture;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.easypost;

namespace Marketplace.Tests
{
    public class CheckoutIntegrationCommandTests
    {
        [Test]
        public void dont_filter_valid_rates()
        {
            // preserve order, rates are already correct
            var method1 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 1,
                Cost = 25
            };
            var method2 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 2,
                Cost = 15
            };
            var method3 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 3,
                Cost = 5
            };
            var estimates = BuildEstimates(method1, method2, method3);
            var result = CheckoutIntegrationCommand.FilterSlowerRatesWithHighCost(estimates);
            var methods = result[0].ShipMethods;

            Assert.AreEqual(3, methods.Count);
            Assert.AreEqual(25, methods[0].Cost);
            Assert.AreEqual(15, methods[1].Cost);
            Assert.AreEqual(5, methods[2].Cost);
        }

        [Test]
        public void remove_invalid_method()
        {
            // remove one offending ship method
            var method1 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 1,
                Cost = 10
            };
            var method2 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 2,
                Cost = 5
            };
            var method3 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 3,
                Cost = 20
            };
            var estimates = BuildEstimates(method1, method2, method3);
            var result = CheckoutIntegrationCommand.FilterSlowerRatesWithHighCost(estimates);
            var methods = result[0].ShipMethods;

            Assert.AreEqual(2, methods.Count);
            Assert.AreEqual(10, methods[0].Cost);
            Assert.AreEqual(5, methods[1].Cost);
        }

        [Test]
        public void remove_two_invalid_methods()
        {
            // remove two offending ship methods
            var method1 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 1,
                Cost = 5
            };
            var method2 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 2,
                Cost = 10
            };
            var method3 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 3,
                Cost = 15
            };
            var estimates = BuildEstimates(method1, method2, method3);
            var result = CheckoutIntegrationCommand.FilterSlowerRatesWithHighCost(estimates);
            var methods = result[0].ShipMethods;

            Assert.AreEqual(1, methods.Count);
            Assert.AreEqual(5, methods[0].Cost);
        }

        [Test]
        public void handle_mixed_order_by_transit_days()
        {
            // remove two offending ship methods, transit days ordered backwards
            var method1 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 3,
                Cost = 15
            };
            var method2 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 2,
                Cost = 10
            };
            var method3 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 1,
                Cost = 5
            };
            var estimates = BuildEstimates(method1, method2, method3);
            var result = CheckoutIntegrationCommand.FilterSlowerRatesWithHighCost(estimates);
            var methods = result[0].ShipMethods;

            Assert.AreEqual(1, methods.Count);
            Assert.AreEqual(5, methods[0].Cost);
        }

        [Test]
        public void handle_free_shipping()
        {
            // handle free shipping
            var method1 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 1,
                Cost = 15
            };
            var method2 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 2,
                Cost = 0
            };
            var method3 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 3,
                Cost = 5
            };
            var estimates = BuildEstimates(method1, method2, method3);
            var result = CheckoutIntegrationCommand.FilterSlowerRatesWithHighCost(estimates);
            var methods = result[0].ShipMethods;

            Assert.AreEqual(2, methods.Count);
            Assert.AreEqual(15, methods[0].Cost);
            Assert.AreEqual(0, methods[1].Cost);
        }

        [Test]
        public void handle_methods_with_same_rates()
        {
            // handle two estimates with same rates
            // we do not want to filter out a slower estimate with the same rate
            var method1 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 1,
                Cost = 15
            };
            var method2 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 2,
                Cost = 15
            };
            var method3 = new MarketplaceShipMethod
            {
                EstimatedTransitDays = 3,
                Cost = 5
            };
            var estimates = BuildEstimates(method1, method2, method3);
            var result = CheckoutIntegrationCommand.FilterSlowerRatesWithHighCost(estimates);
            var methods = result[0].ShipMethods;

            Assert.AreEqual(3, methods.Count);
            Assert.AreEqual(15, methods[0].Cost);
            Assert.AreEqual(1, methods[0].EstimatedTransitDays);
            Assert.AreEqual(5, methods[2].Cost);
            Assert.AreEqual(3, methods[2].EstimatedTransitDays);
        }

        private List<MarketplaceShipEstimate> BuildEstimates(params MarketplaceShipMethod[] shipMethods)
        {
            return new List<MarketplaceShipEstimate>
            {
                new MarketplaceShipEstimate
                {
                    ShipMethods = shipMethods.ToList()
                }
            };
        }
    }
}
