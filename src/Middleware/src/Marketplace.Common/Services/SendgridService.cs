using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dynamitey;
using Marketplace.Common.Constants;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Marketplace;
using Microsoft.WindowsAzure.Storage.Blob;
using ordercloud.integrations.library;
using ordercloud.integrations.library.helpers;
using OrderCloud.SDK;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Marketplace.Common.Services
{
    public interface ISendgridService
    {
        Task SendSingleEmail(string from, string to, string subject, string htmlContent);
        Task SendSingleTemplateEmail(string from, string to, string templateID, object templateData);
        Task SendSingleTemplateEmailMultipleRcpts(string from, List<EmailAddress> tos, string templateID, object templateData);
        Task SendSingleTemplateEmailMultipleRcptsAttachment(string from, List<EmailAddress> tos, string templateID, object templateData, CloudAppendBlob fileReference, string fileName);

        Task SendOrderSubmitEmail(MarketplaceOrderWorksheet orderData);
        Task SendNewUserEmail(MessageNotification<PasswordResetEventBody> payload);
        Task SendOrderRequiresApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification);
        Task SendPasswordResetEmail(MessageNotification<PasswordResetEventBody> messageNotification);
        Task SendOrderSubmittedForApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification);
        Task SendOrderApprovedEmail(MarketplaceOrderApprovePayload payload);
        Task SendOrderDeclinedEmail(MarketplaceOrderDeclinePayload payload);
        Task SendLineItemStatusChangeEmail(MarketplaceOrder order, LineItemStatusChanges lineItemStatusChanges, List<MarketplaceLineItem> lineItems, string firstName, string lastName, string email, EmailDisplayText lineItemEmailDisplayText);
        Task SendLineItemStatusChangeEmailMultipleRcpts(MarketplaceOrder order, LineItemStatusChanges lineItemStatusChanges, List<MarketplaceLineItem> lineItems, List<EmailAddress> tos, EmailDisplayText lineItemEmailDisplayText);
        Task SendContactSupplierAboutProductEmail(ContactSupplierBody template);
        Task SendProductUpdateEmail(List<EmailAddress> tos, CloudAppendBlob fileReference, string fileName);
    }

    public class EmailTemplate
    {
        public object Data { get; set; }
        public EmailDisplayText Message { get; set; }
    }
    public class SendgridService : ISendgridService
    {
        private readonly AppSettings _settings; 
        private readonly IOrderCloudClient _oc;
        private const string NO_REPLY_EMAIL_ADDRESS = "noreply@four51.com";
        private const string ORDER_SUBMIT_TEMPLATE_ID = "d-defb11ada55d48d8a38dc1074eaaca67";
        private const string LINE_ITEM_STATUS_CHANGE = "d-4ca85250efaa4d3f8a2e3144d4373f8c";
        private const string QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-3266ef3d70b54d78a74aaf012eaf5e64";
        private const string BUYER_NEW_USER_TEMPLATE_ID = "d-f3831baa2beb4c19aeace19e48132768";
        private const string BUYER_PASSWORD_RESET_TEMPLATE_ID = "d-ca6a6ff8c9ac4264bf86b5d6cdd3a038";
        private const string INFORMATION_REQUEST = "d-e6bad6d1df2a4876a9f7ea2d3ac50e02";
        private const string PRODUCT_UPDATE_TEMPLATE_ID = "d-8d60fcbc191b4fd1ae526e28713e6abe";
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

        public async Task SendSingleTemplateEmailMultipleRcpts(string from, List<EmailAddress> tos, string templateID, object templateData)
        {
            var client = new SendGridClient(_settings.SendgridApiKey);
            var fromEmail = new EmailAddress(from);
            var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(fromEmail, tos, templateID, templateData);
            await client.SendEmailAsync(msg);
        }

        public async Task SendSingleTemplateEmailMultipleRcptsAttachment(string from, List<EmailAddress> tos, string templateID, object templateData, CloudAppendBlob fileReference, string fileName)
        {
            var client = new SendGridClient(_settings.SendgridApiKey);
            var fromEmail = new EmailAddress(from);
            var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(fromEmail, tos, templateID, templateData);
            using (Stream stream = await fileReference.OpenReadAsync())
            {
                await msg.AddAttachmentAsync(fileName, stream);
            }
                await client.SendEmailAsync(msg);
        }

        public async Task SendPasswordResetEmail(MessageNotification<PasswordResetEventBody> messageNotification)
        {
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = new
                {
                    messageNotification.Recipient.FirstName,
                    messageNotification.Recipient.LastName,
                    messageNotification.EventBody.PasswordRenewalAccessToken,
                    messageNotification.EventBody.PasswordRenewalUrl
                }
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, messageNotification.Recipient.Email, BUYER_PASSWORD_RESET_TEMPLATE_ID, templateData);
        }

        private List<object> CreateTemplateProductList(List<MarketplaceLineItem> lineItems, LineItemStatusChanges lineItemStatusChanges)
        {
            return lineItems.Select(lineItem =>
            {
                var lineItemStatusChange = lineItemStatusChanges.Changes.First(li => li.ID == lineItem.ID);
                return MapToTemplateProduct(lineItem, lineItemStatusChange);
            }).ToList();
        }

        private object MapToTemplateProduct(MarketplaceLineItem lineItem, LineItemStatusChange lineItemStatusChange)
        {
            return new
            {
                ProductName = lineItem.Product.Name,
                ImageURL = lineItem.xp.ImageUrl,
                lineItem.ProductID,
                lineItem.Quantity,
                lineItem.LineTotal,
                QuantityChanged = lineItemStatusChange.Quantity
            };
        }

        public async Task SendLineItemStatusChangeEmail(MarketplaceOrder order, LineItemStatusChanges lineItemStatusChanges, List<MarketplaceLineItem> lineItems, string firstName, string lastName, string email, EmailDisplayText lineItemEmailDisplayText)
        {
            var productsList = CreateTemplateProductList(lineItems, lineItemStatusChanges);
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = new
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Products = productsList,
                    DateSubmitted = order?.DateSubmitted?.ToString(),
                    OrderID = order.ID,
                    order.Comments
                },
                Message = new EmailDisplayText()
                {
                    EmailSubject = lineItemEmailDisplayText?.EmailSubject,
                    DynamicText = lineItemEmailDisplayText?.DynamicText,
                    DynamicText2 = lineItemEmailDisplayText?.DynamicText2
                }
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, email, LINE_ITEM_STATUS_CHANGE, templateData);
        }

        public async Task SendProductUpdateEmail(List<EmailAddress> tos, CloudAppendBlob fileReference, string fileName)
        {
            var yesterday = DateTime.Now.AddDays(-1).ToString();
            var templateData = new
            {
                date = yesterday
            };
            await SendSingleTemplateEmailMultipleRcptsAttachment(NO_REPLY_EMAIL_ADDRESS, tos, PRODUCT_UPDATE_TEMPLATE_ID, templateData, fileReference, fileName);
            //string from, List<EmailAddress> tos, string templateID, object templateData, CloudAppendBlob fileReference, string fileName

        }

        public async Task SendLineItemStatusChangeEmailMultipleRcpts(MarketplaceOrder order, LineItemStatusChanges lineItemStatusChanges, List<MarketplaceLineItem> lineItems, List<EmailAddress> tos, EmailDisplayText lineItemEmailDisplayText)
        {
            var productsList = CreateTemplateProductList(lineItems, lineItemStatusChanges);
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = new
                {
                    FirstName = "",
                    LastName = "",
                    Products = productsList,
                    DateSubmitted = order.DateSubmitted.ToString(),
                    OrderID = order.ID,
                    order.Comments
                },
                Message = new EmailDisplayText()
                {
                    EmailSubject = lineItemEmailDisplayText?.EmailSubject,
                    DynamicText = lineItemEmailDisplayText?.DynamicText,
                    DynamicText2 = lineItemEmailDisplayText?.DynamicText2,
                }
            };
            await SendSingleTemplateEmailMultipleRcpts(NO_REPLY_EMAIL_ADDRESS, tos, LINE_ITEM_STATUS_CHANGE, templateData);
        }

        public async Task SendOrderSubmittedForApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification)
        {
            var order = messageNotification.EventBody.Order;
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = GetOrderTemplateData(order, messageNotification.EventBody.LineItems),
                Message = OrderSubmitEmailConstants.GetRequestedApprovalText()
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, messageNotification.Recipient.Email, ORDER_SUBMIT_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderRequiresApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification)
        {
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = new
                {
                    messageNotification.Recipient.FirstName,
                    messageNotification.Recipient.LastName,
                    OrderID = messageNotification.EventBody.Order.ID
                },
                Message = OrderSubmitEmailConstants.GetOrderRequiresApprovalText()
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, messageNotification.Recipient.Email, ORDER_SUBMIT_TEMPLATE_ID, templateData);
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
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = new
                {
                    messageNotification.Recipient.FirstName,
                    messageNotification.Recipient.LastName,
                    messageNotification.EventBody.PasswordRenewalAccessToken,
                    BaseAppURL,
                    messageNotification.EventBody.Username
                }
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, messageNotification.Recipient.Email, BUYER_NEW_USER_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderApprovedEmail(MarketplaceOrderApprovePayload payload)
        {
            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, payload.Response.Body.ID);
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = GetOrderTemplateData(payload.Response.Body, lineItems.Items),
                Message = OrderSubmitEmailConstants.GetOrderApprovedText()
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, payload.Response.Body.FromUser.Email, ORDER_SUBMIT_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderDeclinedEmail(MarketplaceOrderDeclinePayload payload)
        {
            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, payload.Response.Body.ID);
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = GetOrderTemplateData(payload.Response.Body, lineItems.Items),
                Message = OrderSubmitEmailConstants.GetOrderDeclinedText()
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, payload.Response.Body.FromUser.Email, ORDER_SUBMIT_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderSubmitEmail(MarketplaceOrderWorksheet orderWorksheet)
        {
            var supplierEmailList = await GetSupplierEmails(orderWorksheet);
            var firstName = orderWorksheet.Order.FromUser.FirstName;
            var lastName = orderWorksheet.Order.FromUser.LastName;
            if (orderWorksheet.Order.xp.OrderType == OrderType.Standard)
            {
                var orderData = GetOrderTemplateData(orderWorksheet.Order, orderWorksheet.LineItems);
                EmailTemplate supplierTemplateData = new EmailTemplate()
                {
                    Data = orderData,
                    Message = OrderSubmitEmailConstants.GetOrderSubmitText(orderWorksheet.Order.ID, firstName, lastName, VerifiedUserType.supplier)
                };

                EmailTemplate sellerTemplateData = new EmailTemplate()
                {
                    Data = orderData,
                    Message = OrderSubmitEmailConstants.GetOrderSubmitText(orderWorksheet.Order.ID, firstName, lastName, VerifiedUserType.admin)
                };
                EmailTemplate buyerTemplateData = new EmailTemplate()
                {
                    Data = orderData,
                    Message = OrderSubmitEmailConstants.GetOrderSubmitText(orderWorksheet.Order.ID, firstName, lastName, VerifiedUserType.buyer)
                };

                var sellerEmailList = await GetSellerEmails();

                //  send emails
                await SendSingleTemplateEmailMultipleRcpts(NO_REPLY_EMAIL_ADDRESS, supplierEmailList, ORDER_SUBMIT_TEMPLATE_ID, supplierTemplateData);
                await SendSingleTemplateEmailMultipleRcpts(NO_REPLY_EMAIL_ADDRESS, sellerEmailList, ORDER_SUBMIT_TEMPLATE_ID, sellerTemplateData);
                await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, orderWorksheet.Order.FromUser.Email, ORDER_SUBMIT_TEMPLATE_ID, buyerTemplateData);
            }
            else if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                Address supplierAddress = null; ;
                if(orderWorksheet.Order.xp.SupplierIDs != null && orderWorksheet.Order.xp.ShipFromAddressIDs != null)
                {
                    supplierAddress = await _oc.SupplierAddresses.GetAsync(orderWorksheet.Order.xp.SupplierIDs.FirstOrDefault(), orderWorksheet.Order.xp.ShipFromAddressIDs.FirstOrDefault());
                }
                var orderData = GetQuoteOrderTemplateData(orderWorksheet.Order, orderWorksheet.LineItems, supplierAddress);

                EmailTemplate buyerTemplateData = new EmailTemplate()
                {
                    Data = orderData,
                    Message = OrderSubmitEmailConstants.GetQuoteOrderSubmitText(VerifiedUserType.buyer)
                };
                EmailTemplate supplierTemplateData = new EmailTemplate()
                {
                    Data = orderData,
                    Message = OrderSubmitEmailConstants.GetQuoteOrderSubmitText(VerifiedUserType.supplier)
                };

                //  send emails
                await SendSingleTemplateEmailMultipleRcpts(NO_REPLY_EMAIL_ADDRESS, supplierEmailList, QUOTE_ORDER_SUBMIT_TEMPLATE_ID, supplierTemplateData);
                await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, orderWorksheet.Order.FromUser.Email, QUOTE_ORDER_SUBMIT_TEMPLATE_ID, buyerTemplateData);
            }
        }

        private async Task<List<EmailAddress>> GetSupplierEmails(MarketplaceOrderWorksheet orderWorksheet)
        {
            ListPage<MarketplaceSupplier> suppliers = null;
            if (orderWorksheet.Order.xp.SupplierIDs != null)
            {
                var filterString = String.Join("|", orderWorksheet.Order.xp.SupplierIDs);
                suppliers = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(filters: $"ID={filterString}");
            }
            var supplierTos = new List<EmailAddress>();
            foreach (var supplier in suppliers.Items)
            {
                if (supplier?.xp?.NotificationRcpts?.Count() > 0)
                {
                    foreach (var rcpt in supplier.xp.NotificationRcpts)
                    {
                        supplierTos.Add(new EmailAddress(rcpt));
                    };
                }
            }
            return supplierTos;
        }

        private async Task<List<EmailAddress>> GetSellerEmails()
        {
            var sellerUsers = await _oc.AdminUsers.ListAsync<MarketplaceSellerUser>();
            var sellerTos = new List<EmailAddress>();
            foreach (var seller in sellerUsers.Items)
            {
                if (seller?.xp?.OrderEmails ?? false)
                {
                    sellerTos.Add(new EmailAddress(seller.Email));
                };
                if (seller?.xp?.AddtlRcpts?.Any() ?? false)
                {
                    foreach (var rcpt in seller.xp.AddtlRcpts)
                    {
                        sellerTos.Add(new EmailAddress(rcpt));
                    };
                };
            };
            return sellerTos;
        }

        public async Task SendLineItemStatusChangeEmail(LineItemStatusChange lineItemStatusChange, List<MarketplaceLineItem> lineItems, string firstName, string lastName, string email, EmailDisplayText lineItemEmailDisplayText)
        {
            var productsList = lineItems.Select(MapLineItemToProduct);

            EmailTemplate templateData = new EmailTemplate()
            {
                Data = new
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Products = productsList,


                },
                Message = new EmailDisplayText()
                {
                    EmailSubject = lineItemEmailDisplayText?.EmailSubject,
                    DynamicText = lineItemEmailDisplayText?.DynamicText,
                    DynamicText2 = lineItemEmailDisplayText?.DynamicText2
                }
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, email, LINE_ITEM_STATUS_CHANGE, templateData);
        }

        public async Task SendContactSupplierAboutProductEmail(ContactSupplierBody template)
        {
            var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(template.Product.DefaultSupplierID);
            var supplierEmail = supplier.xp.SupportContact.Email;
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = new
                {
                    ProductID = template.Product.ID,
                    ProductName = template.Product.Name,
                    template.BuyerRequest.FirstName,
                    template.BuyerRequest.LastName,
                    Location = template.BuyerRequest.BuyerLocation,
                    template.BuyerRequest.Phone,
                    template.BuyerRequest.Email,
                    Note = template.BuyerRequest.Comments
                }
            };
            await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, template.BuyerRequest.Email, INFORMATION_REQUEST, templateData);
            var sellerUsers = await ListAllAsync.List((page) => _oc.AdminUsers.ListAsync<MarketplaceUser>(
                    filters: $"xp.RequestInfoEmails=true",
                    page: page,
                    pageSize: 100
                 ));
            foreach (var sellerUser in sellerUsers)
            {
                await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, sellerUser.Email, INFORMATION_REQUEST, templateData);
                if (sellerUser.xp.AddtlRcpts.Any())
                {
                    foreach (var rcpt in sellerUser.xp.AddtlRcpts)
                    {
                        await SendSingleTemplateEmail(NO_REPLY_EMAIL_ADDRESS, rcpt, INFORMATION_REQUEST, templateData);
                    }
                }
            }
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
            var productsList = lineItems.Select(lineItem =>
            {
                return new
                {
                    ProductName = lineItem.Product.Name,
                    ImageURL = lineItem.xp.ImageUrl,
                    lineItem.ProductID,
                    lineItem.Quantity,
                    lineItem.LineTotal,
                };
            });
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
                order.Total,
            };
        }

        private object GetQuoteOrderTemplateData(MarketplaceOrder order, IList<MarketplaceLineItem> lineItems, Address supplierAddress)
        {
            return new
            {
                FirstName = order.FromUser.FirstName,
                LastName = order.FromUser.LastName,
                Phone = order.xp.QuoteOrderInfo.Phone,
                Email = order.FromUser.Email,
                Location = supplierAddress == null ? null : $"{supplierAddress?.Street1}, {supplierAddress?.City}, {supplierAddress?.State} {supplierAddress?.Zip}",
                ProductID = lineItems.FirstOrDefault().Product.ID,
                ProductName = lineItems.FirstOrDefault().Product.Name,
                order = order,
            };
        }

        private List<object> MapLineItemsToProducts(ListPage<MarketplaceLineItem> lineItems, string actionType)
        {
            List<object> products = new List<object>();

            foreach(var lineItem in lineItems.Items)
            {
                if (lineItem.xp.Returns != null && actionType == "return")
                {
                    products.Add(MapReturnedLineItemToProduct(lineItem));
                }
                else if (lineItem.xp.Cancelations != null && actionType == "cancel")
                {
                    products.Add(MapCanceledLineItemToProduct(lineItem));
                }
                else
                {
                    products.Add(MapLineItemToProduct(lineItem));
                }
            }
            return products;
        }

        private object MapReturnedLineItemToProduct(MarketplaceLineItem lineItem) =>
        new
        {
            ProductName = lineItem.Product.Name,
            ImageURL = lineItem.xp.ImageUrl,
            lineItem.ProductID,
            lineItem.Quantity,
            lineItem.LineTotal,
        };

        private object MapCanceledLineItemToProduct(MarketplaceLineItem lineItem) =>
        new
        {
            ProductName = lineItem.Product.Name,
            ImageURL = lineItem.xp.ImageUrl,
            lineItem.ProductID,
            lineItem.Quantity,
            lineItem.LineTotal,
        };

        private object MapLineItemToProduct(MarketplaceLineItem lineItem) =>
          new
          {
              ProductName = lineItem.Product.Name,
              ImageURL = lineItem.xp.ImageUrl,
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