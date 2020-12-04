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
using System.Dynamic;
using NSubstitute.Extensions;
using AutoFixture;
using ordercloud.integrations.library;
using Marketplace.Common.Controllers;
using NSubstitute.ReceivedExtensions;

namespace Marketplace.Tests
{
    class UnitPriceTests
    {
        private IOrderCloudClient _oc;
        //private MarketplaceLineItem _lineItem;
        private VerifiedUserContext _verifiedUser;
        private Task<SuperMarketplaceMeProduct> _taskSuperMarketplaceMeProduct;
        private AppSettings _settings;

        [SetUp]
        public void Setup()
        {
            _oc = Substitute.For<IOrderCloudClient>();
            //_lineItem = Substitute.For<MarketplaceLineItem>();
            _verifiedUser = Substitute.For<VerifiedUserContext>();
            //_taskSuperMarketplaceMeProduct = Substitute.For<Task<SuperMarketplaceMeProduct>>(() => null);
            _settings = Substitute.For<AppSettings>();
        }

        public class TestConstants
        {
            public const string orderID = "testorder";
        }

        [Test]
        public async Task GetUnitPrice_FirstPriceBreak_NoMarkups_CumulativeQtyFalse()
        {
            LineItemCommand _commandSub = Substitute.ForPartsOf<LineItemCommand>(default, default, default, default);

            SuperMarketplaceMeProduct product = BuildProductData(false);

            List<MarketplaceLineItem> existingLineItems = BuildExistingLineItemData(); // Existing total quantity is always 2, one for each variant of a single product

            MarketplaceLineItem lineItem = SetLineItemQtyAndNumberOfMarkedUpSpecs(2, 0);

            decimal lineItemTotal = await _commandSub.ValidateLineItemUnitCost(default, product, existingLineItems, lineItem);

            Assert.AreEqual(lineItemTotal, 5);
        }

        [Test]
        public async Task GetUnitPrice_SecondPriceBreak_NoMarkups_CumulativeQtyFalse()
        {
            LineItemCommand _commandSub = Substitute.ForPartsOf<LineItemCommand>(default, default, default, default);

            SuperMarketplaceMeProduct product = BuildProductData(false);

            List<MarketplaceLineItem> existingLineItems = BuildExistingLineItemData(); // Existing total quantity is always 2, one for each variant of a single product

            MarketplaceLineItem lineItem = SetLineItemQtyAndNumberOfMarkedUpSpecs(5, 0);

            decimal lineItemTotal = await _commandSub.ValidateLineItemUnitCost(default, product, existingLineItems, lineItem);

            Assert.AreEqual(lineItemTotal, 3.5);
        }

        [Test]
        public async Task GetUnitPrice_FirstPriceBreak_OneMarkup_CumulativeQtyFalse()
        {
            LineItemCommand _commandSub = Substitute.ForPartsOf<LineItemCommand>(default, default, default, default);

            SuperMarketplaceMeProduct product = BuildProductData(false);

            List<MarketplaceLineItem> existingLineItems = BuildExistingLineItemData(); // Existing total quantity is always 2, one for each variant of a single product

            MarketplaceLineItem lineItem = SetLineItemQtyAndNumberOfMarkedUpSpecs(2, 1);

            decimal lineItemTotal = await _commandSub.ValidateLineItemUnitCost(default, product, existingLineItems, lineItem);

            Assert.AreEqual(lineItemTotal, 7.25);
        }

        [Test]
        public async Task GetUnitPrice_SecondPriceBreak_OneMarkup_CumulativeQtyFalse()
        {
            LineItemCommand _commandSub = Substitute.ForPartsOf<LineItemCommand>(default, default, default, default);

            SuperMarketplaceMeProduct product = BuildProductData(false);

            List<MarketplaceLineItem> existingLineItems = BuildExistingLineItemData(); // Existing total quantity is always 2, one for each variant of a single product

            MarketplaceLineItem lineItem = SetLineItemQtyAndNumberOfMarkedUpSpecs(5, 1);

            decimal lineItemTotal = await _commandSub.ValidateLineItemUnitCost(default, product, existingLineItems, lineItem);

            Assert.AreEqual(lineItemTotal, 5.75);
        }

        [Test]
        public async Task GetUnitPrice_FirstPriceBreak_TwoMarkups_CumulativeQtyFalse()
        {
            LineItemCommand _commandSub = Substitute.ForPartsOf<LineItemCommand>(default, default, default, default);

            SuperMarketplaceMeProduct product = BuildProductData(false);

            List<MarketplaceLineItem> existingLineItems = BuildExistingLineItemData(); // Existing total quantity is always 2, one for each variant of a single product

            MarketplaceLineItem lineItem = SetLineItemQtyAndNumberOfMarkedUpSpecs(2, 2);

            decimal lineItemTotal = await _commandSub.ValidateLineItemUnitCost(default, product, existingLineItems, lineItem);

            Assert.AreEqual(lineItemTotal, 11.25);
        }

        [Test]
        public async Task GetUnitPrice_SecondPriceBreak_TwoMarkups_CumulativeQtyFalse()
        {
            LineItemCommand _commandSub = Substitute.ForPartsOf<LineItemCommand>(default, default, default, default);

            SuperMarketplaceMeProduct product = BuildProductData(false);

            List<MarketplaceLineItem> existingLineItems = BuildExistingLineItemData(); // Existing total quantity is always 2, one for each variant of a single product

            MarketplaceLineItem lineItem = SetLineItemQtyAndNumberOfMarkedUpSpecs(5, 2);

            decimal lineItemTotal = await _commandSub.ValidateLineItemUnitCost(default, product, existingLineItems, lineItem);

            Assert.AreEqual(lineItemTotal, 9.75);
        }

        private SuperMarketplaceMeProduct BuildProductData(bool UseCumulativeQty)
        {
            SuperMarketplaceMeProduct product = Substitute.For<SuperMarketplaceMeProduct>();
            product.PriceSchedule = Substitute.For<PriceSchedule>();
            product.PriceSchedule.UseCumulativeQuantity = UseCumulativeQty;

            PriceBreak priceBreak1 = Substitute.For<PriceBreak>();
            priceBreak1.Quantity = 1;
            priceBreak1.Price = 5;

            PriceBreak priceBreak2 = Substitute.For<PriceBreak>();
            priceBreak2.Quantity = 5;
            priceBreak2.Price = 3.5M;

            product.PriceSchedule.PriceBreaks = new List<PriceBreak> { priceBreak1, priceBreak2 };

            Spec prodSpecSize = Substitute.For<Spec>();
            prodSpecSize.ID = "Size";
            prodSpecSize.Options = new List<SpecOption>() { new PartialSpecOption { ID = "Small", Value = "Small" }, new PartialSpecOption { ID = "Large", Value = "Large", PriceMarkup = 2.25M } };

            Spec prodSpecColor = Substitute.For<Spec>();
            prodSpecColor.ID = "Color";
            prodSpecColor.Options = new List<SpecOption>() { new PartialSpecOption { ID = "Blue", Value = "Blue" }, new PartialSpecOption { ID = "Green", Value = "Green", PriceMarkup = 4 } };

            product.Specs = new List<Spec> { prodSpecSize, prodSpecColor };

            return product;
        }

        private List<MarketplaceLineItem> BuildExistingLineItemData()
        {
            MarketplaceLineItem existinglineItem1 = Substitute.For<MarketplaceLineItem>();
            existinglineItem1.Quantity = 1;

            MarketplaceLineItem existinglineItem2 = Substitute.For<MarketplaceLineItem>();
            existinglineItem2.Quantity = 1;

            List<MarketplaceLineItem> existingLineItems = new List<MarketplaceLineItem> { existinglineItem1, existinglineItem2 };

            return existingLineItems;
        }

        private MarketplaceLineItem SetLineItemQtyAndNumberOfMarkedUpSpecs(int quantity, int numberOfMarkedUpSpecs)
        {
            MarketplaceLineItem lineItem = Substitute.For<MarketplaceLineItem>();

            lineItem.Quantity = quantity;

            LineItemSpec liSpecSizeSmall = Substitute.For<LineItemSpec>();
            liSpecSizeSmall.SpecID = "Size";
            liSpecSizeSmall.OptionID = "Small";

            LineItemSpec liSpecSizeLarge = Substitute.For<LineItemSpec>();
            liSpecSizeLarge.SpecID = "Size";
            liSpecSizeLarge.OptionID = "Large";

            LineItemSpec liSpecColorBlue = Substitute.For<LineItemSpec>();
            liSpecColorBlue.SpecID = "Color";
            liSpecColorBlue.OptionID = "Blue";

            LineItemSpec liSpecColorGreen = Substitute.For<LineItemSpec>();
            liSpecColorGreen.SpecID = "Color";
            liSpecColorGreen.OptionID = "Green";

            if (numberOfMarkedUpSpecs == 0)
            {
                lineItem.Specs = new List<LineItemSpec> { liSpecSizeSmall, liSpecColorBlue };
            } else if (numberOfMarkedUpSpecs == 1)
            {
                lineItem.Specs = new List<LineItemSpec> { liSpecSizeLarge, liSpecColorBlue };
            } else if (numberOfMarkedUpSpecs == 2)
            {
                lineItem.Specs = new List<LineItemSpec> { liSpecSizeLarge, liSpecColorGreen };
            } else
            {
                throw new Exception("The number of marked up specs for this unit test must be 0, 1, or 2");
            }

            return lineItem;
        }
    }

}
