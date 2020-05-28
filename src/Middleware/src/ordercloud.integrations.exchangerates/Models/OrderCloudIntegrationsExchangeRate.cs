using System.Collections.Generic;

namespace ordercloud.integrations.exchangerates
{
    public class OrderCloudIntegrationsExchangeRate
    {
        public CurrencySymbols BaseSymbol { get; set; }
        public List<OrderCloudIntegrationsConversionRate> Rates { get; set; }
    }
}
