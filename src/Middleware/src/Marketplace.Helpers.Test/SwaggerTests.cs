using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Common.Models;
using Marketplace.Helpers.Extensions;
using Marketplace.Models;
using NUnit.Framework;

namespace Marketplace.Helpers.Tests
{
    public class SwaggerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void is_model_type_true()
        {
            Assert.IsTrue(typeof(OrchestrationLog).IsModelType());
            Assert.IsTrue(typeof(MarketplaceCatalog).IsModelType());
            Assert.IsFalse(typeof(ListArgs<OrchestrationLog>).IsModelType());
            Assert.IsTrue(typeof(OrderCloud.SDK.Catalog).IsModelType());
        }
    }
}
