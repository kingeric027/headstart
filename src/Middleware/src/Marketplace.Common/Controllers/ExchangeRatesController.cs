using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Integration\" represents Currency Conversion Charts")]
    [MarketplaceSection.Integration(ListOrder = 4)]
    [Route("exchangerates")]
    public class ExchangeRatesController : BaseController
    {
        private readonly IExchangeRatesCommand _command;

        public ExchangeRatesController(AppSettings settings, IExchangeRatesCommand command) : base(settings)
        {
            _command = command;
        }

        [HttpGet, Route("{currency}")]
        public async Task<ListPage<OrderCloudIntegrationsConversionRate>> Get(ListArgs<OrderCloudIntegrationsConversionRate> rateArgs, CurrencySymbol currency)
        {
            return await _command.Get(rateArgs, currency);
        }

        [HttpGet, Route("supportedrates")]
        public async Task<ListPage<OrderCloudIntegrationsConversionRate>> GetRateList()
        {
            var list = await _command.GetRateList();
            return list;
        }
    }
}
