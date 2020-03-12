using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketplace.Models.Misc;

namespace Marketplace.Common.Mappers.Avalara
{
    public static class TaxCodeMapper
    {
        public static ListPage<MarketplaceTaxCode> Map(FetchResult<TaxCodeModel> codes, TaxCodeListArgs args)
        {
			var items = codes.value.Select(code => new MarketplaceTaxCode 
			{
				Category = args.CodeCategory,
				Code = code.taxCode,
				Description = code.description
			}).ToList();
			var listPage = new ListPage<MarketplaceTaxCode>
			{
				Items = items,
				Meta = new ListPageMeta
				{
					Page = (int)Math.Ceiling((double)args.Skip / args.Top) + 1,
					PageSize = 100,
					TotalCount = codes.count,
				}
			};
			return listPage;
		}

		public static TaxCodeListArgs Map(ListArgs<TaxCodeModel> source)
		{
			var taxCategory = source?.Filters?[0]?.Values?[0]?.Term ?? "";
			var taxCategorySearch = taxCategory.Trim('0');
			var search = source.Search;
			var filter = search != "" ? $"isActive eq true and taxCode startsWith '{taxCategorySearch}' and (taxCode contains '{search}' OR description contains '{search}')" : $"isActive eq true and taxCode startsWith '{taxCategorySearch}'";
			return new TaxCodeListArgs()
			{
				Filter = filter,
				Top = source.PageSize,
				Skip = (source.Page - 1) * source.PageSize,
				CodeCategory = taxCategory,
				OrderBy = null
			};
        }
    }
}
