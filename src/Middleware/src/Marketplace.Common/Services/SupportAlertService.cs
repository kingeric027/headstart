using Headstart.Common.Models.Misc;
﻿using Headstart.Common.Models.Marketplace;
using Headstart.Models;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using ordercloud.integrations.cardconnect;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Headstart.Common.Models.SendGridModels;

namespace Headstart.Common.Services
{
    public interface ISupportAlertService
    {
        Task VoidAuthorizationFailed(HSPayment payment, string transactionID, HSOrder order, CreditCardVoidException ex);

        Task EmailGeneralSupportQueue(SupportCase supportCase);

        

    }

    // use this service to alert support of critical failures
    public class SupportAlertService : ISupportAlertService
    {
        private const string SUPPORT_TEMPLATE_ID = "d-5054886b66f7447ab3c8e69c50b283b3";
        private readonly TelemetryClient _telemetry;
        private readonly ISendgridService _sendgrid;
        private readonly AppSettings _settings;
        public SupportAlertService(TelemetryClient telemetry, ISendgridService sendgrid, AppSettings settings)
        {
            _telemetry = telemetry;
            _sendgrid = sendgrid;
            _settings = settings;
        }

        public async Task VoidAuthorizationFailed(HSPayment payment, string transactionID, HSOrder order, CreditCardVoidException ex)
        {
            LogVoidAuthorizationFailed(payment, transactionID, order, ex);
            await EmailVoidAuthorizationFailedAsync(payment, transactionID, order, ex);
        }

        public void LogVoidAuthorizationFailed(HSPayment payment, string transactionID, HSOrder order, CreditCardVoidException ex)
        {
            // track in app insights
            // to find go to Transaction Search > Event Type = Event > Filter by any of these custom properties or event name "Payment.VoidAuthorizationFailed"
            var customProperties = new Dictionary<string, string>
                {
                    { "Message", "Attempt to void authorization on payment failed" },
                    { "OrderID", order.ID },
                    { "BuyerID", order.FromCompanyID },
                    { "UserEmail", order.FromUser.Email },
                    { "PaymentID", payment.ID },
                    { "TransactionID", transactionID },
                    { "ErrorResponse", JsonConvert.SerializeObject(ex.ApiError, Formatting.Indented)}
                };
            _telemetry.TrackEvent("Payment.VoidAuthorizationFailed", customProperties);
        }

        public async Task EmailVoidAuthorizationFailedAsync(HSPayment payment, string transactionID, HSOrder order, CreditCardVoidException ex)
        {
            var templateData = new EmailTemplate()
            {
                Data = new
                {
                    OrderID = order.ID,
                    DynamicPropertyName1 = "BuyerID",
                    DynamicPropertyValue1 = order.FromCompanyID,
                    DynamicPropertyName2 = "Username",
                    DynamicPropertyValue2 = order.FromUser.Username,
                    DynamicPropertyName3 = "PaymentID",
                    DynamicPropertyValue3 = payment.ID,
                    DynamicPropertyName4 = "TransactionID",
                    DynamicPropertyValue4 = transactionID,
                    ErrorJsonString = JsonConvert.SerializeObject(ex.ApiError)
                },
                Message = new EmailDisplayText()
                {
                    EmailSubject = "Manual intervention required for this order",
                    DynamicText = "Error encountered while trying to void authorization on this order. Please contact customer and help them manually void authorization"
                }
            };
            var toList = new List<EmailAddress>();
            var supportEmails = _settings.SendgridSettings.SupportEmails.Split(",");
            foreach (var email in supportEmails)
            {
                toList.Add(new EmailAddress { Email = email });
            }
            await _sendgrid.SendSingleTemplateEmailMultipleRcpts(_settings.SendgridSettings.FromEmail, toList, SUPPORT_TEMPLATE_ID, templateData);
        }

        public async Task EmailGeneralSupportQueue(SupportCase supportCase)
        {
            var templateData = new EmailTemplate()
            {
                Data = new
                {
                    DynamicPropertyName1 = "FirstName",
                    DynamicPropertyValue1 = supportCase.FirstName,
                    DynamicPropertyName2 = "LastName",
                    DynamicPropertyValue2 = supportCase.LastName,
                    DynamicPropertyName3 = "Email",
                    DynamicPropertyValue3 = supportCase.Email,
                    DynamicPropertyName4 = "Vendor",
                    DynamicPropertyValue4 = supportCase.Vendor ?? "N/A",
                },
                Message = new EmailDisplayText()
                {
                    EmailSubject = supportCase.Subject,
                    DynamicText = supportCase.Message
                }
            };
            var recipient = DetermineRecipient(supportCase.Subject);
            await _sendgrid.SendSingleTemplateEmailSingleRcptAttachment(_settings.SendgridSettings.FromEmail, recipient, SUPPORT_TEMPLATE_ID, templateData, supportCase.File);
        }

        private string DetermineRecipient(string subject)
        {
            switch (subject.ToLower())
            {
                case "general":
                    return _settings.SendgridSettings.SEBSupportCaseEmail;
                case "report an error/bug":
                    return _settings.SendgridSettings.SEBSupportCaseEmail;
                case "payment, billing, or refunds":
                    return _settings.SendgridSettings.SEBBillingEmail;
                default:
                    return _settings.SendgridSettings.SEBSupportCaseEmail;
            }
        }
    }
}
