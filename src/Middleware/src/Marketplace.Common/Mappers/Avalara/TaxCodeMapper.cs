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
        public static List<MarketplaceTaxCode> Map(FetchResult<TaxCodeModel> taxCodeList)
        {
            return taxCodeList.value
                .Select(taxCode => new MarketplaceTaxCode
                {
                    TaxCode = taxCode.taxCode,
                    Description = taxCode.description
                }).ToList();
        }

        public static MarketplaceListPage<MarketplaceTaxCode> Map(FetchResult<TaxCodeModel> avataxCodes, List<MarketplaceTaxCode> taxCodeList, int top, int skip)
        {
            return new MarketplaceListPage<MarketplaceTaxCode>
            {
                Meta = new MarketplaceListPageMeta
                {
                    Page = skip / top == 0 ? 1 : skip / top,
                    PageSize = 100,
                    TotalCount = avataxCodes.count,
                },
                Items = taxCodeList
            };
        }
    }
}
