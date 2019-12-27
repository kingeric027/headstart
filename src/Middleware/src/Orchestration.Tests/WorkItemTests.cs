using System.Collections;
using System.Threading.Tasks;
using Cosmonaut;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using WorkItem = Marketplace.Common.Models.WorkItem;

namespace Orchestration.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("fast/product/id.json")]
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
        public async Task<Action> determine_action_results(WorkItem wi)
        {
            var command = new OrchestrationCommand(Substitute.For<AppSettings>(), new LogQuery(Substitute.For<ICosmosStore<OrchestrationLog>>()));
            wi.Diff = await command.CalculateDiff(wi);
            var action = await command.DetermineAction(wi);
            return action;
        }

        [Test, TestCaseSource(typeof(DiffFactory), nameof(DiffFactory.TestCases))]
        public async Task<JObject> diff_results(WorkItem wi)
        {
            var command = new OrchestrationCommand(Substitute.For<AppSettings>(), new LogQuery(Substitute.For<ICosmosStore<OrchestrationLog>>()));
            var diff = await command.CalculateDiff(wi);
            return diff;
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