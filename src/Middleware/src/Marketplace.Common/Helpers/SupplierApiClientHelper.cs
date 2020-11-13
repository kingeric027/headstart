using Marketplace.Common.Extensions;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Helpers
{
    public interface ISupplierApiClientHelper
    {
        Task<ApiClient> GetSupplierApiClient(string supplierID, VerifiedUserContext user);
    }
    public class SupplierApiClientHelper : ISupplierApiClientHelper
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        public SupplierApiClientHelper(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }
        public async Task<ApiClient> GetSupplierApiClient(string supplierID, VerifiedUserContext user)
        {
            Supplier supplierDetails = await _oc.Suppliers.GetAsync(supplierID);
            // GET ApiClient using the xp value on supplier
            try
            {
                ApiClient apiClient = await _oc.ApiClients.GetAsync(supplierDetails?.xp?.ApiClientID);
                // If ApiClient exists, return it
                if (apiClient.ID == null)
                {
                    // in some cases, a null xp value was returning a 'blank' api client in the get request
                    return await HandleError(supplierDetails, user);
                }
                return apiClient;
            }
            catch
            {
                // else create and return the new api client
                return await HandleError(supplierDetails, user);
            }

        }
        public async Task<ApiClient> HandleError(Supplier supplierDetails, VerifiedUserContext user)
        {
            var supplierClient = await _oc.ApiClients.CreateAsync(new ApiClient()
            {
                AppName = $"Integration Client {supplierDetails.Name}",
                Active = true,
                DefaultContextUserName = $"dev_{supplierDetails.ID}",
                ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                AccessTokenDuration = 600,
                RefreshTokenDuration = 43200,
                AllowAnyBuyer = false,
                AllowAnySupplier = false,
                AllowSeller = false,
                IsAnonBuyer = false,
            }, user.AccessToken);
            // Assign Supplier API Client to new supplier
            await _oc.ApiClients.SaveAssignmentAsync(new ApiClientAssignment()
            {
                ApiClientID = supplierClient.ID,
                SupplierID = supplierDetails.ID
            }, user.AccessToken);
            // Update supplierXp to contain the new api client value
            await _oc.Suppliers.PatchAsync(supplierDetails.ID, new PartialSupplier { xp = new { ApiClientID = supplierClient.ID } });
            return supplierClient;
        }
    }
}
