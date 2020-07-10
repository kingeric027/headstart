using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Dynamitey;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Extended;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Marketplace.Common.Services
{
    public interface ISendgridService
    {
        Task SendSingleEmail(string from, string to, string subject, string htmlContent);
        Task SendSingleTemplateEmail(string from, string to, string templateID, object templateData);
        Task SendOrderSupplierEmails(MarketplaceOrderWorksheet orderWorksheet, string templateID, object templateData);
        Task SendOrderSubmitEmail(MarketplaceOrderWorksheet orderData);
        Task SendReturnRequestedEmail(string orderID);
        Task SendNewUserEmail(MessageNotification<PasswordResetEventBody> payload);
        Task SendOrderRequiresApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification);
        Task SendPasswordResetEmail(MessageNotification<PasswordResetEventBody> messageNotification);
        Task SendOrderSubmittedForApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification);
        Task SendOrderApprovedEmail(MarketplaceOrderApprovePayload payload);
        Task SendOrderDeclinedEmail(MarketplaceOrderDeclinePayload payload);
    }
    public class SendgridService : ISendgridService
    {
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;
        private const string NO_REPLY_EMAIL_ADDRESS = "noreply@four51.com";
        private const string BUYER_ORDER_SUBMIT_TEMPLATE_ID = "d-defb11ada55d48d8a38dc1074eaaca67";
        private const string SUPPLIER_ORDER_SUBMIT_TEMPLATE_ID = "d-777af54b1e414b0b853f983697889267";
        private const string BUYER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-3266ef3d70b54d78a74aaf012eaf5e64";
        private const string SUPPLIER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-5776a6c57b344aeda605444c96ff39e8";
        private const string BUYER_NEW_USER_TEMPLATE_ID = "d-f3831baa2beb4c19aeace19e48132768";
        private const string BUYER_PASSWORD_RESET_TEMPLATE_ID = "d-ca6a6ff8c9ac4264bf86b5d6cdd3a038";
        private const string BUYER_ORDER_SUBMITTED_FOR_APPROVAL_TEMPLATE_ID = "d-4c674afcd6ef44e9b7793eb6c5b917ea";
        private const string BUYER_ORDER_APPROVED_TEMPLATE_ID = "d-2f3b92b95b7b45ea8f8fb94c8ac928e0";
        private const string BUYER_ORDER_DECLINED_TEMPLATE_ID = "d-3b6167f40d6b407b95759d1cb01fff30";
        private const string ORDER_REQUIRES_APPROVAL_TEMPLATE_ID = "d-fbe9f4e9fabd4a37ba2364201d238316";
        private const string BUYER_REFUND_REQUESTED_TEMPLATE_ID = "d-e7327df33cd4412abaa2fa76a67f0f3e";
        public SendgridService(AppSettings settings, IOrderCloudClient ocClient)
        {
            _oc = ocClient;
            _settings = settings;
        }

        public async Task SendSingleEmail(string from, string to, string subject, string htmlContent) //temp function until all endpoints are accessible for template data
        {
            var client = new SendGridClient(_settings.SendgridApiKey);
            var fromEmail = new EmailAddress(from);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, null, htmlContent);
            await client.SendEmailAsync(msg);
        }

        public async Task SendSingleTemplateEmail(string from, string to, string templateID, object templateData)
        {
            var client = new SendGridClient(_settings.SendgridApiKey);
            var fromEmail = new EmailAddress(from);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleTemplateEmail(fromEmail, toEmail, templateID, templateData);
            await client.SendEmailAsync(msg);
        }

        public async Task SendPasswordResetEmail(MessageNotification<PasswordResetEventBody> messageNotification)
        {
            var templateData = new
            {
                messageNotification.Recipient.FirstName,
                messageNotification.Recipient.LastName,
                messageNotification.EventBody.PasswordRenewalAccessToken,
                messageNotification.EventBody.PasswordRenewalUrl
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, messageNotification.Recipient.Email, BUYER_PASSWORD_RESET_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderSubmittedForApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification)
        {
            var order = messageNotification.EventBody.Order;
            var templateData = GetOrderTemplateData(order, messageNotification.EventBody.LineItems);
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, messageNotification.Recipient.Email, BUYER_ORDER_SUBMITTED_FOR_APPROVAL_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderRequiresApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification)
        {
            var templateData = new
            {
                messageNotification.Recipient.FirstName,
                messageNotification.Recipient.LastName,
                OrderID = messageNotification.EventBody.Order.ID
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, messageNotification.Recipient.Email, ORDER_REQUIRES_APPROVAL_TEMPLATE_ID, templateData);
        }

        public async Task SendNewUserEmail(MessageNotification<PasswordResetEventBody> messageNotification)
        {
            string BaseAppURL = _settings.UI.BaseAdminUrl;
            var jwt = messageNotification.EventBody.PasswordRenewalAccessToken;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var cid = token.Claims.FirstOrDefault(c => c.Type == "cid");
            var apiClient = await _oc.ApiClients.GetAsync(cid.Value);
            // Overwrite with the buyer base url if token appname contans 'buyer'
            if (apiClient.AppName.ToLower().Contains("buyer"))
            {
                BaseAppURL = _settings.UI.BaseBuyerUrl;
            }
            var templateData = new
            {
                messageNotification.Recipient.FirstName,
                messageNotification.Recipient.LastName,
                messageNotification.EventBody.PasswordRenewalAccessToken,
                BaseAppURL,
                messageNotification.EventBody.Username
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, messageNotification.Recipient.Email, BUYER_NEW_USER_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderApprovedEmail(MarketplaceOrderApprovePayload payload)
        {
            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, payload.Response.Body.ID);
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, payload.Response.Body.FromUser.Email, BUYER_ORDER_APPROVED_TEMPLATE_ID, GetOrderTemplateData(payload.Response.Body, lineItems.Items));
        }

        public async Task SendOrderDeclinedEmail(MarketplaceOrderDeclinePayload payload)
        {
            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, payload.Response.Body.ID);
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, payload.Response.Body.FromUser.Email, BUYER_ORDER_DECLINED_TEMPLATE_ID, GetOrderTemplateData(payload.Response.Body, lineItems.Items));
        }

        public async Task SendOrderSubmitEmail(MarketplaceOrderWorksheet orderWorksheet)
        {
            if (orderWorksheet.Order.xp.OrderType == OrderType.Standard)
            {
                await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, orderWorksheet.Order.FromUser.Email, BUYER_ORDER_SUBMIT_TEMPLATE_ID, GetOrderTemplateData(orderWorksheet.Order, orderWorksheet.LineItems));
                await SendSupplierOrderSubmitEmail(orderWorksheet);
            }
            else if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                var dynamicTemplateData = new
                {
                    orderWorksheet.Order.FromUser.FirstName,
                    orderWorksheet.Order.FromUser.LastName
                };
                await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, orderWorksheet.Order.FromUser.Email, BUYER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
                await SendSupplierOrderSubmitEmail(orderWorksheet);
            }
        }

        public async Task SendReturnRequestedEmail(string orderID)
        {
            MarketplaceOrder order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, orderID);
            var productsList = lineItems.Items.Select(MapReturnedLineItemToProduct);
            var dynamicTemplateData = new {
            order.FromUser.FirstName,
            order.FromUser.LastName,
            order.ID,
            DateSubmitted = order.DateSubmitted.ToString(),
            ReturnID = order.xp.OrderReturnInfo.RMANumber,
            Products = productsList
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, order.FromUser.Email, BUYER_REFUND_REQUESTED_TEMPLATE_ID, dynamicTemplateData);
            // TODO: Get Seller's info for return requested email
        }

        public async Task SendSupplierOrderSubmitEmail(MarketplaceOrderWorksheet orderWorksheet)
        {
            if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, orderWorksheet.Order.ID);
                var supplierList = GetSupplierInfo(lineItems);
                foreach (string supplier in supplierList)
                {
                    MarketplaceSupplier supplierInfo = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplier);
                    var quoteOrderData = new { supplierInfo.xp.SupportContact.Name };
                    await SendOrderSupplierEmails(orderWorksheet, SUPPLIER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID, quoteOrderData);
                }
            }
            else
            {
                await SendOrderSupplierEmails(orderWorksheet, SUPPLIER_ORDER_SUBMIT_TEMPLATE_ID, GetOrderTemplateData(orderWorksheet.Order, orderWorksheet.LineItems));
            }
        }

        public async Task SendOrderSupplierEmails(MarketplaceOrderWorksheet orderWorksheet, string templateID, object templateData)
        {
            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, orderWorksheet.Order.ID);
            var supplierList = GetSupplierInfo(lineItems);
            foreach (string supplier in supplierList)
            {
                MarketplaceSupplier supplierInfo = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplier);
                // the email that will be sent a notification of the email for the supplier may not be found on xp.Supportcontact in the future
                var emailRecipient = supplierInfo.xp.SupportContact.Email;
                if (emailRecipient.Length > 0 && orderWorksheet.Order.xp.OrderType == OrderType.Quote)
                {
                    var quoteOrderData = new { supplierInfo.xp.SupportContact.Name };
                    await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, emailRecipient, templateID, quoteOrderData);
                }
                else if (emailRecipient.Length > 0)
                {
                    await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, emailRecipient, templateID, templateData);
                }
            };
        }

        // helper functions 
        private List<string> GetSupplierInfo(ListPage<MarketplaceLineItem> lineItems)
        {
            var supplierList = lineItems.Items.Select(item => item.SupplierID)
                .Distinct()
                .ToList();
            return supplierList;
        }

        private object GetOrderTemplateData(MarketplaceOrder order, IList<MarketplaceLineItem> lineItems)
        {
            var productsList = lineItems.Select(MapLineItemToProduct);
            var shippingAddress = GetShippingAddress(lineItems);
            return new
            {
                order.FromUser.FirstName,
                order.FromUser.LastName,
                OrderID = order.ID,
                DateSubmitted = order.DateSubmitted.ToString(),
                order.ShippingAddressID,
                ShippingAddress = shippingAddress,
                order.BillingAddressID,
                BillingAddress = new
                {
                    order.BillingAddress.Street1,
                    order.BillingAddress.Street2,
                    order.BillingAddress.City,
                    order.BillingAddress.State,
                    order.BillingAddress.Zip
                },
                Products = productsList,
                order.Subtotal,
                order.TaxCost,
                order.ShippingCost,
                PromotionalDiscount = order.PromotionDiscount,
                order.Total
            };
        }

        private object MapReturnedLineItemToProduct(MarketplaceLineItem lineItem) =>
        new
        {
            ProductName = lineItem.Product.Name,
            ImageURL = lineItem.xp.LineItemImageUrl,
            lineItem.ProductID,
            lineItem.Quantity,
            lineItem.LineTotal,
            ReturnQuantity = lineItem.xp.LineItemReturnInfo.QuantityToReturn,
            Comments = lineItem.xp.LineItemReturnInfo.ReturnReason
        };

        private object MapLineItemToProduct(MarketplaceLineItem lineItem) =>
          new
          {
              ProductName = lineItem.Product.Name,
              ImageURL = lineItem.xp.LineItemImageUrl,
              lineItem.ProductID,
              lineItem.Quantity,
              lineItem.LineTotal,
          };

        private object GetShippingAddress(IList<MarketplaceLineItem> lineItems) =>
          new
          {
              lineItems[0].ShippingAddress.Street1,
              lineItems[0].ShippingAddress.Street2,
              lineItems[0].ShippingAddress.City,
              lineItems[0].ShippingAddress.State,
              lineItems[0].ShippingAddress.Zip
          };
    }
}