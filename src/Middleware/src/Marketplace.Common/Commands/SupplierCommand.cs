using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using System.Linq;
using Marketplace.Common.Constants;
using ordercloud.integrations.library.helpers;
using Marketplace.Models;
using System;
using System.Dynamic;
using System.Collections.Generic;
using Marketplace.Common.Extensions;
using Marketplace.Common.Helpers;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceSupplierCommand
    {
        Task<MarketplaceSupplier> Create(MarketplaceSupplier supplier, VerifiedUserContext user, bool isSeedingEnvironment = false);
        Task<MarketplaceSupplier> GetMySupplier(string supplierID, VerifiedUserContext user);
        Task<MarketplaceSupplier> UpdateSupplier(string supplierID, PartialSupplier supplier, VerifiedUserContext user);
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
        public async Task<MarketplaceSupplier> GetMySupplier(string supplierID, VerifiedUserContext user)
        {
            Require.That(supplierID == user.SupplierID,
                new ErrorCode("Unauthorized", 401, $"You are only authorized to view {user.SupplierID}."));
            return await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierID);
        }

        public async Task<MarketplaceSupplier> UpdateSupplier(string supplierID, PartialSupplier supplier, VerifiedUserContext user)
        {
            Require.That(user.UsrType == "admin" || supplierID == user.SupplierID, new ErrorCode("Unauthorized", 401, $"You are not authorized to update supplier {supplierID}"));
            var currentSupplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierID);
            var updatedSupplier = await _oc.Suppliers.PatchAsync<MarketplaceSupplier>(supplierID, supplier);
            // Update supplier products only on a name change
            if (currentSupplier.Name != supplier.Name || currentSupplier.xp.Currency.ToString() != supplier.xp.Currency.Value)
            {
                var productsToUpdate = await ListAllAsync.ListWithFacets((page) => _oc.Products.ListAsync<MarketplaceProduct>(
                supplierID: supplierID,
                page: page,
                pageSize: 100,
                accessToken: user.AccessToken
                ));
                // Use supplier integrations client with a DefaultContextUserName to access a supplier token.  
                // All suppliers have integration clients containing their name, get the supplier and use the name to get the clientID
                var supplierDetails = await _oc.Suppliers.GetAsync(supplierID);
                // List API Clients and find one with supplier name 
                var apiClients = await _oc.ApiClients.ListAsync(supplier.Name);
                ApiClient supplierClient = await GetOrCreateSupplierApiClientByName(supplierID, user);
                if (supplierClient == null) { throw new Exception($"Default supplier client not found. SupplierID: {supplierID}"); }
                var configToUse = new OrderCloudClientConfig
                {
                    ApiUrl = user.ApiUrl,
                    AuthUrl = user.AuthUrl,
                    ClientId = supplierClient.ID,
                    ClientSecret = supplierClient.ClientSecret,
                    GrantType = GrantType.ClientCredentials,
                    Roles = new[]
                               {
                                 ApiRole.SupplierAdmin,
                                 ApiRole.ProductAdmin
                            },

                };
                var ocClient = new OrderCloudClient(configToUse);
                await ocClient.AuthenticateAsync();
                var token = ocClient.TokenResponse.AccessToken;
                foreach (var product in productsToUpdate)
                {
                    product.xp.Facets["supplier"] = new List<string>() { supplier.Name };
                    product.xp.Currency = supplier.xp.Currency;
                }
                await Throttler.RunAsync(productsToUpdate, 100, 5, product => ocClient.Products.SaveAsync(product.ID, product, accessToken: token));
            }

            return updatedSupplier;

        }
        public async Task<MarketplaceSupplier> Create(MarketplaceSupplier supplier, VerifiedUserContext user, bool isSeedingEnvironment = false)
        {
            var token = isSeedingEnvironment ? user.AccessToken : null;

            // Create Supplier
            supplier.ID = "{supplierIncrementor}";
            var ocSupplier = await _oc.Suppliers.CreateAsync(supplier, token);
            supplier.ID = ocSupplier.ID;
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


            // not adding api client ID on supplier Create because that approach would require creating the API client first
            // but creating supplier first is preferable in case there are error in the request
            ocSupplier = await _oc.Suppliers.PatchAsync(ocSupplier.ID, new PartialSupplier()
            {
                xp = new
                {
                    ApiClientID = apiClient.ID
                }
            }, token);

            // Assign Supplier API Client to new supplier
            await _oc.ApiClients.SaveAssignmentAsync(new ApiClientAssignment()
            {
                ApiClientID = apiClient.ID,
                SupplierID = ocSupplierID
            }, token);
            // list message senders
            var msList = await _oc.MessageSenders.ListAsync(accessToken: token);
            // create message sender assignment
            var assignmentList = msList.Items.Select(ms =>
            {
                return new MessageSenderAssignment
                {
                    MessageSenderID = ms.ID,
                    SupplierID = ocSupplierID
                };
            });
            await Throttler.RunAsync(assignmentList, 100, 5, a => _oc.MessageSenders.SaveAssignmentAsync(a, token));
            return supplier;
        }
    
        public async Task CreateUserTypeUserGroupsAndSecurityProfileAssignments(User user, string token, string supplierID)
        {
            // Assign supplier to MPMeAdmin security profile
            await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
            {
                SupplierID = supplierID,
                SecurityProfileID = "MPMeAdmin"
            }, token);

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
