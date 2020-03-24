using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JsonDiffPatchDotNet;
using Marketplace.Common.Commands;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OrderCloud.SDK;
using IPartial = Marketplace.Helpers.Models.IPartial;

namespace Orchestration.Tests
{
    public class MockObject
    {
        [MaxLength(500), OrchestrationIgnore]
        public string Name { get; set; }
        public MockObjectXp xp { get; set; } = new MockObjectXp();
    }

    public class MockObjectXp
    {
        [OrchestrationIgnore]
        public string Ignored { get; set; }
        public MockSubObject MockSub { get; set; } = new MockSubObject();
    }

    public class MockSubObject
    {
        public string Key { get; set; }
        [OrchestrationIgnore]
        public string Value { get; set; }
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("fast/guid/product/id.json")]
        public void construct_workitem(string path)
        {
            var wi = new WorkItem(path);
            Assert.AreEqual("fast", wi.ResourceId);
            Assert.AreEqual(RecordType.Product, wi.RecordType);
            Assert.AreEqual("id", wi.RecordId);
        }

        [TestCase("fast/random/id.json")]
        public void construct_workitem_failure(string path)
        {
            Assert.Throws(typeof(OrchestrationException), () =>
            {
                var wi = new WorkItem(path);
            });
        }

        [Test, TestCaseSource(typeof(ActionFactory), nameof(ActionFactory.TestCases))]
        public Action determine_action_results(WorkItem wi)
        {
            //wi.Diff = wi.Current.Diff(wi.Cache);
            var action = WorkItemMethods.DetermineAction(wi);
            return action;
        }

        [Test, TestCaseSource(typeof(DiffFactory), nameof(DiffFactory.TestCases))]
        public JToken diff_results(WorkItem wi)
        {
            var diff = wi.Current.Diff(wi.Cache);
            return diff;
        }

        [Test, TestCaseSource(typeof(SerializerFactory), nameof(SerializerFactory.TestCases))]
        public void serializer_ignore_results(WorkItem wi)
        {
            var diff = wi.Current.Diff(wi.Cache);
            var attempt = diff.ToObject<MockObject>(new JsonSerializer()
            {
                ContractResolver = new OrchestrationSerializer()
            });
            Assert.IsNull(attempt.xp.Ignored);
            Assert.IsNull(attempt.xp.MockSub.Value);
            Assert.IsNull(attempt.Name);
            //Assert.IsNull(attempt.xp.Note);
        }

        [Test, TestCaseSource(typeof(DeletedXpFactory), nameof(DeletedXpFactory.TestCases))]
        public bool has_deleted_xp(WorkItem item)
        {
            var has = item.Cache.HasDeletedXp(item.Current);
            return has;
        }
    }

    
    public class DeletedXpFactory
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new WorkItem()
                {
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'Tax': { 'Category': 'category', 'Code': 'code' }}}"),
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'Tax': { 'Category': 'category', 'Code': 'code', 'Description': 'description' }}}"),
                }).Returns(true);
            }
        }
    }

    public class SerializerFactory
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new WorkItem()
                {
                    Current = JObject.Parse(
                        @"{ 'ID': 'id', 'Name': 'name is changed', 'xp': { 'Ignored': 'attempting', 'Sub': { 'Key': 'category', 'Value': 'code is changed' }}}"),
                    Cache = JObject.Parse(
                        @"{ 'ID': 'id', 'Name': 'name', 'xp': { 'Sub': { 'Ignored': 'cached', 'Key': 'category', 'Value': 'code' }}}"),
                });

                //yield return new TestCaseData(new WorkItem()
                //{
                //    Current = JObject.Parse(
                //        @"{ 'ID': 'id', 'Name': 'name', 'xp': { 'Tax': { 'Category': 'category', 'Code': 'code' }, 'Note': 'changed note'  }}"),
                //    Cache = JObject.Parse(
                //        @"{ 'ID': 'id', 'Name': 'name', 'xp': { 'Tax': { 'Category': 'category', 'Code': 'code' }, 'Note': 'cached note'  }}")
                //});
            }
        }
    }

    public class DiffFactory
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'AutoForward': false}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'AutoForward': true }")
                }).Returns(JObject.Parse(@"{ 'AutoForward': true }"));

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'Token': 'old_token', 'ClientId': 'old_id', 'ID': 'id'}"),
                    Current = JObject.Parse(@"{ 'Token': 'new_token',  'ClientId': 'new_id','ID': 'id' }")
                }).Returns(null);

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id'}"),
                    Current = JObject.Parse(@"{ 'ID': 'id' }")
                }).Returns(null);

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'OptionCount': 5 }"),
                    Current = JObject.Parse(@"{ 'ID': 'id' }")
                }).Returns(null);

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'OptionCount': 5 }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Required': true }")
                }).Returns(JObject.Parse(@"{ 'Required': true }"));

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'OptionCount': 5, 'xp': { 'key': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Required': true, 'xp': { 'key': 'change' } }")
                }).Returns(JObject.Parse(@"{ 'Required': true, 'xp': { 'key': 'change' }}"));

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'xp': { 'key': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'xp': { 'key': 'value', 'nested': { 'key': 'value' }}}")
                }).Returns(JObject.Parse(@"{ 'xp': { 'key': 'value', 'nested': { 'key': 'value' }}}"));

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'xp': { 'key': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'xp': { 'key': 'value', 'nested': { 'key': 'value', 'array': ['1','2'] }}}")
                }).Returns(JObject.Parse(@"{ 'xp': { 'key': 'value', 'nested': { 'key': 'value', 'array': ['1','2'] }}}"));

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'xp': { 'key': 'value', 'nested': { 'key': 'value', 'array': ['1','2'] }}}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'xp': { 'key': 'value', 'nested': { 'key': 'value', 'array': ['1','2','3'] }}}")
                }).Returns(JObject.Parse(@"{ 'xp': { 'key': 'value', 'nested': { 'key': 'value', 'array': ['1','2','3'] }}}"));
            }
        }
    }

    public class ActionFactory
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'Token': 'old_token', 'ClientId': 'old_id', 'Name': 'name' }"),
                    Current = JObject.Parse(@"{ 'Token': 'new_token', 'ClientId': 'new_id', 'Name': 'name' }"),
                    RecordType = RecordType.Buyer
                }).Returns(Action.Get);

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'Name': 'name' }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    RecordType = RecordType.SpecProductAssignment
                }).Returns(Action.Ignore);

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    RecordType = RecordType.SpecProductAssignment
                }).Returns(Action.Ignore);

                // test for cache with xp, current without that results in PATCH
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'key': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'patch'}"),
                    RecordType = RecordType.Product
                }).Returns(Action.Patch);

                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'OptionCount': 2, 'Name': 'name', 'xp': { 'key': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'key': 'updated' }}"),
                    RecordType = RecordType.PriceSchedule
                }).Returns(Action.Patch);

                // cache has additional properties, current has changes
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'OptionCount': 5, 'Name': 'name' }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'patch' }"),
                    RecordType = RecordType.Spec
                }).Returns(Action.Patch);

                // everything the same, except an updated existing property in xp
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'key': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'key': 'updated' }}"),
                    RecordType = RecordType.SpecOption
                }).Returns(Action.Patch);

                // everything the same, except a missing property in current xp
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'key': 'value', 'missing': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'key': 'updated' }}"),
                    RecordType = RecordType.PriceSchedule
                }).Returns(Action.Update);

                // everything the same, except a new property in current xp and missing in cache xp
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'key': 'value', 'missing': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'xp': { 'key': 'updated', 'extra': 'value' }}"),
                    RecordType = RecordType.ProductFacet
                }).Returns(Action.Update);

                // base property change, xp with no change
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'value', 'xp': { 'key': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'patch', 'xp': { 'key': 'value' }}"),
                    RecordType = RecordType.Product
                }).Returns(Action.Patch);

                // no change from cache with xp
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'value', 'xp': { 'key': 'value' }}"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'value', 'xp': { 'key': 'value' }}"),
                    RecordType = RecordType.Product
                }).Returns(Action.Get);

                // simple property update on base object with no xp
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'patch' }"),
                    RecordType = RecordType.Product
                }).Returns(Action.Patch);

                // property change, but not all properties in cache present
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name', 'Active': true }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'patch' }"),
                    RecordType = RecordType.Product
                }).Returns(Action.Patch);

                // property change, but new property not in cache (may not ever be a case since we're caching the OC response)
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'patch', 'Active': true }"),
                    RecordType = RecordType.Product
                }).Returns(Action.Patch);

                // no change from cache, cache has additional properties
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'OptionCount': 5, 'Name': 'name' }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    RecordType = RecordType.Product
                }).Returns(Action.Get);

                // no change from cache
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    RecordType = RecordType.Product
                }).Returns(Action.Get);

                // no cache
                yield return new TestCaseData(new WorkItem()
                {
                    Cache = null,
                    Current = JObject.Parse(@"{ 'ID': 'id', 'Name': 'name' }"),
                    RecordType = RecordType.Product
                }).Returns(Action.Create);
            }
        }
    }
}