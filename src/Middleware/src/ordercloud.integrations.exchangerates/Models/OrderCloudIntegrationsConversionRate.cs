using ordercloud.integrations.library;

namespace ordercloud.integrations.exchangerates
{
    [SwaggerModel]
    public class OrderCloudIntegrationsConversionRate
    {
        public CurrencySymbols Currency { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public double? Rate { get; set; }
        public string Icon { get; set; }
    }
}
