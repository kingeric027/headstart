﻿using Marketplace.Models;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services
{
    public interface ISebExchangeRatesService
    {
        Task<CurrencySymbol> GetCurrencyForUser(string userToken);
        Task<List<OrderCloudIntegrationsConversionRate>> GetExchangeRatesForUser(string userToken);
    }

    public class SebExchangeRatesService : ISebExchangeRatesService
    {
        private readonly IOrderCloudClient _oc;
        private readonly IExchangeRatesCommand _exchangeRatesCommand;
        public SebExchangeRatesService(IOrderCloudClient oc, IExchangeRatesCommand exchangeRatesCommand)
        {
            _oc = oc;
            _exchangeRatesCommand = exchangeRatesCommand;
        }

        public async Task<CurrencySymbol> GetCurrencyForUser(string userToken)
        {
            var buyerUserGroups = await _oc.Me.ListUserGroupsAsync<MarketplaceLocationUserGroup>(opts => opts.AddFilter(u => u.xp.Type == "BuyerLocation"), userToken);
            var currency = buyerUserGroups.Items.FirstOrDefault(u => u.xp.Currency != null)?.xp?.Currency;
            Require.That(currency != null, new ErrorCode("Exchange Rate Error", 400, "Exchange Rate Not Defined For User"));
            return (CurrencySymbol)currency;
        }

        public async Task<List<OrderCloudIntegrationsConversionRate>> GetExchangeRatesForUser(string userToken)
        {
            var currency = await GetCurrencyForUser(userToken);
            var exchangeRates = await _exchangeRatesCommand.Get(new ListArgs<OrderCloudIntegrationsConversionRate>() { }, currency);
            return exchangeRates.Items.ToList();
        }
    }
}