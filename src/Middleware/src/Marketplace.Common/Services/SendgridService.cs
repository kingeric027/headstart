using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Extended;
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
        Task SendSupplierEmails(string orderID);
        Task SendOrderSubmitTemplateEmail(OrderWorksheet orderData);
    }
    public class SendgridService : ISendgridService
    {
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;
        private const string ORDER_SUBMIT_TEMPLATE_ID = "d-defb11ada55d48d8a38dc1074eaaca67";
        private const string QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-3266ef3d70b54d78a74aaf012eaf5e64";
        public SendgridService(AppSettings settings, IOrderCloudClient ocClient)
        {
            _oc = ocClient;
            _settings = settings;
        }
        public async Task SendSingleTemplateEmail(EmailAddress from, EmailAddress to, string templateID, object templateData)
        {
            var client = new SendGridClient(_settings.SendgridApiKey);
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateID, templateData);
            await client.SendEmailAsync(msg);
        }

        public async Task SendSingleEmail(string from, string to, string subject, string htmlContent)
        {
            var client = new SendGridClient(_settings.SendgridApiKey);
            var fromEmail = new EmailAddress(from);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, null, htmlContent);
            await client.SendEmailAsync(msg);
        }

        public async Task SendOrderSubmitTemplateEmail(OrderWorksheet orderWorksheet)
        {
            if (orderWorksheet.Order.xp.OrderType == OrderType.Standard)
            {
                var productsList = orderWorksheet.LineItems.Select((LineItem item) =>
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
                await SendSingleTemplateEmail(fromEmail, toEmail, ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
            } else if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                var dynamicTemplateData = new
                {
                    orderWorksheet.Order.FromUser.FirstName,
                    orderWorksheet.Order.FromUser.LastName
                };
                var fromEmail = new EmailAddress("noreply@four51.com");
                var toEmail = new EmailAddress(orderWorksheet.Order.FromUser.Email);
                await SendSingleTemplateEmail(fromEmail, toEmail, QUOTE_ORDER_SUBMIT_TEMPLATE_ID, dynamicTemplateData);
            }
        }

        public async Task SendSupplierEmails(string orderID)
        {
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);
            lineItems.Items
                .Select(item => item.SupplierID)
                    .Distinct()
                    .ToList()
                    .ForEach(async supplier =>
                    {
                        MarketplaceSupplier supplierInfo = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplier);

                        // the email that will be sent a notification of the email for the supplier may not be found on xp.Supportcontact in the future
                        var emailRecipient = supplierInfo.xp.SupportContact.Email;
                        if (emailRecipient.Length > 0)
                        {
                            var fromEmail = new EmailAddress("noreply@four51.com");
                            var toEmail = new EmailAddress("scasey@four51.com");
                            await SendSingleTemplateEmail(fromEmail, toEmail, null, null);
                        }
                    });
        }
    }
}