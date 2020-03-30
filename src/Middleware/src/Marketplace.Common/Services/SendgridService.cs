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
        Task SendSupplierEmails(string orderID);
        Task SendOrderSubmitTemplateEmail(OrderWorksheet orderData);
    }
    public class SendgridService : ISendgridService
    {
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;

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

        public async Task SendOrderSubmitTemplateEmail(OrderWorksheet orderWorksheet)
        {
            if (orderWorksheet.Order.xp.OrderType == OrderType.Standard)
            {
                List<object> productsList = new List<object>();

                foreach (var item in orderWorksheet.LineItems)
                {
                    productsList.Add(new
                    {
                        item.ProductID,
                        ProductName = item.Product.Name,
                        item.Quantity,
                        item.LineTotal
                    });
                };

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
                var client = new SendGridClient(_settings.SendgridApiKey);
                var fromEmail = new EmailAddress("noreply@four51.com");
                var toEmail = new EmailAddress(orderWorksheet.Order.FromUser.Email);
                var templateID = "d-defb11ada55d48d8a38dc1074eaaca67";
                var msg = MailHelper.CreateSingleTemplateEmail(fromEmail, toEmail, templateID, dynamicTemplateData);
                await client.SendEmailAsync(msg);
            } else if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                var dynamicTemplateData = new
                {
                    orderWorksheet.Order.FromUser.FirstName,
                    orderWorksheet.Order.FromUser.LastName
                };
                var client = new SendGridClient(_settings.SendgridApiKey);
                var fromEmail = new EmailAddress("noreply@four51.com");
                var toEmail = new EmailAddress(orderWorksheet.Order.FromUser.Email);
                var templateID = "d-3266ef3d70b54d78a74aaf012eaf5e64";
                var msg = MailHelper.CreateSingleTemplateEmail(fromEmail, toEmail, templateID, dynamicTemplateData);
                await client.SendEmailAsync(msg);
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
                            await SendSingleEmail("noreply@four51.com", emailRecipient, "Order Confirmation", "<h1>this is a test email for order submit</h1>");
                        }
                    });
        }
    }
}