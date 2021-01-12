using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Headstart.Common;
using Headstart.Common.Commands;
using Headstart.Common.Models;
using Headstart.Common.Services.ShippingIntegration.Models;
using NSubstitute;
using NUnit.Framework;
using ordercloud.integrations.avalara;
using ordercloud.integrations.easypost;
using ordercloud.integrations.exchangerates;
using OrderCloud.SDK;

namespace Headstart.Tests
{
    public class ShippingTests
    {
        [SetUp]
        public void Setup()
        {

        }

        public class LineItemFactory
        {
            public static IEnumerable LineItemCases
            {
                get
                {
                    yield return new TestCaseData(new LineItem()
                    {
                        Product = new LineItemProduct() { ShipLength = 5, ShipWidth = 5, ShipHeight = 5, ShipWeight = 5 },
                        Variant = null
                    }).Returns(5);
                    yield return new TestCaseData(new LineItem()
                    {
                        Product = new LineItemProduct() { ShipLength = 5, ShipWidth = 5, ShipHeight = 5, ShipWeight = 5 },
                        Variant = new LineItemVariant() { ShipLength = 10, ShipHeight = 10, ShipWeight = 10, ShipWidth = 10}
                    }).Returns(10);
                    yield return new TestCaseData(new LineItem()
                    {
                        Product = new LineItemProduct() { ShipLength = null, ShipWidth = null, ShipHeight = null, ShipWeight = null },
                        Variant = new LineItemVariant() { ShipLength = null, ShipHeight = null, ShipWeight = null, ShipWidth = null }
                    }).Returns(100);
                }
            }
        }

        [Test, TestCaseSource(typeof(LineItemFactory), nameof(LineItemFactory.LineItemCases))]
        public double TestShipDimensions(LineItem item)
        {
            return item.ShipWeightOrDefault(100);
        }

        [Test]
        public void TestSupplierFilter()
        {
            var command = new CheckoutIntegrationCommand(
                Substitute.For<IAvalaraCommand>(),
                Substitute.For<IExchangeRatesCommand>(),
                Substitute.For<IOrderCloudClient>(),
                Substitute.For<IEasyPostShippingService>(),
                Substitute.For<AppSettings>()
            );

            var mockMethods = new List<HSShipMethod>()
            {
                new HSShipMethod() {Name = "FEDEX_GROUND"},
                new HSShipMethod() {Name = "USPS Priority"},
                new HSShipMethod() {Name = "UPS GROUND"}
            };

            var mockNotFoundMethods = new List<HSShipMethod>()
            {
                new HSShipMethod() {Name = "USPS Priority"},
                new HSShipMethod() {Name = "UPS GROUND"}
            };

            var settings = new AppSettings()
            {
                OrderCloudSettings = new OrderCloudSettings()
                {
                    FirstChoiceSupplierID = "050"
                }
            };

            var mockProfiles = new SelfEsteemBrandsShippingProfiles(settings);

            var configured_filter = command.FilterMethodsBySupplierConfig(mockMethods, mockProfiles.FirstOrDefault("050"));
            var misconfigured_filter = command.FilterMethodsBySupplierConfig(mockNotFoundMethods, mockProfiles.FirstOrDefault("050"));
            var unconfigured_filter = command.FilterMethodsBySupplierConfig(mockMethods, mockProfiles.FirstOrDefault(null));
            Assert.IsTrue(configured_filter.Count() == 1);
            Assert.IsTrue(unconfigured_filter.Count() == 3);
            Assert.IsTrue(misconfigured_filter.Count() == 2);
        }
    }
}
