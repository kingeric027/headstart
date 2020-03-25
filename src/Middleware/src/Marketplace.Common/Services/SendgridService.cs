using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Helpers;
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
        Task SendSingleTemplateEmail(string from, string to, string subject, string templateID, object templateData);
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

        public async Task SendSingleTemplateEmail(string from, string to, string subject, string templateID, object templateData)
        {
            var client = new SendGridClient(_settings.SendgridApiKey);
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom(from);
            sendGridMessage.AddTo(to);
            sendGridMessage.SetSubject(subject);
            sendGridMessage.SetTemplateId(templateID);
            sendGridMessage.SetTemplateData(templateData);
            await client.SendEmailAsync(sendGridMessage);
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