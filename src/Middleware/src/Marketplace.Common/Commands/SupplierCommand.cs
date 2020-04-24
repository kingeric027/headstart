using Marketplace.Helpers;
using Marketplace.Common.TemporaryAppConstants;
using Marketplace.Helpers.Models;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceSupplierCommand
    {
        Task<MarketplaceSupplier> Create(MarketplaceSupplier supplier, VerifiedUserContext user, string token);
        Task<MarketplaceSupplier> GetMySupplier(string supplierID, VerifiedUserContext user, string token);
    }
    public class MarketplaceSupplierCommand : IMarketplaceSupplierCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;

        public MarketplaceSupplierCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }
        public async Task<MarketplaceSupplier> GetMySupplier(string supplierID, VerifiedUserContext user, string token)
        {
            Require.That(supplierID == user.SupplierID,
                new ErrorCode("Unauthorized", 401, $"You are only authorized to view {user.SupplierID}."));
            return await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierID, token);
        }
        public async Task<MarketplaceSupplier> Create(MarketplaceSupplier supplier, VerifiedUserContext user, string token)
        {
            // Create Supplier
            supplier.ID = "{supplierIncrementor}";
            var ocSupplier = await _oc.Suppliers.CreateAsync(supplier, token);
            var ocSupplierID = ocSupplier.ID;
     
            // Create Integrations Supplier User
            var supplierUser = await _oc.SupplierUsers.CreateAsync(ocSupplierID, new User()
            {
                Active = true,
                Email = user.Email,
                FirstName = "Integration",
                LastName = "Developer",
                Password = "Four51Yet!", // _settings.OrderCloudSettings.DefaultPassword,
                Username = $"dev_{ocSupplierID}"
            }, token);

            await CreateUserTypeUserGroupsAndSecurityProfileAssignments(supplierUser, token, ocSupplierID);

            // Create API Client for new supplier
            var apiClient = await _oc.ApiClients.CreateAsync(new ApiClient()
            {
                AppName = $"Integration Client {ocSupplier.Name}",
                Active = true,
                DefaultContextUserName = supplierUser.Username,
                ClientSecret = _settings.OrderCloudSettings.ClientSecret,
                AccessTokenDuration = 600,
                RefreshTokenDuration = 43200,
                AllowAnyBuyer = false,
                AllowAnySupplier = false,
                AllowSeller = false,
                IsAnonBuyer = false,
            }, token);
            // Assign Supplier API Client to new supplier
            await _oc.ApiClients.SaveAssignmentAsync(new ApiClientAssignment()
            {
                ApiClientID = apiClient.ID,
                SupplierID = ocSupplierID
            }, token);
           
            return supplier;
        }
    
        public async Task CreateUserTypeUserGroupsAndSecurityProfileAssignments(User user, string token, string supplierID)
        {
            foreach(var userType in SEBUserTypes.Supplier())
            {
                var userGroupID = $"{supplierID}{userType.UserGroupIDSuffix}";

                await _oc.SupplierUserGroups.CreateAsync(supplierID, new UserGroup()
                {
                    ID = userGroupID,
                    Name = userType.UserGroupName,
                    xp =
                        {
                            Type = "UserPermissions",
                        }
                }, token);
 
                await _oc.SupplierUserGroups.SaveUserAssignmentAsync(supplierID, new UserGroupAssignment()
                {
                    UserID = user.ID,
                    UserGroupID = userGroupID
                }, token);

                foreach (var customRole in userType.CustomRoles)
                {
                    await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
                    {
                        SupplierID = supplierID,
                        UserGroupID = userGroupID,
                        SecurityProfileID = customRole.ToString()
                    }, token);
                }
            }
        }
    }
}
