using Marketplace.Common.Commands;
using Marketplace.Common.Services;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.AvaTax;

namespace Marketplace.Common.Controllers.Avalara
{
    [Route("taxcodes")]
    public class TaxCodesController : BaseController
    {
        private readonly IAvataxService _taxService;
        private readonly ITaxCommand _taxCommand;

        public TaxCodesController(AppSettings settings, IAvataxService taxService, ITaxCommand taxCommand) : base(settings)
        {
            _taxService = taxService;
            _taxCommand = taxCommand;
        }

        [HttpGet, Route(""), MarketplaceUserAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceListPage<MarketplaceTaxCode>> GetTaxCodes(MarketplaceListArgs<TaxCodeModel> marketplaceListArgs)
        {
            return await _taxService.ListTaxCodesAsync(marketplaceListArgs);
        }
    }
}
