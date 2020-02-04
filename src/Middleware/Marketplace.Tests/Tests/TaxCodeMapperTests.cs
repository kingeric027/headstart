using Marketplace.Common.Mappers.Avalara;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Tests
{
    class TaxCodeMapperTests
    {
        [Test]
        public void calc_top_skip_page1()
        {
            var result = TaxCodeMapper.Map(1, 100);
            Assert.AreEqual((100, 0), result);
        }
        [Test]
        public void calc_top_skip_page5()
        {
            var result = TaxCodeMapper.Map(5, 100);
            Assert.AreEqual((100, 400), result);
        }
    }
}
