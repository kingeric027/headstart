using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common;
using Marketplace.Common.Commands.SupplierSync;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using NSubstitute;
using NUnit.Framework;
using ordercloud.integrations.library;

namespace Orchestration.Tests
{
    public class TemplateTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("TemplateSheets.xlsx")]
        [TestCase("TemplateExcel.xlsx")]
        public async Task Test(string fileName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Orchestration.Tests.TemplateTests.{fileName}");
            var file = Substitute.For<IFormFile>();
            file.OpenReadStream().Returns(stream);
            var command = new SupplierSyncCommand(Substitute.For<AppSettings>());
            var parsed = await command.ParseProductTemplate(file, new VerifiedUserContext(new ClaimsPrincipal()));
            Assert.IsTrue(parsed.Count == 1); // total product count
            Assert.IsTrue(parsed.FirstOrDefault()?.Product.ID == "example_id");
            // price schedules test
            Assert.IsTrue(parsed.FirstOrDefault()?.PriceSchedule.ID == "example_ps_id");
            Assert.IsTrue(parsed.FirstOrDefault()?.PriceSchedule.ProductID == "example_id");
            // specs test
            Assert.IsTrue(parsed.FirstOrDefault()?.Specs.Count == 2);
            Assert.IsTrue(parsed.FirstOrDefault()?.Specs.All(s => s.ProductID == "example_id"));
            // spec options test
            var spec = parsed.FirstOrDefault()?.Specs.FirstOrDefault(s => s.ID == "example_spec_id_color");
            Assert.IsTrue(spec?.SpecOptions.Count == 2);
            // images test
            Assert.IsTrue(parsed.FirstOrDefault()?.Images.Count == 1);
            Assert.IsTrue(parsed.FirstOrDefault()?.Images.FirstOrDefault()?.ProductID == "example_id");
            // attachments test
            Assert.IsTrue(parsed.FirstOrDefault()?.Attachments.Count == 1);
            Assert.IsTrue(parsed.FirstOrDefault()?.Attachments.FirstOrDefault()?.ProductID == "example_id");
        }

    }
}
