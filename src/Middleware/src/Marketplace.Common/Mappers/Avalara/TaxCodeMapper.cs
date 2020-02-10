using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.AvaTax.Models;
using System.Collections.Generic;
using System.Linq;
using OrderCloud.SDK;

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

        public static ListPage<MarketplaceTaxCode> Map(FetchResult<TaxCodeModel> avataxCodes, List<MarketplaceTaxCode> taxCodeList, int top, int skip)
        {
            return new ListPage<MarketplaceTaxCode>
            {
                Meta = new ListPageMeta
                {
                    Page = skip / top == 0 ? 1 : (skip / top) + 1,
                    PageSize = 100,
                    TotalCount = avataxCodes.count,
                },
                Items = taxCodeList
            };
        }

        public static (int, int) Map(int page, int pageSize)
        {
            var top = pageSize;
            var skip =  page > 1 ? (page - 1) * pageSize : 0;
            return (top, skip);
        }
    }
}
