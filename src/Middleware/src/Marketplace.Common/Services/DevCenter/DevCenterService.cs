using System;
using System.Threading.Tasks;
using Flurl.Http;
using Marketplace.Common.Services.DevCenter.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.DevCenter
{
    public interface IDevCenterService
    {
        Task<string> GetOrgToken(string orgID, string token);
        Task<DevCenterUser> GetMe(string token);
        Task CreateOrganization(Organization org, string token);
        Task<Organization> GetOrganization(string orgID, string token);
    }

    public class DevCenterService : IDevCenterService
    {
        private readonly IFlurlClient _client;
        private readonly AppSettings _settings;

        public DevCenterService(AppSettings settings)
        {
            _settings = settings;
            _client = new FlurlClient($"{settings.OrderCloudSettings.DevcenterApiUrl}/api/v1/");
        }

        public async Task<DevCenterUser> GetMe(string token)
        {
            return await _client.Request("me")
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<DevCenterUser>();
        }

        public async Task<Organization> GetOrganization(string orgID, string token)
        {
            return await _client.Request("organizations", orgID)
                        .WithOAuthBearerToken(token)
                        .GetJsonAsync<Organization>();
        }

        // The devcenter API allows you to get an admin token for that org that isn't related to any user
        // and the roles granted are roles defined for the dev user. If you're the owner, that is full access
        public async Task<string> GetOrgToken(string orgID, string token)
        {
            var request = await _client.Request("organizations", orgID, "token")
                            .WithOAuthBearerToken(token)
                            .GetJsonAsync<OrgTokenResponse>();

            return request.access_token;
        }

        public async Task CreateOrganization(Organization org, string token)
        {
            // doesn't return anything
            await _client.Request("organizations", token)
                .PutJsonAsync(org);
        }
    }
}
