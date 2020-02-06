using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marketplace.Common.Mappers.Avalara
{
    public static class TaxCodeMapper
    {
        public static List<MarketplaceTaxCode> Map(FetchResult<TaxCodeModel> taxCodeList, string category)
        {
            return taxCodeList.value
                .Select(taxCode => new MarketplaceTaxCode
                {
                    Category = category,
                    Code = taxCode.taxCode,
                    Description = taxCode.description
                }).ToList();
        }

        public static MarketplaceListPage<MarketplaceTaxCode> Map(FetchResult<TaxCodeModel> codes, TaxCodeListArgs args)
        {
			var items = codes.value.Select(code => new MarketplaceTaxCode 
			{
				Category = args.CodeCategory,
				Code = code.taxCode,
				Description = code.description
			}).ToList();
			var listPage = new MarketplaceListPage<MarketplaceTaxCode>
			{
				Items = items,
				Meta = new MarketplaceListPageMeta
				{
					Page = args.Skip / args.Top == 0 ? 1 : (args.Skip / args.Top) + 1,
					PageSize = 100,
					TotalCount = codes.count,
				}
			};
			return listPage;
		}

		public static TaxCodeListArgs Map(MarketplaceListArgs<TaxCodeModel> source)
		{
			var taxCategory = source.Filters[0].Values[0].Term;
			var taxCategorySearch = taxCategory.Trim('0');
			var search = source.Search;
			var filter = search != "" ? $"isActive eq true and taxCode startsWith '{taxCategorySearch}' and (taxCode contains '{search}' OR description contains '{search}')" : $"isActive eq true and taxCode startsWith '{taxCategorySearch}'";
			return new TaxCodeListArgs()
			{
				Filter = filter,
				Top = source.PageSize,
				Skip = source.Page > 1 ? (source.Page - 1) * source.PageSize : 0,
				CodeCategory = taxCategory,
				OrderBy = null
			};
        }
    }
}
