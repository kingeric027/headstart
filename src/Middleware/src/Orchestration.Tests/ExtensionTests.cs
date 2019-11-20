using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Marketplace.Common.Extensions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using OrderCloud.SDK;

namespace Orchestration.Tests
{
    public class ExtensionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test, TestCaseSource(typeof(TypeFactory), nameof(TypeFactory.TestCases))]
        public void build_path<T>(RecordType type, T obj) where T : IOrchestrationObject
        {
            var path = obj.BuildPath("supplier");
            Assert.AreEqual(path, $"supplier/{obj.Type().ToString().ToLower()}/id");
        }

        [Test]
        public void join_string_words()
        {
            var list = new List<RecordType>() { RecordType.Product, RecordType.Spec};
            Assert.IsTrue(list.JoinString("|") == "Product|Spec");
        }

        [Test, TestCaseSource(typeof(TypeFactory), nameof(TypeFactory.TestCases))]
        public void test_type_deriver<T>(RecordType type, T obj) where T : IOrchestrationObject
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
                yield return new TestCaseData(RecordType.Product, new OrchestrationProduct()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.ProductFacet, new OrchestrationProductFacet()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.SpecProductAssignment, new OrchestrationSpecProductAssignment()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.PriceSchedule, new OrchestrationPriceSchedule()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.Spec, new OrchestrationSpec()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.SpecOption, new OrchestrationSpecOption()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.Buyer, new OrchestrationBuyer()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.User, new OrchestrationUser()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.UserGroup, new OrchestrationUserGroup()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.UserGroupAssignment, new OrchestrationUserGroupAssignment()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.CostCenter, new OrchestrationCostCenter()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.Address, new OrchestrationAddress()
                {
                    ID = "id"
                });
                yield return new TestCaseData(RecordType.AddressAssignment, new OrchestrationAddressAssignment()
                {
                    ID = "id"
                });
            }
        }
    }
}
