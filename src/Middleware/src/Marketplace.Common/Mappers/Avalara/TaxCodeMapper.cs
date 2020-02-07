using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.AvaTax.Models;
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

        public static MarketplaceListPage<MarketplaceTaxCode> Map(FetchResult<TaxCodeModel> avataxCodes, List<MarketplaceTaxCode> taxCodeList, int top, int skip)
        {
            return new MarketplaceListPage<MarketplaceTaxCode>
            {
                Meta = new MarketplaceListPageMeta
                {
                    Page = skip / top == 0 ? 1 : (skip / top) + 1,
                    PageSize = top,
                    TotalCount = avataxCodes.count,
                },
                Items = taxCodeList
            };
        }

        public static (int, int) Map(int page, int pageSize)
        {
            var top = pageSize;
            var skip =  (page - 1) * pageSize;
            return (top, skip);
        }
    }
}
