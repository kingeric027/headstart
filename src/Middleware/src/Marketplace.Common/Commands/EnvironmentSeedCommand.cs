﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Helpers.Models;
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
        private readonly AppSettings _settings;
        private readonly IDevCenterService _dev;
        private EnvironmentSeed _seed;

        public EnvironmentSeedCommand(AppSettings settings, IOrderCloudClient oc, IDevCenterService dev)
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
            await this.CreateWebhooks(impersonation.access_token, "https://marketplace-api-qa.azurewebsites.net");
            await this.CreateSuppliers(user, impersonation.access_token);
            await this.CreateMarketPlaceRoles(impersonation.access_token);
            //await this.ConfigureBuyers(impersonation.access_token);
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
                    Password = "Four51Yet!", // _settings.OrderCloudSettings.DefaultPassword,
                    Username = $"dev_{supplier.ID}"
                }, token);
                var apiClient = await _oc.ApiClients.CreateAsync(new ApiClient()
                {
                    AppName = $"Integration Client {value}",
                    Active = true,
                    DefaultContextUserName = supplierUser.Username,
                    ClientSecret = "d576450ca8f89967eea0d3477544ea4bee60af051a5c173be09db08c562b", // _settings.OrderCloudSettings.ClientSecret,
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

        static readonly List<Webhook> DefaultWebhooks = new List<Webhook>() {
            new Webhook() {
              Name = "Buyer Patch Address Validation Pre-webhook",
              Description = "Address validation is performed with FreightPOP prior to creates or updates throughout the marketplace to ensure that rate requests do not fail during checkout. Ideally this same validation will prevent avalara calls from failing during checkout as well. We will need to revisit to ensure this validation works for both of these integrations",
              Url = "/validatebuyeraddresspatch",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = true,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/buyers/{buyerID}/addresses/{addressID}", Verb = "PATCH" }
              }
            },
            new Webhook() {
              Name = "Me Patch Address Validation Pre-webhook",
              Description = "Address validation is performed with FreightPOP prior to creates or updates throughout the marketplace to ensure that rate requests do not fail during checkout. Ideally this same validation will prevent avalara calls from failing during checkout as well. We will need to revisit to ensure this validation works for both of these integrations",
              Url = "/validatemeaddresspatch",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = true,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/me/addresses/{addressID}", Verb = "PATCH" }
              }
            },
            new Webhook() {
              Name = "Order Submit",
              Description = "Takes Buyer Order, forwards order to suppliers, imports supplier orders into freight pop, imports information into zoho, avalara, and card connect",
              Url = "/ordersubmit",
              HashKey = "sadffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}/submit", Verb = "POST" }
              }
            },
            new Webhook() {
              Name = "Post and Put Address Validation Pre-webhook",
              Description = "Address validation is performed with FreightPOP prior to creates or updates throughout the marketplace to ensure that rate requests do not fail during checkout. Ideally this same validation will prevent avalara calls from failing during checkout as well. We will need to revisit to ensure this validation works for both of these integrations",
              Url = "/validateaddresspostput",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = true,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/addresses", Verb = "POST" },
                new WebhookRoute() { Route = "v1/buyers/{buyerID}/addresses", Verb = "POST" },
                new WebhookRoute() { Route = "v1/me/addresses", Verb = "POST" },
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}/billto", Verb = "PUT" },
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}/shipto", Verb = "PUT" },
                new WebhookRoute() { Route = "v1/suppliers/{supplierID}/addresses/{addressID}", Verb = "PUT" },
                new WebhookRoute() { Route = "v1/addresses/{addressID}", Verb = "PUT" },
                new WebhookRoute() { Route = "v1/buyers/{buyerID}/addresses/{addressID}", Verb = "PUT" },
                new WebhookRoute() { Route = "v1/me/addresses/{addressID}", Verb = "PUT" },
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}/lineitems/{lineItemID}/shipto", Verb = "PUT" },
                new WebhookRoute() { Route = "v1/suppliers/{supplierID}/addresses", Verb = "POST" }
              }
            },
            new Webhook() {
              Name = "Seller Patch Address Validation Pre-webhook",
              Description = "Address validation is performed with FreightPOP prior to creates or updates throughout the marketplace to ensure that rate requests do not fail during checkout. Ideally this same validation will prevent avalara calls from failing during checkout as well. We will need to revisit to ensure this validation works for both of these integrations",
              Url = "/validateselleraddresspatch",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = true,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/addresses/{addressID}", Verb = "PATCH" }
              }
            },
            new Webhook() {
              Name = "Supplier Patch Address Validation Pre-webhook",
              Description = "Address validation is performed with FreightPOP prior to creates or updates throughout the marketplace to ensure that rate requests do not fail during checkout. Ideally this same validation will prevent avalara calls from failing during checkout as well. We will need to revisit to ensure this validation works for both of these integrations",
              Url = "/validatesupplieraddresspatch",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = true,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/suppliers/{supplierID}/addresses/{addressID}", Verb = "PATCH" }
              }
            }
        };
        public async Task CreateWebhooks(string accessToken, string baseURL)
        {
            var apiClientResponse = await _oc.ApiClients.ListAsync(accessToken: accessToken);
            foreach (Webhook webhook in DefaultWebhooks)
            {
                webhook.ApiClientIDs = apiClientResponse.Items.Select(apiClient => apiClient.ID).ToList();
                webhook.Url = $"{baseURL}{webhook.Url}";
                await _oc.Webhooks.CreateAsync(webhook, accessToken);
            }
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
                BuyerPassword = "Four51Yet!", // _settings.OrderCloudSettings.DefaultPassword,
                SellerApiClientName = $"Default Marketplace Admin UI",
                SellerPassword = "Four51Yet!", // _settings.OrderCloudSettings.DefaultPassword,
                SellerUserName = $"Default_Admin"
            };
            var request = await _dev.PostOrganization(org, token);
            return request;
        }


        static readonly List<SecurityProfile> DefaultSecurityProfiles = new List<SecurityProfile>() {
            new SecurityProfile(){ Name = "MPMeProductAdmin", CustomRoles = { "MPMeProductAdmin" }, ID = "MPMeProductAdmin", Roles = { ApiRole.ProductAdmin, ApiRole.PriceScheduleAdmin, ApiRole.InventoryAdmin } },
            new SecurityProfile(){ Name = "MPMeProductReader", CustomRoles = { "MPMeProductReader" }, ID = "MPMeProductReader", Roles = { ApiRole.ProductReader, ApiRole.PriceScheduleReader } },
            new SecurityProfile(){ Name = "MPProductAdmin", CustomRoles = { "MPProductAdmin" }, ID = "MPProductAdmin", Roles = { ApiRole.ProductReader, ApiRole.CatalogAdmin, ApiRole.ProductAssignmentAdmin, ApiRole.ProductFacetAdmin } },
            new SecurityProfile(){ Name = "MPProductReader", CustomRoles = { "MPProductReader" }, ID = "MPProductReader", Roles = { ApiRole.ProductReader, ApiRole.CatalogReader, ApiRole.ProductFacetReader} },
            new SecurityProfile(){ Name = "MPPromotionAdmin", CustomRoles = { "MPPromotionAdmin" }, ID = "MPPromotionAdmin", Roles = { ApiRole.PromotionAdmin } },
            new SecurityProfile(){ Name = "MPPromotionReader", CustomRoles = { "MPPromotionReader" }, ID = "MPPromotionReader", Roles = { ApiRole.PromotionReader } },
            new SecurityProfile(){ Name = "MPCategoryAdmin", CustomRoles = { "MPCategoryAdmin" }, ID = "MPCategoryAdmin", Roles = { ApiRole.CategoryAdmin } },
            new SecurityProfile(){ Name = "MPCategoryReader", CustomRoles = { "MPCategoryReader" }, ID = "MPCategoryReader", Roles = { ApiRole.CategoryReader } },
            new SecurityProfile(){ Name = "MPOrderAdmin", CustomRoles = { "MPOrderAdmin" }, ID = "MPOrderAdmin", Roles = { ApiRole.OrderAdmin, ApiRole.ShipmentReader } },
            new SecurityProfile(){ Name = "MPOrderReader", CustomRoles = { "MPOrderReader" }, ID = "MPOrderReader", Roles = { ApiRole.OrderReader, ApiRole.ShipmentAdmin } },
            new SecurityProfile(){ Name = "MPShipmentAdmin", CustomRoles = { "MPShipmentAdmin" }, ID = "MPShipmentAdmin", Roles = { ApiRole.OrderReader, ApiRole.ShipmentAdmin } },
            new SecurityProfile(){ Name = "MPBuyerAdmin", CustomRoles = { "MPBuyerAdmin" }, ID = "MPBuyerAdmin", Roles = { ApiRole.BuyerAdmin, ApiRole.BuyerUserAdmin, ApiRole.UserGroupAdmin, ApiRole.AddressAdmin, ApiRole.CreditCardAdmin, ApiRole.ApprovalRuleAdmin } },
            new SecurityProfile(){ Name = "MPBuyerReader", CustomRoles = { "MPBuyerReader" }, ID = "MPBuyerReader", Roles = { ApiRole.BuyerReader, ApiRole.BuyerUserReader, ApiRole.UserGroupReader, ApiRole.AddressReader, ApiRole.CreditCardReader, ApiRole.ApprovalRuleReader } },
            new SecurityProfile(){ Name = "MPSellerAdmin", CustomRoles = { "MPSellerAdmin" }, ID = "MPSellerAdmin", Roles = { ApiRole.AdminUserAdmin } },
            new SecurityProfile(){ Name = "MPSupplierAdmin", CustomRoles = { "MPSupplierAdmin" }, ID = "MPSupplierAdmin", Roles = { ApiRole.SupplierAdmin, ApiRole.SupplierUserAdmin, ApiRole.SupplierAddressAdmin } },
            new SecurityProfile(){ Name = "MPMeSupplierAddressAdmin", CustomRoles = { "MPMeSupplierAddressAdmin" }, ID = "MPMeSupplierAddressAdmin", Roles = { ApiRole.SupplierReader, ApiRole.SupplierAddressAdmin } },
            new SecurityProfile(){ Name = "MPMeSupplierUserAdmin", CustomRoles = { "MPMeSupplierUserAdmin" }, ID = "MPMeSupplierUserAdmin", Roles = { ApiRole.SupplierReader, ApiRole.SupplierUserAdmin } },
        };
        public async Task CreateMarketPlaceRoles(string accessToken)
        {
            foreach (SecurityProfile securityProfile in DefaultSecurityProfiles)
            {
                await _oc.SecurityProfiles.CreateAsync(securityProfile, accessToken);
            }
        }
    }
}
