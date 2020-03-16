using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Common.Services.DevCenter.Models;
using Marketplace.Helpers.Models;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models.Models.Misc;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    public interface IEnvironmentSeedCommand
    {
		Task<ImpersonationToken> Seed(EnvironmentSeed seed, VerifiedUserContext user);
    }
	public class EnvironmentSeedCommand : IEnvironmentSeedCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly AppSettings _settings;
		private readonly IDevCenterService _dev;
		private EnvironmentSeed _seed;
		private readonly IMarketplaceSupplierCommand _supplierCommand;
		private readonly IMarketplaceBuyerCommand _buyerCommand;

		public EnvironmentSeedCommand(AppSettings settings, IOrderCloudClient oc, IDevCenterService dev, IMarketplaceSupplierCommand supplierCommand, IMarketplaceBuyerCommand buyerCommand)
		{
			_settings = settings;
			_oc = oc;
			_dev = dev;
			_supplierCommand = supplierCommand;
			_buyerCommand = buyerCommand;
		}

		public async Task<ImpersonationToken> Seed(EnvironmentSeed seed, VerifiedUserContext user)
		{
			_seed = seed;
			var org = await CreateOrganization(user.AccessToken);
			var company = await _dev.GetOrganizations(org.OwnerDevID, user.AccessToken);

			// at this point everything we do is as impersonation of the admin user on a new token
			var impersonation = await _dev.Impersonate(company.Items.FirstOrDefault(c => c.AdminCompanyID == org.ID).ID, user.AccessToken);
			await PatchDefaultApiClients(impersonation.access_token);
			await CreateWebhooks(impersonation.access_token, "https://marketplace-api-staging.azurewebsites.net");
			await CreateMarketPlaceRoles(impersonation.access_token);
			await CreateBuyers(user, impersonation.access_token);
			await CreateSuppliers(user, impersonation.access_token);
			await CreateXPIndices(impersonation.access_token);
			await CreateIncrementors(impersonation.access_token);
			//await this.ConfigureBuyers(impersonation.access_token);
			return impersonation;
		}

		private async Task CreateBuyers(VerifiedUserContext user, string token) {
			foreach (var buyer in _seed.Buyers)
			{
				await _buyerCommand.Create(buyer, user, token);
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

			// Create Suppliers and necessary user groups and security profile assignments
			foreach (MarketplaceSupplier supplier in _seed.Suppliers)
			{
				await _supplierCommand.Create(supplier, user, token);
			}
		}

		static readonly List<XpIndex> DefaultIndices = new List<XpIndex>() {
			new XpIndex { ThingType = XpThingType.UserGroup, Key = "Type" },       
			new XpIndex { ThingType = XpThingType.Product, Key = "Images.URL" },       
			new XpIndex { ThingType = XpThingType.Product, Key = "Status" },       
			new XpIndex { ThingType = XpThingType.Company, Key = "Data.ServiceCategory" },       
			new XpIndex { ThingType = XpThingType.Company, Key = "Data.VendorLevel" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "NeedsAttention" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "StopShipSync" },       
			new XpIndex { ThingType = XpThingType.User, Key = "UserGroupID" },       
		};

		public async Task CreateXPIndices(string token)
		{
			foreach (var index in DefaultIndices)
			{
				await _oc.XpIndices.PutAsync(index, token);
			}
		}

		static readonly List<Incrementor> DefaultIncrementors = new List<Incrementor>() {
			new Incrementor { ID = "orderIncrementor", Name = "Order Incrementor", LastNumber = 1, LeftPaddingCount = 6 },
			new Incrementor { ID = "supplierIncrementor", Name = "Supplier Incrementor", LastNumber = 1, LeftPaddingCount = 3 }
		};

		public async Task CreateIncrementors(string token)
		{
			foreach (var incrementor in DefaultIncrementors)
			{
				await _oc.Incrementors.CreateAsync(incrementor, token);
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


		public async Task CreateWebhooks(string accessToken, string baseURL)
		{

			var apiClientResponse = await _oc.ApiClients.ListAsync(accessToken: accessToken);
			foreach (Webhook webhook in DefaultWebhooks)
			{
				webhook.ApiClientIDs = apiClientResponse.Items.Select(apiClient => apiClient.ID).ToList();
				webhook.Url = $"{baseURL}{webhook.Url}";
				webhook.HashKey = _settings.OrderCloudSettings.WebhookHashKey;
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

		public async Task CreateMarketPlaceRoles(string accessToken)
		{
			var profiles = DefaultSecurityProfiles.Select(p =>
				new SecurityProfile() { Name = p.CustomRole.ToString(), ID = p.CustomRole.ToString(), CustomRoles = { p.CustomRole.ToString() }, Roles = p.Roles });

			foreach (var profile in profiles)
			{
				await _oc.SecurityProfiles.CreateAsync(profile, accessToken);
			}
		}

		static readonly List<MarketplaceSecurityProfile> DefaultSecurityProfiles = new List<MarketplaceSecurityProfile>() {
			// seller/supplier
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeProductAdmin, Roles = new[] { ApiRole.ProductAdmin, ApiRole.PriceScheduleAdmin, ApiRole.InventoryAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeProductReader, Roles = new[] { ApiRole.ProductReader, ApiRole.PriceScheduleReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPProductAdmin, Roles = new[] { ApiRole.ProductReader, ApiRole.CatalogAdmin, ApiRole.ProductAssignmentAdmin, ApiRole.ProductFacetAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPProductReader, Roles = new[] { ApiRole.ProductReader, ApiRole.CatalogReader, ApiRole.ProductFacetReader} },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPPromotionAdmin, Roles = new[] { ApiRole.PromotionAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPPromotionReader, Roles = new[] { ApiRole.PromotionReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPCategoryAdmin, Roles = new[] { ApiRole.CategoryAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPCategoryReader, Roles = new[] { ApiRole.CategoryReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPOrderAdmin, Roles = new[] { ApiRole.OrderAdmin, ApiRole.ShipmentReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPOrderReader, Roles = new[] { ApiRole.OrderReader, ApiRole.ShipmentAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPShipmentAdmin, Roles = new[] { ApiRole.OrderReader, ApiRole.ShipmentAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPBuyerAdmin, Roles = new[] { ApiRole.BuyerAdmin, ApiRole.BuyerUserAdmin, ApiRole.UserGroupAdmin, ApiRole.AddressAdmin, ApiRole.CreditCardAdmin, ApiRole.ApprovalRuleAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPBuyerReader, Roles = new[] { ApiRole.BuyerReader, ApiRole.BuyerUserReader, ApiRole.UserGroupReader, ApiRole.AddressReader, ApiRole.CreditCardReader, ApiRole.ApprovalRuleReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPSellerAdmin, Roles = new[] { ApiRole.AdminUserAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPSupplierAdmin, Roles = new[] { ApiRole.SupplierAdmin, ApiRole.SupplierUserAdmin, ApiRole.SupplierAddressAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeSupplierAddressAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierAddressAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeSupplierUserAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierUserAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPSupplierUserGroupAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierUserGroupAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPReportReader },
			
			// buyer
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPBaseBuyer, Roles = new[] { ApiRole.MeXpAdmin, ApiRole.ProductFacetReader, ApiRole.Shopper, ApiRole.SupplierAddressReader, ApiRole.SupplierReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPApprovalRuleAdmin , Roles = new[] { ApiRole.ApprovalRuleAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPCreditCardAdmin  , Roles = new[] { ApiRole.MeCreditCardAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPAddressAdmin  , Roles = new[] { ApiRole.MeAddressAdmin, ApiRole.AddressAdmin } },
		};

		static readonly List<Webhook> DefaultWebhooks = new List<Webhook>() {
			new Webhook() {
			  Name = "Buyer Patch Address Validation Pre-webhook",
			  Description = "Address validation is performed with FreightPOP prior to creates or updates throughout the marketplace to ensure that rate requests do not fail during checkout. Ideally this same validation will prevent avalara calls from failing during checkout as well. We will need to revisit to ensure this validation works for both of these integrations",
			  Url = "/validatebuyeraddresspatch",
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
	}
}
