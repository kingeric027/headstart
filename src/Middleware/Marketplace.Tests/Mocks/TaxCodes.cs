using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orchestration.Tests.Mocks
{
    class TaxCodes
    {
        public static FetchResult<TaxCodeModel> taxCodeObjectFromAvalara()
        {
            return new FetchResult<TaxCodeModel>()
            {
                count = 1,
                value = new List<TaxCodeModel>()
                {
                    new TaxCodeModel
                   {
                    id = 9934,
                    companyId = 1,
                    taxCode = "PP030107",
                    taxCodeTypeId = "P",
                    description = "Paper Products-Paper bags",
                    parentTaxCode = "PP030100",
                    isPhysical = true,
                    goodsServiceCode = 0,
                    isActive = true,
                    isSSTCertified = true,
                    createdDate = DateTime.Parse("2006-01-24T04=59=48.27"),
                    createdUserId = 0,
                    modifiedDate = DateTime.Parse("2013-03-27T22=54=25.363"),
                    modifiedUserId = 0
                    }
                }
            };
        }
         public static MarketplaceListPage<MarketplaceTaxCode> marketplaceTaxCodeListPage()
        {
            return new MarketplaceListPage<MarketplaceTaxCode>
            {
                Meta = new MarketplaceListPageMeta()
                {
                    Page = 1,
                    PageSize = 100,
                    TotalCount = 1,
                },
                Items = new List<MarketplaceTaxCode>
                {
                    new MarketplaceTaxCode()
                    {
                        Category = "Test-Category",
                        Code = "Test-Tax-Code",
                        Description = "Test Tax Code Description"
                    }
                }
            };
        } 
    }
}