using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using ordercloud.integrations.easypost;
using OrderCloud.SDK;

namespace Marketplace.Tests
{
    public class ShippingTests
    {
        [SetUp]
        public void Setup()
        {

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
