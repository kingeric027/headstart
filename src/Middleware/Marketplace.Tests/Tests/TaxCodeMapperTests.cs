
using Marketplace.Common.Mappers.Avalara;
using Marketplace.Common.Services.AvaTax.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using Orchestration.Tests.Mocks;
using OrderCloud.SDK;

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
        [Test]
        public void map_to_marketplace_list_page_page1()
        {
            var avalaraTaxCodesFromApiCall = TaxCodes.taxCodeObjectFromAvalaraFirstRecord();
            var marketplaceTaxCodesListFromMapper = TaxCodes.marketplaceTaxCodeListPageFirstRecord();
            var result = TaxCodeMapper.Map(avalaraTaxCodesFromApiCall, marketplaceTaxCodesListFromMapper, 1, 0);
            var expectedResult = new ListPage<MarketplaceTaxCode>
            {
                Meta = new ListPageMeta
                {
                    Page = 1,
                    PageSize = 1,
                    TotalCount = 1,
                },
                Items = marketplaceTaxCodesListFromMapper
            };
            var serializedExpectedResult = JsonConvert.SerializeObject(expectedResult);
            var serializedResult = JsonConvert.SerializeObject(result);
            Assert.AreEqual(serializedExpectedResult, serializedResult);
        }
        [Test]
        public void map_to_marketplace_list_page_page2()
        {
            var avalaraTaxCodesFromApiCall = TaxCodes.taxCodeObjectFromAvalaraSecondRecord();
            var marketplaceTaxCodesListFromMapper = TaxCodes.marketplaceTaxCodeListPageSecondRecord();
            var result = TaxCodeMapper.Map(avalaraTaxCodesFromApiCall, marketplaceTaxCodesListFromMapper, 1, 1);
            var expectedResult = new ListPage<MarketplaceTaxCode>
            {
                Meta = new ListPageMeta
                {
                    Page = 2,
                    PageSize = 1,
                    TotalCount = 1,
                },
                Items = marketplaceTaxCodesListFromMapper
            };
            var serializedExpectedResult = JsonConvert.SerializeObject(expectedResult);
            var serializedResult = JsonConvert.SerializeObject(result);
            Assert.AreEqual(serializedExpectedResult, serializedResult);
        }
        [Test]
        public void map_to_marketplace_list_page_all_records()
        {
            var avalaraTaxCodesFromApiCall = TaxCodes.taxCodeObjectFromAvalaraAllRecords();
            var marketplaceTaxCodesListFromMapper = TaxCodes.marketplaceTaxCodeListPageAllRecords();
            var result = TaxCodeMapper.Map(avalaraTaxCodesFromApiCall, marketplaceTaxCodesListFromMapper, 100, 0);
            var expectedResult = new ListPage<MarketplaceTaxCode>
            {
                Meta = new ListPageMeta
                {
                    Page = 1,
                    PageSize = 100,
                    TotalCount = 2,
                },
                Items = marketplaceTaxCodesListFromMapper
            };
            var serializedExpectedResult = JsonConvert.SerializeObject(expectedResult);
            var serializedResult = JsonConvert.SerializeObject(result);
            Assert.AreEqual(serializedExpectedResult, serializedResult);
        }
    }
}
