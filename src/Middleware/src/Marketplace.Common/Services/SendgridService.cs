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
        Task SendSingleTemplateEmail(EmailAddress from, EmailAddress to, string templateID, object templateData);
        Task SendOrderSupplierEmails(OrderWorksheet orderWorksheet, string templateID, object templateData);
        Task SendOrderSubmitEmail(OrderWorksheet orderData);
        Task SendNewUserEmail(WebhookPayloads.Users.Create payload);
        Task SendOrderUpdatedEmail(WebhookPayloads.Orders.Patch payload);
    }
    public class SendgridService : ISendgridService
    {
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;
        private const string BUYER_ORDER_SUBMIT_TEMPLATE_ID = "d-defb11ada55d48d8a38dc1074eaaca67";
        private const string SUPPLIER_ORDER_SUBMIT_TEMPLATE_ID = "d-777af54b1e414b0b853f983697889267";
        private const string BUYER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-3266ef3d70b54d78a74aaf012eaf5e64";
        private const string SUPPLIER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-5776a6c57b344aeda605444c96ff39e8";
        private const string BUYER_NEW_USER_TEMPLATE_ID = "d-f3831baa2beb4c19aeace19e48132768";
        private const string BUYER_ORDER_UPDATED_TEMPLATE_ID = "d-ff15cd80bb934f90ae4fe90678d88d54";
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
        public async Task SendSingleTemplateEmail(EmailAddress from, EmailAddress to, string templateID, object templateData)
        {
            var client = new SendGridClient(_settings.SendgridApiKey);
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateID, templateData);
            var res = await client.SendEmailAsync(msg);
            Console.WriteLine(res);
        }

        public async Task SendNewUserEmail(WebhookPayloads.Users.Create payload)
        {
            var templateData = new
            {
                payload.Request.Body.FirstName,
                payload.Request.Body.LastName,
                payload.Request.Body.Username
            };
            var fromEmail = new EmailAddress("noreply@four51.com");
            var toEmail = new EmailAddress(payload.Request.Body.Email);
            await SendSingleTemplateEmail(fromEmail, toEmail, BUYER_NEW_USER_TEMPLATE_ID, templateData);
        }
        
        public async Task SendOrderUpdatedEmail(WebhookPayloads.Orders.Patch payload)
        {
            var order = await _oc.Orders.GetAsync(OrderDirection.Incoming, payload.Response.Body.ID);
            Console.WriteLine(order);
           var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, payload.Response.Body.ID);
            List<object> productsList = new List<object>();

            foreach (var item in lineItems.Items)
            {
                productsList.Add(new
                {
                    ProductName = item.Product.Name,
                    item.ProductID,
                    item.Quantity,
                    item.LineTotal
                });
            };
                var dynamicTemplateData = new
                {
                    payload.Response.Body.FromUser.FirstName,
                    payload.Response.Body.FromUser.LastName,
                    payload.Response.Body.ID,
                    DateSubmitted = payload.Response.Body.DateCreated.ToString(),
                    payload.Response.Body.ShippingAddressID,
                    BillingAddressID = 1283497,
                    BillingAddress = new
                    {
                        Street1 = "123 test street",
                        Street2 = "",
                        City = "minneapolis",
                        State = "MN",
                        Zip = 55408
                    },
                    products = productsList,
                    payload.Response.Body.Subtotal,
                    payload.Response.Body.TaxCost,
                    payload.Response.Body.ShippingCost,
                    PromotionalDiscount = payload.Response.Body.PromotionDiscount,
                    payload.Response.Body.Total
                };
                var fromEmail = new EmailAddress("noreply@four51.com");
                //var toEmail = new EmailAddress("scasey@four51.com");
                var toEmail = new EmailAddress(payload.Response.Body.FromUser.Email);
                await SendSingleTemplateEmail(fromEmail, toEmail, BUYER_ORDER_UPDATED_TEMPLATE_ID, dynamicTemplateData);
/*            }
            else if (payload.Response.Body.xp.OrderType == "Quote")
            {
                var dynamicTemplateData = new
                {
                    payload.Response.Body.FromUser.FirstName,
                    payload.Response.Body.FromUser.LastName
                };
                var fromEmail = new EmailAddress("noreply@four51.com");
                var toEmail = new EmailAddress(payload.Response.Body.FromUser.Email);
                await SendSingleTemplateEmail(fromEmail, toEmail, BUYER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
            }*/
        }

        /*                 --------- ORDER SUBMIT EMAILS ----------                 */
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
                var fromEmail = new EmailAddress("noreply@four51.com");
                var toEmail = new EmailAddress(orderWorksheet.Order.FromUser.Email);
                await SendSingleTemplateEmail(fromEmail, toEmail, BUYER_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
                await SendSupplierOrderSubmitEmail(orderWorksheet);
            } else if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                var dynamicTemplateData = new
                {
                    orderWorksheet.Order.FromUser.FirstName,
                    orderWorksheet.Order.FromUser.LastName
                };
                var fromEmail = new EmailAddress("noreply@four51.com");
                var toEmail = new EmailAddress(orderWorksheet.Order.FromUser.Email);
                await SendSingleTemplateEmail(fromEmail, toEmail, BUYER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
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
                    await SendOrderSupplierEmails(orderWorksheet, SUPPLIER_QUOTE_ORDER_SUBMIT_TEMPLATE_ID, quoteOrderData);
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
                await SendOrderSupplierEmails(orderWorksheet, SUPPLIER_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
            }
        }
        public async Task SendOrderSupplierEmails(OrderWorksheet orderWorksheet, string templateID, object templateData)
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
                    var fromEmail = new EmailAddress("noreply@four51.com");
                    var toEmail = new EmailAddress(emailRecipient);
                    await SendSingleTemplateEmail(fromEmail, toEmail, templateID, quoteOrderData);
                }
                else if (emailRecipient.Length > 0)
                {
                    var fromEmail = new EmailAddress("noreply@four51.com");
                    var toEmail = new EmailAddress(emailRecipient);
                    await SendSingleTemplateEmail(fromEmail, toEmail, templateID, templateData);
                }
            };
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