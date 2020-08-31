using System.Collections;
using Marketplace.Common.Commands.SupplierSync;
using NUnit.Framework;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Models;

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
                yield return new TestCaseData(RecordType.HydratedProduct,
                    new TemplateHydratedProduct()
                    {
                        Product = new TemplateProduct() {ID = "id"}
                    });
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
