using System.Threading.Tasks;
using Flurl.Http;
using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services
{

    /// <summary>
    /// this is a temporary service that should be removed when shipping integration is in production order cloud and this is available in the sdk more easily
    /// </summary>
    public interface IOrderCloudSandboxService
    {
        Task<OrderWorksheet> GetOrderWorksheetAsync(OrderDirection orderDirection, string orderID);
        Task CreateIntegrationEvent(IntegrationEvent integrationEvent, string token = "");
    }
    public class OrderCloudSandboxService : IOrderCloudSandboxService
    {
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;
		private readonly IFlurlClient _flurl;
		private string accessToken;
		public OrderCloudSandboxService(AppSettings settings, IOrderCloudClient ocClient, IFlurlClient flurl)
        {
			_flurl = flurl;
			_oc = ocClient;
            _settings = settings;
        }
		private async Task AuthenticateAync()
		{
            var sdk = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                AuthUrl = _settings.OrderCloudSettings.AuthUrl,
                ClientId = _settings.OrderCloudSettings.ClientID,
                ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                Roles = new[]
                   {
                        ApiRole.FullAccess
                    }
            });
            var tokenResponse = await sdk.AuthenticateAsync();
            accessToken = tokenResponse.AccessToken;
        }


		public async Task<MarketplaceOrderWorksheet> GetOrderWorksheetAsync(OrderDirection orderDirection, string orderID)
		{
			await AuthenticateAync();
            return await _flurl.Request($"{_settings.OrderCloudSettings.ApiUrl}/v1/orders/{orderDirection}/{orderID}/worksheet")
                .WithHeader("Authorization", $"Bearer {accessToken}").GetAsync().ReceiveJson<MarketplaceOrderWorksheet>();
		}
    }
}