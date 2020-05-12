using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Models;
using ordercloud.integrations.extensions;

namespace Orchestration.Tests
{
    public class ExtensionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void to_credit_card_display_test()
        {
            var visa = "4444333322221111".ToCreditCardDisplay();
            Assert.AreEqual("1111", visa);

            var amex = "373485467448025".ToCreditCardDisplay();
            Assert.AreEqual("8025", amex);
        }

        [Test, TestCaseSource(typeof(TypeFactory), nameof(TypeFactory.TestCases))]
        public void build_path<T>(RecordType type, T obj) where T : IMarketplaceObject
        {
            var model = new OrchestrationObject<T>()
            {
                ClientId = "fake",
                ID = obj.ID,
                Token = "fake",
                Model = obj
            };
            var path = model.BuildPath("supplierid", "clientid");
            Assert.AreEqual(path, $"supplierid/clientid/{obj.Type().ToString().ToLower()}/id");
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
                yield return new TestCaseData(RecordType.Spec, new MarketplaceSpec()
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
                yield return new TestCaseData(RecordType.CostCenter, new MarketplaceCostCenter()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.Address, new MarketplaceAddressBuyer()
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
