using Marketplace.Models;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using System.Linq;
using System.Dynamic;
using System;
using Marketplace.Common.Commands.Crud;
using ordercloud.integrations.library.Cosmos;
using System.Collections.Generic;
using Marketplace.Common.Extensions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Services.CMS;
using Marketplace.Common.Services.CMS.Models;

namespace Marketplace.Common.Commands
{
    public interface INotificationCommand
    {
        Task<SuperMarketplaceProduct> CreateModifiedMonitoredSuperProductNotification(MonitoredProductFieldModifiedNotification notification, VerifiedUserContext user);
        Task<SuperMarketplaceProduct> UpdateMonitoredSuperProductNotificationStatus(Document<MonitoredProductFieldModifiedNotification> document, string supplierID, string productID, VerifiedUserContext user);
        Task<ListPage<Document<MonitoredProductFieldModifiedNotification>>> ReadMonitoredSuperProductNotificationList(SuperMarketplaceProduct product, VerifiedUserContext user);
    }
    public class NotificationCommand : INotificationCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        private readonly ICMSClient _cms;
        private readonly IMarketplaceProductCommand _productCommand;
        private readonly ISupplierApiClientHelper _apiClientHelper;
        private readonly string _documentSchemaID = "MonitoredProductFieldModifiedNotification";

        public NotificationCommand(IOrderCloudClient oc, AppSettings settings, ICMSClient cms, IMarketplaceProductCommand productCommand, ISupplierApiClientHelper apiClientHelper)
        {
            _oc = oc;
            _settings = settings;
            _cms = cms;
            _productCommand = productCommand;
            _apiClientHelper = apiClientHelper;
        }
        public async Task<SuperMarketplaceProduct> CreateModifiedMonitoredSuperProductNotification(MonitoredProductFieldModifiedNotification notification, VerifiedUserContext user)
        {
            if (notification == null || notification?.Product == null) { throw new Exception("Unable to process notification with no product"); }
            var _product = await _oc.Products.PatchAsync(notification.Product.ID, new PartialProduct { Active = false }, user.AccessToken);
            var document = new Document<MonitoredProductFieldModifiedNotification>();
            document.Doc = notification;
            document.ID = $"{notification.Product.ID}_{CosmosInteropID.New()}";
            // Create notifictaion in the cms
            await _cms.Documents.Create(_documentSchemaID, document, await GetAdminToken());
            // Assign the notification to the product
            // TODO: this doesn't work because need to own thing being assigned to AND have DocumentAdmin and we don't want to give suppliers DocumentAdmin
            // await _cms.Documents.SaveAssignment("MonitoredProductFieldModifiedNotification", new DocumentAssignment() { DocumentID = document.ID, ResourceType = ResourceType.Products, ResourceID = _product.ID }, user.AccessToken);
            return await _productCommand.Get(_product.ID, user.AccessToken);
        }

        public async Task<ListPage<Document<MonitoredProductFieldModifiedNotification>>> ReadMonitoredSuperProductNotificationList(SuperMarketplaceProduct product, VerifiedUserContext user)
        {
            var token = await GetAdminToken();
            ListArgs<Document<MonitoredProductFieldModifiedNotification>> args;

            args = new ListArgs<Document<MonitoredProductFieldModifiedNotification>>()
            {
                Search = $"{product.Product.ID}",
                SearchOn = "ID,Name",
                PageSize = 100
            };

            var document = await _cms.Documents.List(_documentSchemaID, args, token);

            return document;
        }

        public async Task<SuperMarketplaceProduct> UpdateMonitoredSuperProductNotificationStatus(Document<MonitoredProductFieldModifiedNotification> document, string supplierID, string productID, VerifiedUserContext user)
        {
            MarketplaceProduct product = null;
            var token = await GetAdminToken();
            try
            {
                product = await _oc.Products.GetAsync<MarketplaceProduct>(productID);

            }
            catch (OrderCloudException ex)
            {
                //Product was deleted after it was updated. Delete orphaned notification 
               if (ex.HttpStatus == System.Net.HttpStatusCode.NotFound)
                {
                    await _cms.Documents.Delete(_documentSchemaID, document.ID, token);
                    return new SuperMarketplaceProduct();
                }
            }
            if (document.Doc.Status == NotificationStatus.ACCEPTED)
            {   
                product = await _oc.Products.PatchAsync<MarketplaceProduct>(productID, new PartialProduct() { Active = true }, user.AccessToken);

                //Delete document after acceptance
                await _cms.Documents.Delete(_documentSchemaID, document.ID, token);
            }
            else
            {
                await _cms.Documents.Save(_documentSchemaID, document.ID, document, token);
            }
            var superProduct = await _productCommand.Get(productID, token);
            superProduct.Product = product;
            return superProduct;
        }

        private async Task<string> GetAdminToken()
        {
            var adminOcToken = _oc.TokenResponse?.AccessToken;
            if (adminOcToken == null || DateTime.UtcNow > _oc.TokenResponse.ExpiresUtc)
            {
                await _oc.AuthenticateAsync();
                adminOcToken = _oc.TokenResponse.AccessToken;
            }
            return adminOcToken;
        }
    }
}
