using System.Threading.Tasks;
using Flurl.Http;

namespace ordercloud.integrations.exchangerates
{
    /// <summary>
    /// Rates supported by https://exchangeratesapi.io/
    /// </summary>
    public interface IOrderCloudIntegrationsExchangeRatesClient
    {
        Task<ExchangeRatesBase> Get(CurrencySymbols symbol);
    }

    public class OrderCloudIntegrationsExchangeRatesClient: IOrderCloudIntegrationsExchangeRatesClient
    {
        private readonly IFlurlClient _flurl;

        public OrderCloudIntegrationsExchangeRatesClient()
        {
            _flurl = new FlurlClient
            {
                BaseUrl = $"https://api.exchangeratesapi.io/"
            };
        }

        private IFlurlRequest Request(string resource)
        {
            return _flurl.Request(resource);
        }
       
        public async Task<ExchangeRatesBase> Get(CurrencySymbols symbol)
        {
            return await this.Request("latest")
                .SetQueryParam("base", symbol)
                .GetJsonAsync<ExchangeRatesBase>();
        }
    }
}
