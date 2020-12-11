﻿using Marketplace.Models;
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
    }
    public class NotificationCommand : INotificationCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        private readonly ICMSClient _cms;
        private readonly IMarketplaceProductCommand _productCommand;
        private readonly ISupplierApiClientHelper _apiClientHelper;

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
            // Make the Product.Active = false;
            var _product = await _oc.Products.PatchAsync(notification.Product.ID, new PartialProduct { Active = false }, user.AccessToken);
            var document = new Document<MonitoredProductFieldModifiedNotification>();
            document.Doc = notification;
            document.ID = CosmosInteropID.New();
            // Create notifictaion in the cms
            await _cms.Documents.Create("MonitoredProductFieldModifiedNotification", document, user.AccessToken);
            // Assign the notification to the product
            await _cms.Documents.SaveAssignment("MonitoredProductFieldModifiedNotification", new DocumentAssignment() { DocumentID = document.ID, ResourceType = ResourceType.Products, ResourceID = _product.ID }, user.AccessToken);
            return await _productCommand.Get(_product.ID, user.AccessToken);
        }
        public async Task<SuperMarketplaceProduct> UpdateMonitoredSuperProductNotificationStatus(Document<MonitoredProductFieldModifiedNotification> document, string supplierID, string productID, VerifiedUserContext user)
        {
            var product = await _oc.Products.GetAsync<MarketplaceProduct>(productID);
            if (document.Doc.Status == NotificationStatus.ACCEPTED)
            {
                var supplierClient = await _apiClientHelper.GetSupplierApiClient(supplierID, user.AccessToken);
                if (supplierClient == null) { throw new Exception($"Default supplier client not found. SupplierID: {supplierID}, ProductID: {productID}"); }

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
                product = await ocClient.Products.PatchAsync<MarketplaceProduct>(productID, new PartialProduct() { Active = true }, token);

                //Delete document after acceptance
                await _cms.Documents.Delete("MonitoredProductFieldModifiedNotification", document.ID, user.AccessToken);
            }
            await _cms.Documents.Save("MonitoredProductFieldModifiedNotification", document.ID, document, user.AccessToken);
            var superProduct = await _productCommand.Get(productID, user.AccessToken);
            superProduct.Product = product;
            return superProduct;
        }
    }
}
