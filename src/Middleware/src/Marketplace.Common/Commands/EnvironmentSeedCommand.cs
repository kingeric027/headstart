using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Helpers;
using Marketplace.Common.Services;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Common.Services.DevCenter.Models;
using Marketplace.Models;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    public interface IEnvironmentSeedCommand
    {
		Task<ImpersonationToken> Seed(EnvironmentSeed seed, VerifiedUserContext user);
		Task PostStagingRestore();
    }
	public class EnvironmentSeedCommand : IEnvironmentSeedCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly AppSettings _settings;
		private readonly IDevCenterService _dev;
		private EnvironmentSeed _seed;
		private readonly IMarketplaceSupplierCommand _supplierCommand;
		private readonly IMarketplaceBuyerCommand _buyerCommand;
		private readonly string _localWebhookUrl = "https://marketplaceteam.ngrok.io";
		private readonly string _allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		private static string _adminUIApiClientID;
		private static string _buyerUIApiClientID;
		private static string _middlewareApiClientID;

		public EnvironmentSeedCommand(AppSettings settings, IOrderCloudClient oc, IDevCenterService dev, IMarketplaceSupplierCommand supplierCommand, IMarketplaceBuyerCommand buyerCommand, IOrderCloudSandboxService orderCloudSandboxService)
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
			await CreateApiClients(impersonation.access_token);
			await SetApiClientIDs(impersonation.access_token); // must be before patch api clients
			await PatchDefaultApiClients(impersonation.access_token);
			await CreateWebhooks(impersonation.access_token, seed.ApiUrl);
			await CreateMessageSenders(impersonation.access_token, seed.ApiUrl);
			await CreateMarketPlaceRoles(impersonation.access_token);
			await CreateIncrementors(impersonation.access_token); // must be before create buyers
			await CreateBuyers(user, impersonation.access_token);
			await CreateXPIndices(impersonation.access_token);
			await CreateAndAssignIntegrationEvents(seed.ApiUrl, impersonation.access_token);
			await CreateSuppliers(user, impersonation.access_token);
			//await this.ConfigureBuyers(impersonation.access_token);
			return impersonation;
		}

		public async Task PostStagingRestore()
		{	
			var token = (await _oc.AuthenticateAsync()).AccessToken;
			var baseUrl = _settings.EnvironmentSettings.BaseUrl;
			await SetApiClientIDs(token);

			var deleteMS = DeleteAllMessageSenders(token);
			var deleteWH = DeleteAllWebhooks(token);
			var deleteIE = DeleteAllIntegrationEvents(token);
			await Task.WhenAll(deleteMS, deleteWH, deleteIE);

			var createMS = CreateMessageSenders(token, baseUrl);
			var createWH = CreateWebhooks(token, baseUrl);
			var createIE = CreateAndAssignIntegrationEvents(token, baseUrl);
			await Task.WhenAll(createMS, createWH, createIE);
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


		private async Task CreateBuyers(VerifiedUserContext user, string token) {
			foreach (var buyer in _seed.Buyers)
			{
				var superBuyer = new SuperMarketplaceBuyer()
				{
					Buyer = buyer,
					Markup = new BuyerMarkup() { Percent = 0 }
				};
				await _buyerCommand.Create(superBuyer, token);
			}
		}

		private async Task CreateSuppliers(VerifiedUserContext user, string token)
		{
			// Create Suppliers and necessary user groups and security profile assignments
			foreach (MarketplaceSupplier supplier in _seed.Suppliers)
			{
				await _supplierCommand.Create(supplier, user, token);
			}
		}

		static readonly List<XpIndex> DefaultIndices = new List<XpIndex>() {
			new XpIndex { ThingType = XpThingType.UserGroup, Key = "Type" },       
			new XpIndex { ThingType = XpThingType.UserGroup, Key = "Role" },       
			new XpIndex { ThingType = XpThingType.Company, Key = "Data.ServiceCategory" },       
			new XpIndex { ThingType = XpThingType.Company, Key = "Data.VendorLevel" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "NeedsAttention" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "StopShipSync" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "OrderType" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "LocationID" },       
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
			new Incrementor { ID = "supplierIncrementor", Name = "Supplier Incrementor", LastNumber = 1, LeftPaddingCount = 3 },
			new Incrementor { ID = "buyerIncrementor", Name = "Buyer Incrementor", LastNumber = 1, LeftPaddingCount = 4 }
		};

		public async Task CreateIncrementors(string token)
		{
			foreach (var incrementor in DefaultIncrementors)
			{
				await _oc.Incrementors.CreateAsync(incrementor, token);
			}
		}

		private async Task SetApiClientIDs(string token)
		{
			var list = await _oc.ApiClients.ListAsync(accessToken: token);
			_adminUIApiClientID = list.Items.First(a => a.AppName == "Default Marketplace Admin UI").ID;
			_buyerUIApiClientID = list.Items.First(a => a.AppName == "Default Marketplace Buyer UI").ID;
			_middlewareApiClientID = list.Items.First(a => a.AppName == "Middleware Integrations").ID;
		}

		private async Task PatchDefaultApiClients(string token)
		{
			var buyer = _oc.ApiClients.PatchAsync(_buyerUIApiClientID, new PartialApiClient()
			{
				Active = true,
				AllowAnyBuyer = true,
				AllowAnySupplier = false,
				AllowSeller = false,
				AccessTokenDuration = 600,
				RefreshTokenDuration = 43200,
			}, accessToken: token);
			var admin = _oc.ApiClients.PatchAsync(_adminUIApiClientID, new PartialApiClient()
			{
				Active = true,
				AllowAnyBuyer = true,
				AllowAnySupplier = false,
				AllowSeller = false,
				AccessTokenDuration = 600,
				RefreshTokenDuration = 43200,
			}, accessToken: token);
			await Task.WhenAll(buyer, admin);
		}

		private async Task CreateApiClients(string token)
		{
			var clientSecret = RandomGen.GetString(_allowedChars, 60);
			var integrationsClient = new PartialApiClient()
			{
				AppName = "Middleware Integrations",
				Active = true,
				AllowAnyBuyer = false,
				AllowAnySupplier = false,
				AllowSeller = true,
				AccessTokenDuration = 600,
				RefreshTokenDuration = 43200,
				DefaultContextUserName = "Default_Admin",
				ClientSecret = clientSecret
			};
			await _oc.ApiClients.CreateAsync(integrationsClient, token);
		}

		private async Task CreateMessageSenders(string accessToken, string baseURL)
		{
			foreach (var messageSender in DefaultMessageSenders)
			{
				messageSender.URL = $"{baseURL}{messageSender.URL}";
				await _oc.MessageSenders.CreateAsync(messageSender, accessToken);
			}
		}
		private async Task CreateAndAssignIntegrationEvents(string token, string apiUrl)
		{
			await _oc.IntegrationEvents.CreateAsync(new IntegrationEvent()
			{
				ElevatedRoles = new [] { ApiRole.FullAccess },
				ID = "freightpopshipping",
				EventType = IntegrationEventType.OrderCheckout,
				CustomImplementationUrl = apiUrl,
				Name = "FreightPOP Shipping",
				HashKey = _settings.OrderCloudSettings.WebhookHashKey,
				ConfigData = new
				{
					ExcludePOProductsFromShipping = true,
					ExcludePOProductsFromTax = true,
				}
			}, token);
			await _oc.IntegrationEvents.CreateAsync(new IntegrationEvent()
			{
				ElevatedRoles = new [] { ApiRole.FullAccess },
				ID = "freightpopshippingLOCAL",
				EventType = IntegrationEventType.OrderCheckout,
				CustomImplementationUrl = _localWebhookUrl,
				Name = "FreightPOP Shipping LOCAL",
				HashKey = _settings.OrderCloudSettings.WebhookHashKey,
				ConfigData = new
				{
					ExcludePOProductsFromShipping = true,
					ExcludePOProductsFromTax = true,
				}
			}, token);
			await _oc.ApiClients.PatchAsync(_buyerUIApiClientID, new PartialApiClient { OrderCheckoutIntegrationEventID = "freightpopshipping" });
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

		public async Task DeleteAllWebhooks(string token)
		{
			var webhooks = await _oc.Webhooks.ListAsync(pageSize: 100, accessToken: token);
			await Throttler.RunAsync(webhooks.Items, 500, 20, webhook => 
				_oc.Webhooks.DeleteAsync(webhook.ID, token));
		}

		public async Task DeleteAllMessageSenders(string token)
		{
			var messageSenders = await _oc.MessageSenders.ListAsync(pageSize: 100, accessToken: token);
			await Throttler.RunAsync(messageSenders.Items, 500, 20, messageSender =>
				_oc.MessageSenders.DeleteAsync(messageSender.ID));
		}

		public async Task DeleteAllIntegrationEvents(string token)
		{
			await _oc.ApiClients.PatchAsync(_buyerUIApiClientID, new PartialApiClient { OrderCheckoutIntegrationEventID = null });
			var integrationEvents = await _oc.IntegrationEvents.ListAsync(pageSize: 100, accessToken: token);
			await Throttler.RunAsync(integrationEvents.Items, 500, 20, integrationEvent =>
				_oc.IntegrationEvents.DeleteAsync(integrationEvent.ID));
		}

		private async Task CreateWebhooks(string accessToken, string baseURL)
		{
			var DefaultWebhooks = new List<Webhook>() {
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
			  },
			  ApiClientIDs = new [] { _buyerUIApiClientID }
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
			  },
			  ApiClientIDs = new [] { _buyerUIApiClientID }
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
			  },
			  ApiClientIDs = new [] { _adminUIApiClientID }
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
			  },
			  ApiClientIDs = new [] { _adminUIApiClientID }
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
			  },
			  ApiClientIDs = new [] { _buyerUIApiClientID, _adminUIApiClientID }
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
			  },
			  ApiClientIDs = new [] { _adminUIApiClientID }
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
			  },
			  ApiClientIDs = new [] { _adminUIApiClientID }
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
			  },
			 ApiClientIDs = new [] { _adminUIApiClientID }
			},
		};
			foreach (Webhook webhook in DefaultWebhooks)
			{
				webhook.Url = $"{baseURL}{webhook.Url}";
				webhook.HashKey = _settings.OrderCloudSettings.WebhookHashKey;
				await _oc.Webhooks.CreateAsync(webhook, accessToken);
			}
		}

		static readonly List<MessageSender> DefaultMessageSenders = new List<MessageSender>() 
		{
			new MessageSender()
			{
				Name = "Password Reset",
				MessageTypes = new[] { MessageType.ForgottenPassword },
				URL = "/passwordreset",
				SharedKey = "wkaWSxPBBAABaxEp", // Where does this come from? Should it live somewhere else?
				xp = new { 
						MessageTypeConfig = new {
							MessageType = "ForgottenPassword",
							FromEmail = "noreply@ordercloud.io",
							Subject = "Here is the link to reset your password",
							TemplateName = "ForgottenPassword",
							MainContent = "ForgottenPassword"
						}
				}
			}
		};
		
		static readonly List<MarketplaceSecurityProfile> DefaultSecurityProfiles = new List<MarketplaceSecurityProfile>() {
			// seller/supplier
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeProductAdmin, Roles = new[] { ApiRole.ProductAdmin, ApiRole.PriceScheduleAdmin, ApiRole.InventoryAdmin, ApiRole.ProductFacetReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeProductReader, Roles = new[] { ApiRole.ProductReader, ApiRole.PriceScheduleReader, ApiRole.ProductFacetReader } },
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
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeSupplierAdmin, Roles = new[] {ApiRole.SupplierReader, ApiRole.SupplierAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeSupplierAddressAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierAddressAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeSupplierUserAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierUserAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPSupplierUserGroupAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierUserGroupAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPReportReader },
			
			// buyer
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPBaseBuyer, Roles = new[] { ApiRole.MeAdmin, ApiRole.MeCreditCardAdmin, ApiRole.MeAddressAdmin, ApiRole.MeXpAdmin, ApiRole.ProductFacetReader, ApiRole.Shopper, ApiRole.SupplierAddressReader, ApiRole.SupplierReader } },
			
			/* these roles don't do much, access to changing location information will be done through middleware calls that
			*  confirm the user is in the location specific access user group. These roles will be assigned to the location 
			*  specific user group and allow us to determine if a user has an admin role for at least one location through 
			*  the users JWT
			*/
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPLocationPermissionAdmin },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPLocationOrderApprover },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPLocationNeedsApproval },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPLocationViewAllOrders },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPLocationCreditCardAdmin },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPLocationAddressAdmin },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPLocationResaleCertAdmin }
		};
	}
}
