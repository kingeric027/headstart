﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace ordercloud.integrations.exchangerates
{
    public interface IExchangeRatesCommand
    {
        Task<ListPage<OrderCloudIntegrationsConversionRate>> Get(ListArgs<OrderCloudIntegrationsConversionRate> rateArgs, CurrencySymbol currency);
        Task<OrderCloudIntegrationsExchangeRate> Get(CurrencySymbol symbol);
        Task<ListPage<OrderCloudIntegrationsConversionRate>> GetRateList();

        ListPage<OrderCloudIntegrationsConversionRate> Filter(ListArgs<OrderCloudIntegrationsConversionRate> rateArgs,
            OrderCloudIntegrationsExchangeRate rates);

        Task<double?> ConvertCurrency(CurrencySymbol from, CurrencySymbol to, double value);
    }

    public class ExchangeRatesCommand : IExchangeRatesCommand
    {
        private readonly IOrderCloudIntegrationsExchangeRatesClient _client;
        private readonly IOrderCloudIntegrationsBlobService _blob;

        public ExchangeRatesCommand() : this(new BlobServiceConfig())
        {
            _client = new OrderCloudIntegrationsExchangeRatesClient();
        }

        public ExchangeRatesCommand(BlobServiceConfig config)
        {
            _client = new OrderCloudIntegrationsExchangeRatesClient();
            _blob = new OrderCloudIntegrationsBlobService(config);
        }

        public ExchangeRatesCommand(IOrderCloudIntegrationsBlobService blob)
        {
            _client = new OrderCloudIntegrationsExchangeRatesClient();
            _blob = blob;
        }

        /// <summary>
        /// Intended for public API based consumption. Hence the ListArgs implementation
        /// </summary>
        /// <param name="rateArgs"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public async Task<ListPage<OrderCloudIntegrationsConversionRate>> Get(ListArgs<OrderCloudIntegrationsConversionRate> rateArgs, CurrencySymbol currency)
        {
            var rates = await _blob.Get<OrderCloudIntegrationsExchangeRate>($"{currency}.json");
            return Filter(rateArgs, rates);
        }

        public ListPage<OrderCloudIntegrationsConversionRate> Filter(ListArgs<OrderCloudIntegrationsConversionRate> rateArgs, OrderCloudIntegrationsExchangeRate rates)
        {
            if (rateArgs.Filters.Any(filter => filter.Name == "Symbol"))
            {
                rates.Rates = (
                        from rate in rates.Rates
                        from s in rateArgs.Filters.FirstOrDefault(r => r.Name == "Symbol")?.Values
                        where rate.Currency == s.Term.To<CurrencySymbol>()
                        select rate).ToList();
            }

            var list = new ListPage<OrderCloudIntegrationsConversionRate>()
            {
                Meta = new ListPageMeta()
                {
                    Page = 1,
                    PageSize = 1,
                    TotalCount = rates.Rates.Count,
                    ItemRange = new[] { 1, rates.Rates.Count }
                },
                Items = rates.Rates
            };

            return list;
        }

        public async Task<double?> ConvertCurrency(CurrencySymbol from, CurrencySymbol to, double value)
        {
            var rates = await this.Get(new ListArgs<OrderCloudIntegrationsConversionRate>(), from);
            var rate = rates.Items.FirstOrDefault(r => r.Currency == to)?.Rate;
			return value * rate;
        }

        /// <summary>
        /// Intended for private consumption by functions that update the cached resources.
        /// </summary>
        /// <param name="rateArgs"></param>
        /// <returns></returns>
        public async Task<OrderCloudIntegrationsExchangeRate> Get(CurrencySymbol symbol)
        {
            var rates = await _client.Get(symbol);
            return new OrderCloudIntegrationsExchangeRate()
            {
                BaseSymbol = symbol,
                Rates = MapRates(rates.rates)
            };
        }

        public async Task<ListPage<OrderCloudIntegrationsConversionRate>> GetRateList()
        {
            var rates = MapRates();
            return await Task.FromResult(new ListPage<OrderCloudIntegrationsConversionRate>()
            {
                Meta = new ListPageMeta()
                {
                    Page = 1,
                    PageSize = 1,
                    TotalCount = rates.Count,
                    ItemRange = new[] { 1, rates.Count }
                },
                Items = rates
            });
        }

        private static string GetIcon(CurrencySymbol symbol)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ordercloud.integrations.exchangerates.Icons.{symbol}.gif");
            if (stream == null) return null;
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return $"data:image/jpg;base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        private static List<OrderCloudIntegrationsConversionRate> MapRates(ExchangeRatesValues ratesValues = null)
        {
            return Enum.GetValues(typeof(CurrencySymbol)).Cast<CurrencySymbol>().Select(e => new OrderCloudIntegrationsConversionRate()
            {
                Currency = e,
                Icon = GetIcon(e),
                Symbol = SymbolLookup.CurrencySymbolLookup.FirstOrDefault(s => s.Key == e).Value.Symbol,
                Name = SymbolLookup.CurrencySymbolLookup.FirstOrDefault(s => s.Key == e).Value.Name,
                Rate = FixRate(ratesValues, e)
            }).ToList();
        }

        private static double? FixRate(ExchangeRatesValues values, CurrencySymbol e)
        {
            var t = values?.GetType().GetProperty($"{e}")?.GetValue(values, null).To<double?>();
            if (!t.HasValue)
                return 1;
            return t.Value == 0 ? 1 : t.Value;
        }
    }
}
