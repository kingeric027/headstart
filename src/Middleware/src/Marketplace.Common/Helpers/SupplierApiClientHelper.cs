using Marketplace.Common.Extensions;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Helpers
{
    public class SupplierApiClientHelper
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        public SupplierApiClientHelper(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }
        public async Task<ApiClient> GetOrCreateSupplierApiClientByName(string supplierID, VerifiedUserContext user)
        {
            ApiClient supplierClient;
            var supplierDetails = await _oc.Suppliers.GetAsync(supplierID);
            // List API Clients and find one with supplier name 
            var apiClients = await _oc.ApiClients.ListAsync(supplierDetails.Name);
            if (apiClients.Items.HasItem())
            {
                // If ApiClient exists, return it
                supplierClient = apiClients?.Items?[0];
            } else
            {
                // else create and return the new api client
                supplierClient = await _oc.ApiClients.CreateAsync(new ApiClient()
                {
                    AppName = $"Integration Client {supplierDetails.Name}",
                    Active = true,
                    DefaultContextUserName = $"dev_{supplierID}",
                    ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                    AccessTokenDuration = 600,
                    RefreshTokenDuration = 43200,
                    AllowAnyBuyer = false,
                    AllowAnySupplier = false,
                    AllowSeller = false,
                    IsAnonBuyer = false,
                }, user.AccessToken);
            }
            return supplierClient;
        }
    }
}
