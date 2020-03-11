using Cosmonaut.Response;
using Marketplace.Helpers.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Marketplace.Helpers.Tests
{
	public class CosmosTests
	{
		[Test]
		[TestCase(1, 20, 100, 5, new[] { 1, 20 })]
		[TestCase(2, 20, 100, 5, new[] { 21, 40 })]
		[TestCase(1, 1, 100, 100, new[] { 1, 1 })]
		[TestCase(4, 12, 345, 29, new[] { 37, 48 })]
		public void to_list_page_test(int page, int pageSize, int count, int totalPages, int[] itemRange)
		{
			CosmosPagedResults<object> mock = (CosmosPagedResults<object>)FormatterServices.GetUninitializedObject(typeof(CosmosPagedResults<object>)); //does not call ctor
			var result = CosmosExtensions.ToListPage<object>(mock, page, pageSize, count);
			Assert.AreEqual(page, result.Meta.Page);
			Assert.AreEqual(pageSize, result.Meta.PageSize);
			Assert.AreEqual(count, result.Meta.TotalCount);
			Assert.AreEqual(totalPages, result.Meta.TotalPages);
			Assert.AreEqual(itemRange, result.Meta.ItemRange);
		}
	}
}
