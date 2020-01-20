using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Marketplace.Common.Services
{
    public interface ISendgridService
    {
        Task SendSingleEmail(string from, string to, string subject, string htmlContent);
    }
    public class SendgridService : ISendgridService
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