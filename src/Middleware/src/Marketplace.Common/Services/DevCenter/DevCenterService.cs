using System;
using System.Threading.Tasks;
using Flurl.Http;
using Marketplace.Common.Models;
using Marketplace.Helpers.Models;

namespace Marketplace.Common.Services.DevCenter
{
    public interface IDevCenterService
    {
        Task<MarketplaceListPage<DevAccessibleCompany>> GetOrganizations(int ownerDevId, string token);
        Task<DevCenterUser> GetMe(string token);
        Task<ImpersonationToken> Impersonate(int id, string token);
        Task<AdminCompany> PostOrganization(Organization org, string token);
    }

    public class DevCenterService : IDevCenterService
    {
        private readonly IFlurlClient _client;
        private readonly AppSettings _settings;

        public DevCenterService(AppSettings settings)
        {
            _settings = settings;
            _client = new FlurlClient("https://api.ordercloud.io");
        }

        private IFlurlRequest Get(string resource, string token)
        {
            return _client.Request($"devcenter/{resource}").WithOAuthBearerToken(token);
        }

        private IFlurlRequest Post(string resource, string token)
        {
            return _client.Request($"devcenter/{resource}").WithOAuthBearerToken(token);
        }

        public async Task<DevCenterUser> GetMe(string token)
        {
            var user = await this.Get("me", token).GetJsonAsync<DevCenterUser>();
            return user;
        }

        public async Task<MarketplaceListPage<DevAccessibleCompany>> GetOrganizations(int ownerDevId, string token)
        {
            var orgs = await this.Get("me/DevAccessibleCompanies", token)
                .SetQueryParam("OwnerDevID", ownerDevId)
                .GetJsonAsync<MarketplaceListPage<DevAccessibleCompany>>();
            return orgs;
        }

        public async Task<ImpersonationToken> Impersonate(int id, string token)
        {
            var request = await this.Get($"me/DevAccessibleCompanies/{id}/impersonateToken", token)
                .GetJsonAsync<ImpersonationToken>();
            return request;
        }

        public async Task<AdminCompany> PostOrganization(Organization org, string token)
        {
            var post = await this.Post("adminCompanies", token)
                .PostJsonAsync(org)
                .ReceiveJson<AdminCompany>();
            return post;
        }
    }
}
