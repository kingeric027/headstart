using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
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
        Task SendSupplierEmails(OrderWorksheet orderWorksheet, string templateID, object templateData);
        Task SendOrderSubmitEmail(OrderWorksheet orderData);
        Task SendOrderApprovalEmails(MessageNotification payload);
        Task SendNewUserEmail(WebhookPayloads.Users.Create payload);
    }
    public class SendgridService : ISendgridService
    {
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;
        private const string NO_REPLY_EMAIL = "noreply @four51.com";
        private const string BUYER_ORDER_SUBMIT_TEMPLATE_ID = "d-defb11ada55d48d8a38dc1074eaaca67";
        private const string SUPPLIER_ORDER_SUBMIT_TEMPLATE_ID = "d-777af54b1e414b0b853f983697889267";
        private const string BUYER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-3266ef3d70b54d78a74aaf012eaf5e64";
        private const string SUPPLIER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-5776a6c57b344aeda605444c96ff39e8";
        private const string BUYER_FORGOT_PASS_TEMPLATE_ID = "d-ca6a6ff8c9ac4264bf86b5d6cdd3a038";
        private const string BUYER_ORDER_SENT_FOR_APPROVAL_TEMPLATE_ID = "d-4c674afcd6ef44e9b7793eb6c5b917ea";
        private const string BUYER_NEW_USER_TEMPLATE_ID = "d-f3831baa2beb4c19aeace19e48132768";
        private const string ADMIN_NEW_USER_TEMPLATE_ID = "d-f50756b6e0c444398ab0e8ba939e7372";
        public SendgridService(AppSettings settings, IOrderCloudClient ocClient)
        {
            _oc = ocClient;
            _settings = settings;
        }
        public async Task SendSingleEmail(string from, string to, string subject, string htmlContent)
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

        public async Task SendSupplierEmails(OrderWorksheet orderWorksheet, string templateID, object templateData)
        {
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderWorksheet.Order.ID);
            var supplierList = GetSupplierInfo(lineItems);
            foreach (string supplier in supplierList)
            {
                MarketplaceSupplier supplierInfo = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplier);
                // the email that will be sent a notification of the email for the supplier may not be found on xp.Supportcontact in the future
                var emailRecipient = supplierInfo.xp.SupportContact.Email;
                if (emailRecipient.Length > 0 && orderWorksheet.Order.xp.OrderType == OrderType.Quote)
                {
                    var quoteOrderData = new { supplierInfo.xp.SupportContact.Name };
                    await SendSingleTemplateEmail(NO_REPLY_EMAIL, emailRecipient, templateID, quoteOrderData);
                } else if (emailRecipient.Length > 0)
                {
                await SendSingleTemplateEmail(NO_REPLY_EMAIL, emailRecipient, templateID, templateData);
                }
            };
        }
         
        public async Task SendNewUserEmail(WebhookPayloads.Users.Create payload)
        {
            var templateData = new
            {
                payload.Request.Body.FirstName,
                payload.Request.Body.LastName,
                payload.Request.Body.Username
            };
            var emailRecipient = payload.Request.Body.Email;
            await SendSingleTemplateEmail(NO_REPLY_EMAIL, emailRecipient, BUYER_NEW_USER_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderSubmitEmail(OrderWorksheet orderWorksheet)
        {
            if (orderWorksheet.Order.xp.OrderType == OrderType.Standard)
            {
                var productsList = orderWorksheet.LineItems.Select((MarketplaceLineItem item) =>
                 new {
                        item.ProductID,
                        item.Product.Name,
                        item.Quantity,
                        item.LineTotal
                });

                var dynamicTemplateData = new
                {
                    orderWorksheet.Order.FromUser.FirstName,
                    orderWorksheet.Order.FromUser.LastName,
                    OrderID = orderWorksheet.Order.ID,
                    DateSubmitted = orderWorksheet.Order.DateSubmitted.ToString(),
                    orderWorksheet.Order.ShippingAddressID,
                    orderWorksheet.Order.BillingAddressID,
                    BillingAddress = new
                    {
                        orderWorksheet.Order.BillingAddress.Street1,
                        orderWorksheet.Order.BillingAddress.Street2,
                        orderWorksheet.Order.BillingAddress.City,
                        orderWorksheet.Order.BillingAddress.State,
                        orderWorksheet.Order.BillingAddress.Zip
                    },
                    Products = productsList,
                    orderWorksheet.Order.Subtotal,
                    orderWorksheet.Order.TaxCost,
                    orderWorksheet.Order.ShippingCost,
                    PromotionalDiscount = orderWorksheet.Order.PromotionDiscount,
                    orderWorksheet.Order.Total
                };
                var emailRecipient = orderWorksheet.Order.FromUser.Email;
                await SendSingleTemplateEmail(NO_REPLY_EMAIL, emailRecipient, BUYER_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
                await SendSupplierOrderSubmitEmail(orderWorksheet);
            } else if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                var dynamicTemplateData = new
                {
                    orderWorksheet.Order.FromUser.FirstName,
                    orderWorksheet.Order.FromUser.LastName
                };
                var emailRecipient = orderWorksheet.Order.FromUser.Email;
                await SendSingleTemplateEmail(NO_REPLY_EMAIL, emailRecipient, BUYER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
                await SendSupplierOrderSubmitEmail(orderWorksheet);
            }
        }

        public async Task SendSupplierOrderSubmitEmail(OrderWorksheet orderWorksheet)
        {
            if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderWorksheet.Order.ID);
                var supplierList = GetSupplierInfo(lineItems);
                foreach (string supplier in supplierList)
                {
                    MarketplaceSupplier supplierInfo = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplier);
                    var quoteOrderData = new { supplierInfo.xp.SupportContact.Name };
                    await SendSupplierEmails(orderWorksheet, SUPPLIER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID, quoteOrderData);
                }
            } else
            {
                var productsList = orderWorksheet.LineItems.Select((MarketplaceLineItem item) =>
                    new {
                        item.ProductID,
                        item.Product.Name,
                        item.Quantity,
                        item.LineTotal
                    });
                var dynamicTemplateData = new
                {
                    orderWorksheet.Order.ToCompanyID,
                    orderWorksheet.Order.FromUser.FirstName,
                    orderWorksheet.Order.FromUser.LastName,
                    DateSubmitted = orderWorksheet.Order.DateSubmitted.ToString(),
                    orderWorksheet.Order.FromUser.Email,
                    orderWorksheet.Order.ShippingAddressID,
                    orderWorksheet.Order.BillingAddressID,
                    BillingAddress = new
                    {
                        orderWorksheet.Order.BillingAddress.Street1,
                        orderWorksheet.Order.BillingAddress.Street2,
                        orderWorksheet.Order.BillingAddress.City,
                        orderWorksheet.Order.BillingAddress.State,
                        orderWorksheet.Order.BillingAddress.Zip
                    },
                    Products = productsList,
                    orderWorksheet.Order.Subtotal,
                    orderWorksheet.Order.TaxCost,
                    orderWorksheet.Order.ShippingCost,
                    PromotionalDiscount = orderWorksheet.Order.PromotionDiscount,
                    orderWorksheet.Order.Total
                };
                await SendSupplierEmails(orderWorksheet, SUPPLIER_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
            }
        }

        public async Task SendOrderApprovalEmails(MessageNotification payload) {
            Console.WriteLine(payload);
            //await SendSingleTemplateEmail(fromEmail, toEmail, BUYER_ORDER_SENT_FOR_APPROVAL_TEMPLATE_ID, templateData);
        }

        // helper function
        private List<string> GetSupplierInfo(ListPage<LineItem> lineItems)
        {
            var supplierList = lineItems.Items.Select(item => item.SupplierID)
                .Distinct()
                .ToList();
            return supplierList;
        }
    }
}