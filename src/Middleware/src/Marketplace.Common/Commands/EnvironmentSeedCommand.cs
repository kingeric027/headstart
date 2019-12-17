﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Controllers;
using Marketplace.Common.Models;
using Marketplace.Common.Services.DevCenter;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    public interface IEnvironmentSeedCommand
    {
        Task Seed(EnvironmentSeed seed, VerifiedUserContext user);
    }
    public class EnvironmentSeedCommand : IEnvironmentSeedCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly IAppSettings _settings;
        private readonly IDevCenterService _dev;
        private EnvironmentSeed _seed;

        public EnvironmentSeedCommand(IAppSettings settings, IOrderCloudClient oc, IDevCenterService dev)
        {
            _settings = settings;
            _oc = oc;
            _dev = dev;
        }

        public async Task Seed(EnvironmentSeed seed, VerifiedUserContext user)
        {
            _seed = seed;
            var org = await this.CreateOrganization(user.AccessToken);
            var company = await _dev.GetOrganizations(org.OwnerDevID, user.AccessToken);

            // at this point everything we do is as impersonation of the admin user on a new token
            var impersonation = await _dev.Impersonate(company.Items.FirstOrDefault(c => c.AdminCompanyID == org.ID).ID, user.AccessToken);
            await this.PatchDefaultApiClients(impersonation.access_token);
            await this.CreateSuppliers(user, impersonation.access_token);
            await this.ConfigureBuyers(impersonation.access_token);
        }

        private async Task ConfigureBuyers(string token)
        {
            foreach (var (key, value) in _seed.Suppliers)
            {
                await _oc.Catalogs.SaveAssignmentAsync(new CatalogAssignment()
                {
                    BuyerID = "Default_Marketplace_Buyer",
                    CatalogID = key,
                    ViewAllCategories = true,
                    ViewAllProducts = true
                }, token);
            }
        }

        private async Task CreateSuppliers(VerifiedUserContext user, string token)
        {
            var profile = await _oc.SecurityProfiles.CreateAsync(new SecurityProfile()
            {
                CustomRoles = new List<string>(),
                ID = "supplierIntegration",
                Name = "Supplier Integration Security Profile",
                Roles = new List<ApiRole>() { ApiRole.FullAccess }
            }, token);

            foreach (var (key, value) in _seed.Suppliers)
            {
                var supplier = await _oc.Suppliers.CreateAsync(new Supplier()
                {
                    Active = true,
                    ID = key,
                    Name = value,
                    xp = { }
                }, token);
                var catalog = await _oc.Catalogs.CreateAsync(new Catalog()
                {
                    Active = true,
                    ID = $"{supplier.ID}",
                    Description = $"{supplier.Name} Default Catalog",
                    Name = $"{supplier.Name} Default Catalog"
                }, token);
                var userGroup = await _oc.SupplierUserGroups.CreateAsync(key, new UserGroup()
                {
                    Description = "Integrations",
                    Name = "Integration Group"
                }, token);
                var supplierUser = await _oc.SupplierUsers.CreateAsync(key, new User()
                {
                    Active = true,
                    Email = user.Email,
                    FirstName = "Integration",
                    LastName = "Developer",
                    Password = _settings.OrderCloudSettings.DefaultPassword,
                    Username = $"dev_{supplier.Name}"
                }, token);
                var apiClient = await _oc.ApiClients.CreateAsync(new ApiClient()
                {
                    AppName = $"Integration Client {value}",
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
                await _oc.SupplierUserGroups.SaveUserAssignmentAsync(key, new UserGroupAssignment()
                {
                    UserID = supplierUser.ID,
                    UserGroupID = userGroup.ID
                }, token);
                await _oc.ApiClients.SaveAssignmentAsync(new ApiClientAssignment()
                {
                    ApiClientID = apiClient.ID,
                    SupplierID = supplier.ID
                }, token);
                await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
                {
                    //UserID = user.ID,
                    SupplierID = supplier.ID,
                    UserGroupID = userGroup.ID,
                    SecurityProfileID = profile.ID
                }, token);
            }
        }

        private async Task PatchDefaultApiClients(string token)
        {
            var list = await _oc.ApiClients.ListAsync(accessToken: token);
            var tasks = list.Items.Select(client => _oc.ApiClients.PatchAsync(client.ID, new PartialApiClient()
            {
                Active = true,
                AllowAnyBuyer = client.AppName.Contains("Buyer"),
                AllowAnySupplier = client.AppName.Contains("Admin"),
                AllowSeller = client.AppName.Contains("Admin"),
                AccessTokenDuration = 600,
                RefreshTokenDuration = 43200,
            }, accessToken: token))
                .ToList();
            await Task.WhenAll(tasks);
        }

        private async Task<AdminCompany> CreateOrganization(string token)
        {
            var org = new Organization()
            {
                Name = $"Marketplace.Print {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}",
                Active = true,
                BuyerID = "Default_Marketplace_Buyer",
                BuyerApiClientName = $"Default Marketplace Buyer UI",
                BuyerName = $"Default Marketplace Buyer",
                BuyerUserName = $"Default_Buyer",
                BuyerPassword = _settings.OrderCloudSettings.DefaultPassword,
                SellerApiClientName = $"Default Marketplace Admin UI",
                SellerPassword = _settings.OrderCloudSettings.DefaultPassword,
                SellerUserName = $"Default_Admin"
            };
            var request = await _dev.PostOrganization(org, token);
            return request;
        }
    }
}