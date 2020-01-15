using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using OrderCloud.SDK;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.Extensions;

namespace Orchestration.Tests
{
    public class ExtensionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test, TestCaseSource(typeof(TypeFactory), nameof(TypeFactory.TestCases))]
        public void build_path<T>(RecordType type, T obj) where T : IMarketplaceObject
        {
            // TODO: Address ID here
            var model = new OrchestrationObject<T>()
            {
                ClientId = "fake",
                //ID = obj.ID,
                Token = "fake",
                Model = obj
            };
            var path = model.BuildPath("supplier");
            Assert.AreEqual(path, $"supplier/{obj.Type().ToString().ToLower()}/id");
        }

        [Test]
        public void join_string_words()
        {
            var list = new List<RecordType>() { RecordType.Product, RecordType.Spec};
            Assert.IsTrue(list.JoinString("|") == "Product|Spec");
        }

        [Test, TestCaseSource(typeof(TypeFactory), nameof(TypeFactory.TestCases))]
        public void test_type_deriver<T>(RecordType type, T obj) where T : IMarketplaceObject
        {
            Assert.AreEqual(type, obj.Type());
        }
    }

    public class TypeFactory
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(RecordType.Product, new MarketplaceProduct()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.ProductFacet, new MarketplaceProductFacet()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.SpecProductAssignment, new MarketplaceSpecProductAssignment()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.PriceSchedule, new MarketplacePriceSchedule()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.Spec, new Marketplace.Helpers.Models.MarketplaceSpec()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.SpecOption, new MarketplaceSpecOption()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.Buyer, new MarketplaceBuyer()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.User, new MarketplaceUser()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.UserGroup, new MarketplaceUserGroup()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.UserGroupAssignment, new MarketplaceUserGroupAssignment()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.CostCenter, new BaseCostCenter()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.Address, new Marketplace.Helpers.Models.MarketplaceAddress()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.AddressAssignment, new MarketplaceAddressAssignment()
                {
                    ID = "id"
                });
            }
        }
    }
}
