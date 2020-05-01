﻿
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.Avalara.Mappers;
using Marketplace.Helpers;
using Marketplace.Models.Misc;
using Newtonsoft.Json;
using NUnit.Framework;
using Orchestration.Tests.Mocks;
using OrderCloud.SDK;
using System.Collections;

namespace Marketplace.Tests
{
    public class TaxCodeMapperTests
    {
        [Test]
		[TestCase(5, 100, 100, 400)]
		[TestCase(1, 100, 100, 0)]
		public void map_oc_list_args_to_avalara(int page, int pageSize, int expectedTop, int expectedSkip)
        {
			var result = AvalaraMapper.Map(new ListArgs<TaxCode>() { Filters = null, Page = page, PageSize = pageSize });
			Assert.AreEqual(expectedTop, result.Top);
			Assert.AreEqual(expectedSkip, result.Skip);
		}

		[Test]
		[TestCase(0, 1, 1)]
		[TestCase(1, 1, 2)]
		[TestCase(100, 1, 101)]
		public void map_avalara_list_args_to_oc(int skip, int top, int page)
		{
			var avalaraTaxCodesFromApiCall = TaxCodes.taxCodeObjectFromAvalaraFirstRecord();
			var args = new TaxCodeListArgs() { Skip = skip, Top = top };
			var result = AvalaraMapper.Map(avalaraTaxCodesFromApiCall, args);
			var expectedMeta = new ListPageMeta
			{
				Page = page,
				PageSize = 100,
				TotalCount = 1,
			};
			Assert.AreEqual(expectedMeta.Page, result.Meta.Page);
			Assert.AreEqual(expectedMeta.PageSize, result.Meta.PageSize);
			Assert.AreEqual(expectedMeta.TotalCount, result.Meta.TotalCount);
		}
	}
}
