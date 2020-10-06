using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Helpers;
using Marketplace.Common.Services.DevCenter;
using Marketplace.Models;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Commands
{
    public interface IEnvironmentSeedCommand
    {
		Task<string> Seed(EnvironmentSeed seed, VerifiedUserContext devUserContext);
		Task PostStagingRestore();
    }
	public class EnvironmentSeedCommand : IEnvironmentSeedCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly AppSettings _settings;
		private readonly IDevCenterService _dev;
		private readonly IMarketplaceSupplierCommand _supplierCommand;
		private readonly IMarketplaceBuyerCommand _buyerCommand;
		private readonly ISchemaQuery _schemaQuery;
		private readonly IDocumentQuery _docQuery;

		private readonly string _buyerApiClientName = "Default HeadStart Buyer UI";
		private readonly string _buyerLocalApiClientName = "Default Marketplace Buyer UI LOCAL"; // used for pointing integration events to the ngrok url
		private readonly string _sellerApiClientName = "Default HeadStart Admin UI";
		private readonly string _integrationsApiClientName = "Middleware Integrations";
		private readonly string _sellerUserName = "Default_Admin";
		private readonly string _fullAccessSecurityProfile = "DefaultContext";

		public EnvironmentSeedCommand(
			AppSettings settings,
			IDevCenterService dev,
			IMarketplaceSupplierCommand supplierCommand,
			IMarketplaceBuyerCommand buyerCommand,
			ISchemaQuery schemaQuery,
			IDocumentQuery docQuery,
			IOrderCloudClient oc
		)
		{
			_settings = settings;
			_dev = dev;
			_supplierCommand = supplierCommand;
			_buyerCommand = buyerCommand;
			_schemaQuery = schemaQuery;
			_docQuery = docQuery;
			_oc = oc;
		}

		public async Task<string> Seed(EnvironmentSeed seed, VerifiedUserContext devUserContext)
		{
			await VerifyOrgExists(seed.SellerOrgID, devUserContext.AccessToken);
			var orgToken = await _dev.GetOrgToken(seed.SellerOrgID, devUserContext.AccessToken);

            await CreateDefaultSellerUser(orgToken);
            await CreateApiClients(orgToken);
            await CreateSecurityProfiles(seed, orgToken);
            await AssignSecurityProfiles(seed, orgToken);

            var apiClients = await GetApiClients(orgToken);
            await CreateWebhooks(apiClients.BuyerUiApiClient.ID, apiClients.AdminUiApiClient.ID, orgToken);
            await CreateMessageSenders(orgToken);
            await CreateIncrementors(orgToken); // must be before CreateBuyers
            await CreateBuyers(seed, orgToken);
            await CreateXPIndices(orgToken);
            await CreateAndAssignIntegrationEvents(apiClients.BuyerUiApiClient.ID, apiClients.BuyerLocalUiApiClient.ID, orgToken);

            var userContext = await GetVerifiedUserContext(apiClients.MiddlewareApiClient);
            await CreateSuppliers(userContext, seed, orgToken);
            await CreateContentDocSchemas(userContext);
            await CreateDefaultContentDocs(userContext);
			
			return orgToken;

		}

		public async Task VerifyOrgExists(string orgID, string devToken)
        {
			try
            {
				await _dev.GetOrganization(orgID, devToken);
            } catch
            {
				// the devcenter API no longer allows us to create an organization outside of devcenter
				// so we need to create the org first before seeding it
				throw new Exception("Failed to retrieve seller organization with SellerOrgID. The organization must exist before it can be seeded");
            }
        }

		public async Task PostStagingRestore()
		{	
			var token = (await _oc.AuthenticateAsync()).AccessToken;
			var apiClients = await GetApiClients(token);

			var deleteMS = DeleteAllMessageSenders(token);
			var deleteWH = DeleteAllWebhooks(token);
			var deleteIE = DeleteAllIntegrationEvents(apiClients.BuyerUiApiClient.ID, token);
			await Task.WhenAll(deleteMS, deleteWH, deleteIE);

			var createMS = CreateMessageSenders(token);
			var createWH = CreateWebhooks(apiClients.BuyerUiApiClient.ID, apiClients.AdminUiApiClient.ID, token);
			var createIE = CreateAndAssignIntegrationEvents(apiClients.BuyerUiApiClient.ID, apiClients.BuyerLocalUiApiClient.ID, token);
			await Task.WhenAll(createMS, createWH, createIE);
		}


		private async Task AssignSecurityProfiles(EnvironmentSeed seed, string orgToken)
		{
			// assign buyer security profiles
			var buyerSecurityProfileAssignmentRequests = seed.Buyers.Select(b =>
			{
				return _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment
				{
					BuyerID = b.ID,
					SecurityProfileID = CustomRole.MPBaseBuyer.ToString()
				}, orgToken);
			});
			await Task.WhenAll(buyerSecurityProfileAssignmentRequests);

			// assign seller security profiles to seller org
			var sellerSecurityProfileAssignmentRequests = SellerMarketplaceRoles.Select(role =>
			{
				return _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
				{
					SecurityProfileID = role.ToString()
				}, orgToken);
			});
			await Task.WhenAll(sellerSecurityProfileAssignmentRequests);
			
			// assign full access security profile to default admin user
			var defaultAdminUser = (await _oc.AdminUsers.ListAsync(accessToken: orgToken)).Items.First(u => u.Username == _sellerUserName);
			await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
			{
				SecurityProfileID = _fullAccessSecurityProfile,
				UserID = defaultAdminUser.ID
			}, orgToken);
		}

		private async Task CreateBuyers(EnvironmentSeed seed, string token) {
			seed.Buyers.Add(new MarketplaceBuyer
			{
				ID = "Default_HeadStart_Buyer",
				Name = "Default HeadStart Buyer",
				Active = true,
				xp = new BuyerXp
				{
					MarkupPercent = 0
				}
			});
			foreach (var buyer in seed.Buyers)
			{
				var superBuyer = new SuperMarketplaceBuyer()
				{
					Buyer = buyer,
					Markup = new BuyerMarkup() { Percent = 0 }
				};
				await _buyerCommand.Create(superBuyer, token);
			}
		}

		private async Task CreateSuppliers(VerifiedUserContext user, EnvironmentSeed seed, string token)
		{
			// Create Suppliers and necessary user groups and security profile assignments
			foreach (MarketplaceSupplier supplier in seed.Suppliers)
			{
				await _supplierCommand.Create(supplier, user, token);
			}
		}

		private async Task<VerifiedUserContext> GetVerifiedUserContext(ApiClient middlewareApiClient)
        {
			// some endpoints such as documents and documentschemas require a verified user context for a user in the seller org
			// however the context that we get when calling this endpoint is for the dev user so we need to create a user context
			// with the seller user
			var ocConfig = new OrderCloudClientConfig
			{
				ApiUrl = _settings.OrderCloudSettings.ApiUrl,
				AuthUrl = _settings.OrderCloudSettings.ApiUrl,
				ClientId = middlewareApiClient.ID,
				ClientSecret = middlewareApiClient.ClientSecret,
				GrantType = GrantType.ClientCredentials,
				Roles = new[]
				{
					ApiRole.FullAccess
				}
			};
			return await new VerifiedUserContext().Define(ocConfig);
		}

		private async Task CreateContentDocSchemas(VerifiedUserContext userContext)
        {
            var kitSchema = new DocSchema
            {
                ID = "KitProduct",
				RestrictedAssignmentTypes = new List<ResourceType> { },
				Schema = JObject.Parse(File.ReadAllText("../Marketplace.Common/Assets/ContentDocSchemas/kitproduct.json"))
			};

            var supplierFilterConfigSchema = new DocSchema
            {
                ID = "SupplierFilterConfig",
                RestrictedAssignmentTypes = new List<ResourceType> { },
                Schema = JObject.Parse(File.ReadAllText("../Marketplace.Common/Assets/ContentDocSchemas/supplierfilterconfig.json"))
			};

			await Task.WhenAll(
				_schemaQuery.Create(kitSchema, userContext),
				_schemaQuery.Create(supplierFilterConfigSchema, userContext)
			);
        }

		private async Task CreateDefaultContentDocs(VerifiedUserContext userContext)
        {
			// any default created docs should be generic enough to be used by all orgs
			await Task.WhenAll(
				_docQuery.Create("SupplierFilterConfig", GetCountriesServicingDoc(), userContext),
				_docQuery.Create("SupplierFilterConfig", GetServiceCategoryDoc(), userContext),
				_docQuery.Create("SupplierFilterConfig", GetVendorLevelDoc(), userContext)
			);
        }

		private SupplierFilterConfigDocument GetCountriesServicingDoc()
        {
			return new SupplierFilterConfigDocument
			{
				ID = "CountriesServicing",
				Doc = new SupplierFilterConfig
				{
					Display = "Countries Servicing",
					Path = "xp.CountriesServicing",
					Items = new List<Filter>
					{
						new Filter
						{
							Text = "UnitedStates",
							Value = "US"
						}
					},
					AllowSellerEdit = true,
					AllowSupplierEdit = true,
					BuyerAppFilterType  = "NonUI"
				}
			};
		}

		private dynamic GetServiceCategoryDoc()
		{
			return new SupplierFilterConfigDocument
			{
				ID = "ServiceCategory",
				Doc = new SupplierFilterConfig
				{
					Display = "Service Category",
					Path = "xp.Categories.ServiceCategory",
					AllowSupplierEdit = false,
					AllowSellerEdit = true,
					BuyerAppFilterType = "SelectOption",
					Items = new List<Filter>
					{

					}
				}
			};
		}

		private SupplierFilterConfigDocument GetVendorLevelDoc()
        {
			return new SupplierFilterConfigDocument
			{
				ID = "VendorLevel",
				Doc = new SupplierFilterConfig
				{
					Display = "Vendor Level",
					Path = "xp.Categories.VendorLevel",
					AllowSupplierEdit = true,
					AllowSellerEdit = true,
					BuyerAppFilterType = "SelectOption",
					Items = new List<Filter>
					{

					}
				}
			};
		}

		private async Task CreateDefaultSellerUser(string token)
        {
			var defaultSellerUser = new User
			{
				ID = "Default_Admin",
				Username = _sellerUserName,
				Email = "test@test.com",
				Active = true,
				FirstName = "Default",
				LastName = "User"
			};
			await _oc.AdminUsers.CreateAsync(defaultSellerUser, token);
		}

		static readonly List<XpIndex> DefaultIndices = new List<XpIndex>() {
			new XpIndex { ThingType = XpThingType.UserGroup, Key = "Type" },       
			new XpIndex { ThingType = XpThingType.UserGroup, Key = "Role" },
			new XpIndex { ThingType = XpThingType.UserGroup, Key = "Country" },
			new XpIndex { ThingType = XpThingType.Company, Key = "Data.ServiceCategory" },       
			new XpIndex { ThingType = XpThingType.Company, Key = "Data.VendorLevel" },       
			new XpIndex { ThingType = XpThingType.Company, Key = "SyncFreightPop" },       
			new XpIndex { ThingType = XpThingType.Company, Key = "CountriesServicing" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "NeedsAttention" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "StopShipSync" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "OrderType" },       
			new XpIndex { ThingType = XpThingType.Order, Key = "LocationID" },       
			new XpIndex { ThingType = XpThingType.User, Key = "UserGroupID" },
			new XpIndex { ThingType = XpThingType.User, Key = "RequestInfoEmails" },       
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

		private async Task<ApiClientIDs> GetApiClients(string token)
		{
			var list = await _oc.ApiClients.ListAsync(pageSize: 100, accessToken: token);
			var adminUIApiClient = list.Items.First(a => a.AppName == _sellerApiClientName);
			var buyerUIApiClient = list.Items.First(a => a.AppName == _buyerApiClientName);
			var buyerLocalUIApiClient = list.Items.First(a => a.AppName == _buyerLocalApiClientName);
			var middlewareApiClient = list.Items.First(a => a.AppName == _integrationsApiClientName);
			return new ApiClientIDs()
			{
				AdminUiApiClient = adminUIApiClient,
				BuyerUiApiClient = buyerUIApiClient,
				BuyerLocalUiApiClient = buyerLocalUIApiClient,
				MiddlewareApiClient = middlewareApiClient
			};
		}

		public class ApiClientIDs
        {
			public ApiClient AdminUiApiClient { get; set; }
			public ApiClient BuyerUiApiClient { get; set; }
			public ApiClient BuyerLocalUiApiClient { get; set; }
			public ApiClient MiddlewareApiClient { get; set; }
        }

		private async Task CreateApiClients(string token)
		{
			var allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var integrationsClient = new ApiClient()
			{
				AppName = _integrationsApiClientName,
				Active = true,
				AllowAnyBuyer = false,
				AllowAnySupplier = false,
				AllowSeller = true,
				AccessTokenDuration = 600,
				RefreshTokenDuration = 43200,
				DefaultContextUserName = _sellerUserName,
				ClientSecret = RandomGen.GetString(allowedChars, 60)
			};
			var sellerClient = new ApiClient()
			{
				AppName = _sellerApiClientName,
				Active = true,
				AllowAnyBuyer = false,
				AllowAnySupplier = true,
				AllowSeller = true,
				AccessTokenDuration = 600,
				RefreshTokenDuration = 43200
			};
			var buyerClient = new ApiClient()
			{
				AppName = _buyerApiClientName,
				Active = true,
				AllowAnyBuyer = true,
				AllowAnySupplier = false,
				AllowSeller = false,
				AccessTokenDuration = 600,
				RefreshTokenDuration = 43200
			};
			var buyerLocalClient = new ApiClient()
			{
				AppName = _buyerLocalApiClientName,
				Active = true,
				AllowAnyBuyer = true,
				AllowAnySupplier = false,
				AllowSeller = false,
				AccessTokenDuration = 600,
				RefreshTokenDuration = 43200
			};

			var integrationsClientRequest = _oc.ApiClients.CreateAsync(integrationsClient, token);
			var sellerClientRequest = _oc.ApiClients.CreateAsync(sellerClient, token);
			var buyerClientRequest = _oc.ApiClients.CreateAsync(buyerClient, token);
			var buyerLocalClientRequest = _oc.ApiClients.CreateAsync(buyerLocalClient, token);

			await Task.WhenAll(integrationsClientRequest, sellerClientRequest, buyerClientRequest, buyerLocalClientRequest);
		}

		private async Task CreateMessageSenders(string accessToken)
		{
			foreach (var messageSender in DefaultMessageSenders)
			{
				messageSender.URL = $"{_settings.EnvironmentSettings.BaseUrl}{messageSender.URL}";
				await _oc.MessageSenders.CreateAsync(messageSender, accessToken);
			}
		}
		private async Task CreateAndAssignIntegrationEvents(string buyerUiApiClientID, string buyerLocalUiApiClientID, string token)
		{
			await _oc.IntegrationEvents.CreateAsync(new IntegrationEvent()
			{
				ElevatedRoles = new [] { ApiRole.FullAccess },
				ID = "freightpopshipping",
				EventType = IntegrationEventType.OrderCheckout,
				CustomImplementationUrl = _settings.EnvironmentSettings.BaseUrl,
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
				CustomImplementationUrl = "https://marketplaceteam.ngrok.io", // local webhook url
				Name = "FreightPOP Shipping LOCAL",
				HashKey = _settings.OrderCloudSettings.WebhookHashKey,
				ConfigData = new
				{
					ExcludePOProductsFromShipping = true,
					ExcludePOProductsFromTax = true,
				}
			}, token);
			await _oc.ApiClients.PatchAsync(buyerUiApiClientID, new PartialApiClient { OrderCheckoutIntegrationEventID = "freightpopshipping" }, token);
			await _oc.ApiClients.PatchAsync(buyerLocalUiApiClientID, new PartialApiClient { OrderCheckoutIntegrationEventID = "freightpopshippingLOCAL" }, token);
		}

		public async Task CreateSecurityProfiles(EnvironmentSeed seed, string accessToken)
		{
			var profiles = DefaultSecurityProfiles.Select(p =>
				new SecurityProfile()
				{
					Name = p.CustomRole.ToString(),
					ID = p.CustomRole.ToString(),
					CustomRoles = p.CustomRoles == null ? new List<string>() { p.CustomRole.ToString() } : p.CustomRoles.Append(p.CustomRole).Select(r => r.ToString()).ToList(),
					Roles = p.Roles
				}).ToList();

			profiles.Add(new SecurityProfile()
			{
				Roles = new List<ApiRole> { ApiRole.FullAccess },
				Name = _fullAccessSecurityProfile,
				ID = _fullAccessSecurityProfile
			});

			var profileCreateRequests = profiles.Select(p => _oc.SecurityProfiles.CreateAsync(p, accessToken));
			await Task.WhenAll(profileCreateRequests);
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
				_oc.MessageSenders.DeleteAsync(messageSender.ID, token));
		}

		public async Task DeleteAllIntegrationEvents(string buyerUiApiClientID, string token)
		{
			await _oc.ApiClients.PatchAsync(buyerUiApiClientID, new PartialApiClient { OrderCheckoutIntegrationEventID = null }, token);
			var integrationEvents = await _oc.IntegrationEvents.ListAsync(pageSize: 100, accessToken: token);
			await Throttler.RunAsync(integrationEvents.Items, 500, 20, integrationEvent =>
				_oc.IntegrationEvents.DeleteAsync(integrationEvent.ID, token));
		}

		private async Task CreateWebhooks(string buyerUiApiClientID, string adminUiApiClientID, string accessToken)
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
			  ApiClientIDs = new [] { buyerUiApiClientID }
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
			  ApiClientIDs = new [] { buyerUiApiClientID }
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
			  ApiClientIDs = new [] { adminUiApiClientID }
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
			  ApiClientIDs = new [] { adminUiApiClientID }
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
			  ApiClientIDs = new [] { buyerUiApiClientID, adminUiApiClientID }
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
			  ApiClientIDs = new [] { adminUiApiClientID }
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
			  ApiClientIDs = new [] { adminUiApiClientID }
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
			 ApiClientIDs = new [] { adminUiApiClientID }
			},
		};
			foreach (Webhook webhook in DefaultWebhooks)
			{
				webhook.Url = $"{_settings.EnvironmentSettings.BaseUrl}{webhook.Url}";
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
			},
			new MessageSender()
			{
				Name = "New User Registration",
				MessageTypes = new[] { MessageType.NewUserInvitation },
				URL = "/newuser",
				SharedKey = "wkaWSxPBBAABaxEp", // Where does this come from? Should it live somewhere else?
				xp = new {
						MessageTypeConfig = new {
							MessageType = "NewUserInvitation",
							FromEmail = "noreply@ordercloud.io",
							Subject = "New User Registration",
							TemplateName = "ForgottenPassword",
							MainContent = "NewUserInvitation"
						}
				}
			}
		};

		static readonly List<MarketplaceSecurityProfile> DefaultSecurityProfiles = new List<MarketplaceSecurityProfile>() {
			
			// seller/supplier
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeProductAdmin, Roles = new[] { ApiRole.ProductAdmin, ApiRole.PriceScheduleAdmin, ApiRole.InventoryAdmin, ApiRole.ProductFacetReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeProductReader, Roles = new[] { ApiRole.ProductReader, ApiRole.PriceScheduleReader, ApiRole.ProductFacetReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPProductAdmin, Roles = new[] { ApiRole.ProductAdmin, ApiRole.CatalogAdmin, ApiRole.ProductAssignmentAdmin, ApiRole.ProductFacetAdmin, ApiRole.AdminAddressReader, ApiRole.PriceScheduleAdmin  } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPProductReader, Roles = new[] { ApiRole.ProductReader, ApiRole.CatalogReader, ApiRole.ProductFacetReader} },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPPromotionAdmin, Roles = new[] { ApiRole.PromotionAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPContentAdmin, CustomRoles = new[] { CustomRole.AssetAdmin, CustomRole.SchemaAdmin, CustomRole.DocumentAdmin, } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPPromotionReader, Roles = new[] { ApiRole.PromotionReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPStoreFrontAdmin, Roles = new[] { ApiRole.ProductFacetAdmin, ApiRole.ProductFacetReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPCategoryAdmin, Roles = new[] { ApiRole.CategoryAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPCategoryReader, Roles = new[] { ApiRole.CategoryReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPOrderAdmin, Roles = new[] { ApiRole.OrderAdmin, ApiRole.ShipmentReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPOrderReader, Roles = new[] { ApiRole.OrderReader, ApiRole.ShipmentAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPShipmentAdmin, Roles = new[] { ApiRole.OrderReader, ApiRole.ShipmentAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPBuyerAdmin, Roles = new[] { ApiRole.BuyerAdmin, ApiRole.BuyerUserAdmin, ApiRole.UserGroupAdmin, ApiRole.AddressAdmin, ApiRole.CreditCardAdmin, ApiRole.ApprovalRuleAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPBuyerReader, Roles = new[] { ApiRole.BuyerReader, ApiRole.BuyerUserReader, ApiRole.UserGroupReader, ApiRole.AddressReader, ApiRole.CreditCardReader, ApiRole.ApprovalRuleReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPSellerAdmin, Roles = new[] { ApiRole.AdminUserAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPSupplierAdmin, Roles = new[] { ApiRole.SupplierAdmin, ApiRole.SupplierUserAdmin, ApiRole.SupplierAddressAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeSupplierAdmin, Roles = new[] {ApiRole.SupplierReader, ApiRole.SupplierAdmin }, CustomRoles = new[] { CustomRole.AssetAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeSupplierAddressAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierAddressAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPMeSupplierUserAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierUserAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPSupplierUserGroupAdmin, Roles = new[] { ApiRole.SupplierReader, ApiRole.SupplierUserGroupAdmin } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPReportReader, CustomRoles = new[] { CustomRole.MPReportReader } },
			new MarketplaceSecurityProfile() { CustomRole = CustomRole.MPReportAdmin, CustomRoles = new[] { CustomRole.MPReportAdmin } },
			
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

		static readonly List<CustomRole> SellerMarketplaceRoles = new List<CustomRole>() {
			CustomRole.MPProductAdmin,
			CustomRole.MPPromotionAdmin,
			CustomRole.MPStoreFrontAdmin,
			CustomRole.MPCategoryAdmin,
			CustomRole.MPOrderAdmin,
			CustomRole.MPShipmentAdmin,
			CustomRole.MPBuyerAdmin,
			CustomRole.MPSellerAdmin,
			CustomRole.MPSupplierAdmin,
			CustomRole.MPSupplierUserGroupAdmin,
			CustomRole.MPReportReader,
			CustomRole.MPReportAdmin,
			CustomRole.MPContentAdmin
		};
	}
}
