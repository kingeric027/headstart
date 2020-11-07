using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using NSubstitute;
using NUnit.Framework;
using ordercloud.integrations.avalara;
using ordercloud.integrations.easypost;
using ordercloud.integrations.exchangerates;
using OrderCloud.SDK;

namespace Marketplace.Tests
{
    public class ShippingTests
    {
        private ICheckoutIntegrationCommand _checkout;
        private IAvalaraCommand _avalara;
        private IExchangeRatesCommand _exchange;
        private IOrderCloudClient _oc;
        private IEasyPostShippingService _shipping;
        private AppSettings _settings;

        [SetUp]
        public void Setup()
        {
            _avalara = Substitute.For<IAvalaraCommand>();
            _exchange = Substitute.For<IExchangeRatesCommand>();
            _oc = Substitute.For<IOrderCloudClient>();
            _shipping = Substitute.For<IEasyPostShippingService>();
            _settings = Substitute.For<AppSettings>();
            _checkout = new CheckoutIntegrationCommand(_avalara, _exchange, _oc, _shipping, _settings);
        }

        [Test, TestCaseSource(typeof(LineItemFactory), nameof(LineItemFactory.LineItemCases))]
        public int TestShipmentGrouping(List<MarketplaceLineItem> line_items)
        {
            var grouped = _checkout.GroupByShipping(line_items);
            return grouped.Count;
        }

        public class LineItemFactory
        {
            public static IEnumerable LineItemCases
            {
                get
                {
                    yield return new TestCaseData(new List<MarketplaceLineItem>()
                    {
                        new MarketplaceLineItem() { 
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Edina", State = "MN", Zip = "55425" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Jackson", State = "MS", Zip = "23548" }
                        }
                    }).Returns(1);
                    yield return new TestCaseData(new List<MarketplaceLineItem>()
                    {
                        new MarketplaceLineItem() {
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Edina", State = "MN", Zip = "55425" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Jackson", State = "MS", Zip = "23548" }
                        },
                        new MarketplaceLineItem() {
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Edina", State = "MN", Zip = "55425" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Jackson", State = "MS", Zip = "23548" }
                        }
                    }).Returns(1);
                    yield return new TestCaseData(new List<MarketplaceLineItem>()
                    {
                        new MarketplaceLineItem() {
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Edina", State = "MN", Zip = "55425" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Jackson", State = "MS", Zip = "23548" }
                        },
                        new MarketplaceLineItem() {
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Wilmington", State = "DE", Zip = "26598" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Jackson", State = "MS", Zip = "23548" }
                        }
                    }).Returns(2);
                    yield return new TestCaseData(new List<MarketplaceLineItem>()
                    {
                        new MarketplaceLineItem() {
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Edina", State = "MN", Zip = "55425" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Jackson", State = "MS", Zip = "23548" }
                        },
                        new MarketplaceLineItem() {
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Edina", State = "MN", Zip = "55425" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Jackson", State = "MS", Zip = "23548" }
                        },
                        new MarketplaceLineItem() {
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Wilmington", State = "DE", Zip = "26598" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Jackson", State = "MS", Zip = "23548" }
                        },
                        new MarketplaceLineItem() {
                            ShipFromAddress = new MarketplaceAddressSupplier() { City = "Wilmington", State = "DE", Zip = "26598" },
                            ShippingAddress = new MarketplaceAddressBuyer() { City = "Austin", State = "TX", Zip = "54677" }
                        }
                    }).Returns(3);
                }
            }
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
