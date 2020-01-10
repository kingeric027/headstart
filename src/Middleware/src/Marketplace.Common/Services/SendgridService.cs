using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services
{
    public class SendgridService
    {
        private readonly AppSettings _settings;

        public SendgridService(AppSettings settings)
        {
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
    }
}