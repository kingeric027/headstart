using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using OrderCloud.SDK;

namespace Orchestration.Tests
{
    public class ModelTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ensure_spec_control_type_required()
        {
            var prop = typeof(SpecUI)
                .GetProperty("ControlType")?
                .GetCustomAttributes(typeof(RequiredAttribute), false);
            Assert.IsTrue(prop != null);
        }

        [Test]
        public void ensure_spec_ui_xp_required()
        {
            var prop = typeof(OrchestrationSpecXp)
                .GetProperty("UI")?
                .GetCustomAttributes(typeof(RequiredAttribute), false);
            Assert.IsTrue(prop != null);
        }

        [Test]
        public void ensure_base_orchestration_object_id_required()
        {
            var obj = typeof(IOrchestrationObject);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => obj.IsAssignableFrom(p));
            foreach (var type in types)
            {
                var prop = type.GetProperty("ID")?.GetCustomAttributes(typeof(RequiredAttribute), false);
                Assert.IsTrue(prop != null);
            }
        }
    }
}
