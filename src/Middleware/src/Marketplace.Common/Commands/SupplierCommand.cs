using Marketplace.Common.Helpers;
using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceSupplierCommand
    {
        Task<MarketplaceSupplier> Create(MarketplaceSupplier supplier, VerifiedUserContext user);
    }
    public class MarketplaceSupplierCommand : IMarketplaceSupplierCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;

        public MarketplaceSupplierCommand(AppSettings settings)
        {
            _settings = settings;
            _oc = OcFactory.GetSEBAdmin();
        }
        public async Task<MarketplaceSupplier> Create(MarketplaceSupplier supplier, VerifiedUserContext user)
        {
            // Map Marketplace Supplier to OrderCloud Supplier
            var mappedSupplier = MarketplaceSupplierMapper.Map(supplier);
            var token = user.AccessToken;
            // Create Supplier
            var ocSupplier = await _oc.Suppliers.CreateAsync(mappedSupplier, token);
            var ocSupplierID = ocSupplier.ID;
            // Create Integration User Group
            var integrationUserGroup = await _oc.SupplierUserGroups.CreateAsync(ocSupplierID, new UserGroup()
            {
                Description = "Integrations",
                Name = "Integration Group"
            });
            // Create 3 User Groups for supplier with `xp.Type = "UserPermissions"`
            var accountAdminUserGroup = await _oc.SupplierUserGroups.CreateAsync(ocSupplierID, new UserGroup()
            {
                ID = $"{ocSupplierID}AccountAdmin",
                Name = $"{ocSupplierID} Account Admin",
                xp =
                    {
                        Type = "UserPermissions"
                    }
            });
            var orderAdminUserGroup = await _oc.SupplierUserGroups.CreateAsync(ocSupplierID, new UserGroup()
            {
                ID = $"{ocSupplierID}OrderAdmin",
                Name = $"{ocSupplierID} Order Admin",
                xp =
                    {
                        Type = "UserPermissions"
                    }
            });
            var productAdminUserGroup = await _oc.SupplierUserGroups.CreateAsync(ocSupplierID, new UserGroup()
            {
                ID = $"{ocSupplierID}ProductAdmin",
                Name = $"{ocSupplierID} Product Admin",
                xp =
                    {
                        Type = "UserPermissions"
                    }
            });
            // Create Integrations Supplier User
            var supplierUser = await _oc.SupplierUsers.CreateAsync(ocSupplierID, new User()
            {
                Active = true,
                Email = user.Email,
                FirstName = "Integration",
                LastName = "Developer",
                Password = "Four51Yet!", // _settings.OrderCloudSettings.DefaultPassword,
                Username = $"dev_{ocSupplierID}"
            });
            // Create API Client for new supplier
            var apiClient = await _oc.ApiClients.CreateAsync(new ApiClient()
            {
                AppName = $"Integration Client {ocSupplier.Name}",
                Active = true,
                DefaultContextUserName = supplierUser.Username,
                ClientSecret = "d576450ca8f89967eea0d3477544ea4bee60af051a5c173be09db08c562b", // _settings.OrderCloudSettings.ClientSecret,
                AccessTokenDuration = 600,
                RefreshTokenDuration = 43200,
                AllowAnyBuyer = false,
                AllowAnySupplier = false,
                AllowSeller = false,
                IsAnonBuyer = false,
            });
            // Assign Integration Supplier User to Integration Supplier User Group
            await _oc.SupplierUserGroups.SaveUserAssignmentAsync(ocSupplierID, new UserGroupAssignment()
            {
                UserID = supplierUser.ID,
                UserGroupID = integrationUserGroup.ID
            });
            // Assign Supplier API Client to new supplier
            await _oc.ApiClients.SaveAssignmentAsync(new ApiClientAssignment()
            {
                ApiClientID = apiClient.ID,
                SupplierID = ocSupplierID
            });
            // Assign Integration Security Profile to Integration User Group
            await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
            {
                //UserID = user.ID,
                SupplierID = ocSupplierID,
                UserGroupID = integrationUserGroup.ID,
                SecurityProfileID = "supplierIntegration"
            });
            // Define Marketplace Security Profiles for each Supplier User Group
            // Account Admin
            var accountAdminMpSecurityProfiles = new List<string>
            {
                "MPMeSupplierAddressAdmin",
                "MPMeSupplierUserAdmin"
            };
            // Order Admin
            var orderAdminMpSecurityProfiles = new List<string>
            {
                "MPOrderAdmin",
                "MPShipmentAdmin"
            };
            // Product Admin
            var productAdminMpSecurityProfiles = new List<string>
            {
                "MPMeProductAdmin",
            };
            // Assign the new supplier's user groups to each of these security profiles
            // Account Admin to respective security profiles
            foreach (string securityProfile in accountAdminMpSecurityProfiles)
            {
                await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
                {
                    SupplierID = ocSupplierID,
                    UserGroupID = accountAdminUserGroup.ID,
                    SecurityProfileID = securityProfile
                });
            };
            // Order Admin to respective security profiles
            foreach (string securityProfile in orderAdminMpSecurityProfiles)
            {
                await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
                {
                    SupplierID = ocSupplierID,
                    UserGroupID = orderAdminUserGroup.ID,
                    SecurityProfileID = securityProfile
                });
            };
            // Product Admin to respecitve security profiles
            foreach (string securityProfile in productAdminMpSecurityProfiles)
            {
                await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
                {
                    SupplierID = ocSupplierID,
                    UserGroupID = productAdminUserGroup.ID,
                    SecurityProfileID = securityProfile
                });
            };
            return MarketplaceSupplierMapper.Map(ocSupplier);
        }
    }
}
