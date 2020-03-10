using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Helpers.Models;
using NUnit.Framework;
using OrderCloud.SDK;

namespace Marketplace.Helpers.Tests
{
    public class ModelBinderTests
    {
        [SetUp]
        public void Setup() { }

        [Test, TestCaseSource(typeof(ArgsFactory), nameof(ArgsFactory.TestCases))]
        public void model_binder_filter_string_match(string expression, List<Tuple<string, string>> args)
        {
            var query = new ListArgs<Product>()
            {
                Filters = new List<ListFilter>()
                {
                    new ListFilter()
                    {
                        QueryParams = args
                    }
                }
            };
            Assert.IsTrue(expression == query.ToFilterString());
        }
    }

    public class ArgsFactory
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData("xp.Facets.color=blue", new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("xp.Facets.color", "blue")
                });
                yield return new TestCaseData("xp.Facets.color=blue&xp.Facets.color=red", new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("xp.Facets.color", "blue"),
                    new Tuple<string, string>("xp.Facets.color", "red")
                });
                yield return new TestCaseData("xp.Facets.color=blue&xp.Facets.supplier=fast", new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("xp.Facets.color", "blue"),
                    new Tuple<string, string>("xp.Facets.supplier", "fast")
                });
            }
        }
    }
}
