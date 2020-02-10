﻿using Marketplace.Common.Commands;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Services.AvaTax;
using Marketplace.Models;

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
        public async Task<ListPage<MarketplaceTaxCode>> GetTaxCodes(ListArgs<TaxCodeModel> marketplaceListArgs)
        {
            return await _taxService.ListTaxCodesAsync(marketplaceListArgs);
        }
    }
}
