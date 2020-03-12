using Marketplace.Common.Commands;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.Avalara;
using Marketplace.Common.Services.AvaTax;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;

namespace Marketplace.Common.Controllers.Avalara
{
    [DocComments("\"Integration\" represents Tax Codes for Marketplace")]
    [MarketplaceSection.Integration(ListOrder = 1)]
    [Route("taxcodes")]
    public class TaxCodesController : BaseController
    {
        private readonly IAvataxService _taxService;

        public TaxCodesController(AppSettings settings, IAvataxService taxService) : base(settings)
        {
            _taxService = taxService;
        }

        [DocName("LIST Tax Codes")]
        [HttpGet, Route(""), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<ListPage<MarketplaceTaxCode>> GetTaxCodes(ListArgs<TaxCodeModel> marketplaceListArgs)
        {
            return await _taxService.ListTaxCodesAsync(marketplaceListArgs);
        }
    }
}
