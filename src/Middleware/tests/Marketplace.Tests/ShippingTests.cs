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

        [Test]
        public void TestMapParcel()
        {
            //var line_items = new List<LineItem>()
            //{
            //    new LineItem() { Product = new LineItemProduct() { ShipLength = 5, ShipWidth = 5, ShipHeight = 5, ShipWeight = 1}}
            //};
            //var mapped = EasyPostMappers.MapParcel(line_items);
            //var expected = new EasyPostParcel()
            //{
            //    weight = 1,
            //    height = 5,
            //    width = 5,
            //    length = 5
            //};
            //Assert.AreEqual(mapped.weight, expected.weight);
            //Assert.AreEqual(mapped.height, expected.height);
            //Assert.AreEqual(mapped.length, expected.length);
            //Assert.AreEqual(mapped.width, expected.width);
        }

        //[Test, TestCaseSource(typeof(ParcelFactory), nameof(ParcelFactory.ParcelCases))]
        //public EasyPostParcel calculates_dimensions(IList<LineItem> items)
        //{
        //    //var mapped = EasyPostMappers.MapParcel(items);
        //    //return mapped;
        //}

        public class ParcelFactory
        {
            public static IEnumerable ParcelCases
            {
                get
                {
                    yield return new TestCaseData(new List<LineItem>()
                    {
                        new LineItem() { Product = new LineItemProduct() { ShipLength = 5, ShipWidth = 5, ShipHeight = 5, ShipWeight = 1 }}
                    }).Returns(new EasyPostParcel() { height = 5, length = 5, weight = 1, width = 5, created_at = null, id = null, mode = null, predefined_package = null, updated_at = null });
                }
            }
        }
    }
}
