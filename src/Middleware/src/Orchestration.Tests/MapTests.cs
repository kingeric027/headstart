using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Helpers.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Orchestration.Tests
{
    public class MapTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void map_test()
        {
            //var spec = new MarketplaceSpec()
            //{
            //    AllowOpenText = true,
            //    DefaultOptionID = "oid",
            //    DefaultValue = "value",
            //    DefinesVariant = false,
            //    ID = "ID",
            //    ListOrder = 1,
            //    Name = "test_spec",
            //    Required = true,
            //    UI = new SpecUI()
            //    {
            //        ControlType = ControlType.Text
            //    }
            //};
            //var mapped = SpecMapper.Map(spec);
            //Assert.AreEqual(ControlType.Text, mapped.xp.UI.ControlType);
        }
    }
}
