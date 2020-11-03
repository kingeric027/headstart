using Marketplace.Models;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using System.Linq;
using ordercloud.integrations.cms;
using System.Dynamic;
using System;
using Marketplace.Common.Commands.Crud;
using ordercloud.integrations.library.Cosmos;
using System.Collections.Generic;
using Marketplace.Common.Extensions;

namespace Marketplace.Common.Commands
{
    public interface INotificationCommand
    {
        Task<SuperMarketplaceProduct> CreateModifiedMonitoredSuperProductNotification(MonitoredProductFieldModifiedNotification notification, VerifiedUserContext user);
        Task<SuperMarketplaceProduct> UpdateMonitoredSuperProductNotificationStatus(MonitoredProductFieldModifiedNotificationDocument document, string supplierID, string productID, VerifiedUserContext user);
    }
    public class NotificationCommand : INotificationCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        private readonly IDocumentQuery _query;
        private readonly IDocumentAssignmentQuery _assignmentQuery;
        private readonly IMarketplaceProductCommand _productCommand;

        public NotificationCommand(IOrderCloudClient oc, AppSettings settings, IDocumentQuery query, IDocumentAssignmentQuery assignmentQuery, IMarketplaceProductCommand productCommand)
        {
            _oc = oc;
            _settings = settings;
            _query = query;
            _assignmentQuery = assignmentQuery;
            _productCommand = productCommand;
        }
        public async Task<SuperMarketplaceProduct> CreateModifiedMonitoredSuperProductNotification(MonitoredProductFieldModifiedNotification notification, VerifiedUserContext user)
        {
            // Make the Product.Active = false;
            var _product = await _oc.Products.PatchAsync(notification.Product.ID, new PartialProduct { Active = false }, user.AccessToken);
            var document = new MonitoredProductFieldModifiedNotificationDocument();
            document.Doc = notification;
            document.ID = CosmosInteropID.New();
            // Create notifictaion in the cms
            await _query.Create<MonitoredProductFieldModifiedNotification>("MonitoredProductFieldModifiedNotification", document, user);
            // Assign the notification to the product
            await _assignmentQuery.SaveAssignment("MonitoredProductFieldModifiedNotification", new DocumentAssignment() { DocumentID = document.ID, ResourceType = ResourceType.Products, ResourceID = _product.ID }, user);
            return await _productCommand.Get(_product.ID, user);
        }
        public async Task<SuperMarketplaceProduct> UpdateMonitoredSuperProductNotificationStatus(MonitoredProductFieldModifiedNotificationDocument document, string supplierID, string productID, VerifiedUserContext user)
        {
            var product = await _oc.Products.GetAsync<MarketplaceProduct>(productID);
            if (document.Doc.Status == NotificationStatus.ACCEPTED)
            {
                //Use supplier integrations client with a DefaultContextUserName to access a supplier token.  
                //All suppliers have integration clients with a default user of dev_{supplierID}.
                var assignments = await _oc.ApiClients.ListAssignmentsAsync(supplierID: supplierID);

                if (!assignments.Items.HasItem()) { throw new Exception($"Integration Client default user not found. SupplierID: {supplierID}, ProductID: {productID}"); }

                ApiClient supplierClient = await _oc.ApiClients.GetAsync(assignments.Items[0].ApiClientID);

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
            }
            await _query.Save<MonitoredProductFieldModifiedNotification>("MonitoredProductFieldModifiedNotification", document.ID, document, user);
            var superProduct = await _productCommand.Get(productID, user);
            superProduct.Product = product;
            return superProduct;
        }
    }
}
