using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Common.Services.DevCenter.Models;
using Marketplace.Helpers.Models;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Marketplace;
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
        private readonly IMarketplaceSupplierCommand _command;

        public EnvironmentSeedCommand(AppSettings settings, IOrderCloudClient oc, IDevCenterService dev, IMarketplaceSupplierCommand command)
        {
            _settings = settings;
            _oc = oc;
            _dev = dev;
            _command = command;
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
            await this.CreateMarketPlaceRoles(impersonation.access_token);
            await this.CreateSuppliers(user, impersonation.access_token);
            //await this.ConfigureBuyers(impersonation.access_token);
        }

        //private async Task ConfigureBuyers(string token) {}

        private async Task CreateSuppliers(VerifiedUserContext user, string token)
        {
            var profile = await _oc.SecurityProfiles.CreateAsync(new SecurityProfile()
            {
                CustomRoles = new List<string>(),
                ID = "supplierIntegration",
                Name = "Supplier Integration Security Profile",
                Roles = new List<ApiRole>() { ApiRole.FullAccess }
            }, token);

            // Create Suppliers and necessary user groups and security profile assignments
            foreach (MarketplaceSupplier supplier in _seed.Suppliers)
            {
                await _command.Create(supplier, user, token);
            }

           //Add xp index for SupplierUserGroup.xp.Type
           await _oc.XpIndices.PutAsync(new XpIndex
            {
                ThingType = XpThingType.UserGroup,
                Key = "Type"
            }, token);
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
            },
            new Webhook() {
              Name = "Order Approved",
              Description = "Triggers email letting user know the order was approved.",
              Url = "/orderapproved",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}/approve", Verb = "POST" }
              }
            },
            new Webhook() {
              Name = "Order Declined",
              Description = "Triggers email letting user know the order was declined.",
              Url = "/orderdeclined",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}/decline", Verb = "POST" }
              }
            },
            new Webhook() {
              Name = "Order Shipped",
              Description = "Triggers email letting user know the order was shipped.",
              Url = "/ordershipped",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}/ship", Verb = "POST" }
              }
            },
            new Webhook() {
              Name = "Order Cancelled",
              Description = "Triggers email letting user know the order has been cancelled.",
              Url = "/ordercancelled",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}/cancel", Verb = "POST" }
              }
            },
            new Webhook() {
              Name = "Order Updated",
              Description = "Triggers email letting user know the order has been updated.",
              Url = "/orderupdated",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/orders/{direction}/{orderID}", Verb = "PATCH" }
              }
            },
            new Webhook() {
              Name = "New User",
              Description = "Triggers an email welcoming the buyer user.  Triggers an email letting admin know about the new buyer user.",
              Url = "/newuser",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/buyers/{buyerID}/users", Verb = "POST" }
              }
            },
            new Webhook() {
              Name = "Product Created",
              Description = "Triggers email to user with details of newly created product.",
              Url = "/productcreated",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/products", Verb = "POST" }
              }
            },
            new Webhook() {
              Name = "Product Update",
              Description = "Triggers email to user indicating that a product has been updated.",
              Url = "/productupdate",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/products/{productID}", Verb = "PATCH" }
              }
            },
            new Webhook() {
              Name = "Supplier Updated",
              Description = "Triggers email letting user know the supplier has been updated.",
              Url = "/supplierupdated",
              HashKey = "asdffdsa",
              ElevatedRoles =
                new List<ApiRole>
                {
                    ApiRole.FullAccess
                },
              BeforeProcessRequest = false,
              WebhookRoutes = new List<WebhookRoute>
              {
                new WebhookRoute() { Route = "v1/suppliers/{supplierID}", Verb = "PATCH" }
              }
            },
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
            new SecurityProfile(){ Name = "MPMeProductAdmin", CustomRoles = { "MPMeProductAdmin" }, ID = "MPMeProductAdmin", Roles = { ApiRole.ProductAdmin, ApiRole.PriceScheduleAdmin, ApiRole.InventoryAdmin, ApiRole.SupplierAddressReader } },
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
            new SecurityProfile(){ Name = "MPSupplierUserGroupAdmin", CustomRoles = { "MPSupplierUserGroupAdmin" }, ID = "MPSupplierUserGroupAdmin", Roles = { ApiRole.SupplierReader, ApiRole.SupplierUserGroupAdmin } },
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
