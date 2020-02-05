using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Helpers;
using OrderCloud.SDK;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Marketplace.Common.Services
{
    public interface ISendgridService
    {
        Task SendSingleEmail(string from, string to, string subject, string htmlContent);
        Task SendSupplierEmails(string orderID);
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
        public async Task SendSupplierEmails(string orderID)
        {
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);
            lineItems.Items
                .Select(item => item.SupplierID)
                    .Distinct()
                    .ToList()
                    .ForEach(async supplier =>
                    {
                        Supplier supplierInfo = await _oc.Suppliers.GetAsync(supplier);
                        await SendSingleEmail("noreply@four51.com", supplierInfo.xp.Contacts[0].Email, "Order Confirmation", "<h1>this is a test email for order submit</h1>");
                    });
        }
    }
}