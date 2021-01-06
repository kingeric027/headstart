using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dynamitey;
using Flurl.Http;
using Flurl.Http.Configuration;
using Marketplace.Common.Constants;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Marketplace;
using Microsoft.WindowsAzure.Storage.Blob;
using ordercloud.integrations.library.helpers;
using OrderCloud.SDK;
using SendGrid;
using SendGrid.Helpers.Mail;
using static Marketplace.Common.Models.SendGridModels;

namespace Marketplace.Common.Services
{
    public interface ISendgridService
    {
        Task SendSingleEmail(string from, string to, string subject, string htmlContent);
        Task SendSingleTemplateEmail(string from, string to, string templateID, object templateData);
        Task SendSingleTemplateEmailMultipleRcpts(string from, List<EmailAddress> tos, string templateID, object templateData);
        Task SendSingleTemplateEmailMultipleRcptsAttachment(string from, List<EmailAddress> tos, string templateID, object templateData, CloudAppendBlob fileReference, string fileName);
        Task SendOrderSubmitEmail(HSOrderWorksheet orderData);
        Task SendNewUserEmail(MessageNotification<PasswordResetEventBody> payload);
        Task SendPasswordResetEmail(MessageNotification<PasswordResetEventBody> messageNotification);
        Task SendOrderRequiresApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification);
        Task SendOrderSubmittedForApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification);
        Task SendOrderApprovedEmail(MessageNotification<OrderSubmitEventBody> messageNotification);
        Task SendOrderDeclinedEmail(MessageNotification<OrderSubmitEventBody> messageNotification);
        Task SendLineItemStatusChangeEmail(HSOrder order, LineItemStatusChanges lineItemStatusChanges, List<HSLineItem> lineItems, string firstName, string lastName, string email, EmailDisplayText lineItemEmailDisplayText);
        Task SendLineItemStatusChangeEmailMultipleRcpts(HSOrder order, LineItemStatusChanges lineItemStatusChanges, List<HSLineItem> lineItems, List<EmailAddress> tos, EmailDisplayText lineItemEmailDisplayText);
        Task SendContactSupplierAboutProductEmail(ContactSupplierBody template);
        Task SendProductUpdateEmail(List<EmailAddress> tos, CloudAppendBlob fileReference, string fileName);
    }


    public class SendgridService : ISendgridService
    {
        private readonly AppSettings _settings; 
        private readonly IOrderCloudClient _oc;
        private const string ORDER_SUBMIT_TEMPLATE_ID = "d-defb11ada55d48d8a38dc1074eaaca67";
        private const string ORDER_APPROVAL_TEMPLATE_ID = "d-2f3b92b95b7b45ea8f8fb94c8ac928e0";
        private const string LINE_ITEM_STATUS_CHANGE = "d-4ca85250efaa4d3f8a2e3144d4373f8c";
        private const string QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "d-3266ef3d70b54d78a74aaf012eaf5e64";
        private const string BUYER_NEW_USER_TEMPLATE_ID = "d-f3831baa2beb4c19aeace19e48132768";
        private const string BUYER_PASSWORD_RESET_TEMPLATE_ID = "d-ca6a6ff8c9ac4264bf86b5d6cdd3a038";
        private const string INFORMATION_REQUEST = "d-e6bad6d1df2a4876a9f7ea2d3ac50e02";
        private const string PRODUCT_UPDATE_TEMPLATE_ID = "d-d0e5fda7ce8c4ffe9da1fe82ab14beb6";
        private readonly ISendGridClient _client;

        public SendgridService(AppSettings settings, IOrderCloudClient ocClient, ISendGridClient client)
        {
            _oc = ocClient;
            _client = client;
            _settings = settings;
        }

        public async Task SendSingleEmail(string from, string to, string subject, string htmlContent) //temp function until all endpoints are accessible for template data
        {
            var fromEmail = new EmailAddress(from);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, null, htmlContent);
            await _client.SendEmailAsync(msg);
        }

        public virtual async Task SendSingleTemplateEmail(string from, string to, string templateID, object templateData)
        {
            var fromEmail = new EmailAddress(from);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleTemplateEmail(fromEmail, toEmail, templateID, templateData);
            await _client.SendEmailAsync(msg);
        }

        public virtual async Task SendSingleTemplateEmailMultipleRcpts(string from, List<EmailAddress> tos, string templateID, object templateData)
        {
            var fromEmail = new EmailAddress(from);
            var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(fromEmail, tos, templateID, templateData);
            await _client.SendEmailAsync(msg);
        }

        public async Task SendSingleTemplateEmailMultipleRcptsAttachment(string from, List<EmailAddress> tos, string templateID, object templateData, CloudAppendBlob fileReference, string fileName)
        {
            var fromEmail = new EmailAddress(from);
            var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(fromEmail, tos, templateID, templateData);
            using (var stream = await fileReference.OpenReadAsync())
            {
                await msg.AddAttachmentAsync(fileName, stream);
            }
            await _client.SendEmailAsync(msg);
        }

        public async Task SendPasswordResetEmail(MessageNotification<PasswordResetEventBody> messageNotification)
        {
            var templateData = new EmailTemplate()
            {
                Data = new
                {
                    messageNotification.Recipient.FirstName,
                    messageNotification.Recipient.LastName,
                    messageNotification.EventBody.PasswordRenewalAccessToken,
                    messageNotification.EventBody.PasswordRenewalUrl
                }
            };
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, messageNotification.Recipient.Email, BUYER_PASSWORD_RESET_TEMPLATE_ID, templateData);
        }

        private List<object> CreateTemplateProductList(List<HSLineItem> lineItems, LineItemStatusChanges lineItemStatusChanges)
        {
            //  first get line items that actually had a change
            var changedLiIds = lineItemStatusChanges.Changes.Where(change => change.Quantity > 0).Select(change => change.ID);
            var changedLineItems = changedLiIds.Select(i => lineItems.Single(l => l.ID == i));
            //  now map to template data
            return changedLineItems.Select(lineItem =>
            {
                var lineItemStatusChange = lineItemStatusChanges.Changes.First(li => li.ID == lineItem.ID);
                return MapToTemplateProduct(lineItem, lineItemStatusChange);
            }).ToList();
        }

        private object MapToTemplateProduct(HSLineItem lineItem, LineItemStatusChange lineItemStatusChange)
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

        public async Task SendLineItemStatusChangeEmail(HSOrder order, LineItemStatusChanges lineItemStatusChanges, List<HSLineItem> lineItems, string firstName, string lastName, string email, EmailDisplayText lineItemEmailDisplayText)
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
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, email, LINE_ITEM_STATUS_CHANGE, templateData);
        }

        public async Task SendProductUpdateEmail(List<EmailAddress> tos, CloudAppendBlob fileReference, string fileName)
        {
            var yesterday = DateTime.Now.AddDays(-1).ToString();
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = new
                {
                    date = yesterday
                },
                Message = new EmailDisplayText()
                {
                    EmailSubject = $"Product Updates from {yesterday}",
                    DynamicText = $"Please see the attached file of newly created and updated products from {yesterday}"
                }
            };
            await SendSingleTemplateEmailMultipleRcptsAttachment(_settings.SendgridSettings.FromEmail, tos, PRODUCT_UPDATE_TEMPLATE_ID, templateData, fileReference, fileName);
        }

        public async Task SendLineItemStatusChangeEmailMultipleRcpts(HSOrder order, LineItemStatusChanges lineItemStatusChanges, List<HSLineItem> lineItems, List<EmailAddress> tos, EmailDisplayText lineItemEmailDisplayText)
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
            await SendSingleTemplateEmailMultipleRcpts(_settings.SendgridSettings.FromEmail, tos, LINE_ITEM_STATUS_CHANGE, templateData);
        }

        public async Task SendOrderSubmittedForApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification)
        {
            var order = messageNotification.EventBody.Order;
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = GetOrderTemplateData(order, messageNotification.EventBody.LineItems),
                Message = OrderSubmitEmailConstants.GetRequestedApprovalText()
            };
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, messageNotification.Recipient.Email, ORDER_APPROVAL_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderRequiresApprovalEmail(MessageNotification<OrderSubmitEventBody> messageNotification)
        {
            var order = messageNotification.EventBody.Order;
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = GetOrderTemplateData(order, messageNotification.EventBody.LineItems),
                Message = OrderSubmitEmailConstants.GetOrderRequiresApprovalText()
            };
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, messageNotification.Recipient.Email, ORDER_APPROVAL_TEMPLATE_ID, templateData);
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
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, messageNotification.Recipient.Email, BUYER_NEW_USER_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderApprovedEmail(MessageNotification<OrderSubmitEventBody> messageNotification)
        {
            var order = messageNotification.EventBody.Order;
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = GetOrderTemplateData(order, messageNotification.EventBody.LineItems),
                Message = OrderSubmitEmailConstants.GetOrderApprovedText()
            };
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, messageNotification.Recipient.Email, ORDER_APPROVAL_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderDeclinedEmail(MessageNotification<OrderSubmitEventBody> messageNotification)
        {
            var order = messageNotification.EventBody.Order;
            EmailTemplate templateData = new EmailTemplate()
            {
                Data = GetOrderTemplateData(order, messageNotification.EventBody.LineItems),
                Message = OrderSubmitEmailConstants.GetOrderDeclinedText()
            };
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, messageNotification.Recipient.Email, ORDER_APPROVAL_TEMPLATE_ID, templateData);
        }

        public async Task SendOrderSubmitEmail(HSOrderWorksheet orderWorksheet)
        {
            var supplierEmailList = await GetSupplierEmails(orderWorksheet);
            var firstName = orderWorksheet.Order.FromUser.FirstName;
            var lastName = orderWorksheet.Order.FromUser.LastName;
            if (orderWorksheet.Order.xp.OrderType == OrderType.Standard)
            {
                var orderData = GetOrderTemplateData(orderWorksheet.Order, orderWorksheet.LineItems);
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
                
                await SendSingleTemplateEmailMultipleRcpts(_settings.SendgridSettings.FromEmail, sellerEmailList, ORDER_SUBMIT_TEMPLATE_ID, sellerTemplateData);
                await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, orderWorksheet.Order.FromUser.Email, ORDER_SUBMIT_TEMPLATE_ID, buyerTemplateData);
                await SendSupplierOrderSubmitEmails(orderWorksheet);
            }
            else if (orderWorksheet.Order.xp.OrderType == OrderType.Quote)
            {
                var orderData = GetQuoteOrderTemplateData(orderWorksheet.Order, orderWorksheet.LineItems);

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
                await SendSingleTemplateEmailMultipleRcpts(_settings.SendgridSettings.FromEmail, supplierEmailList, QUOTE_ORDER_SUBMIT_TEMPLATE_ID, supplierTemplateData);
                await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, orderWorksheet.Order.FromUser.Email, QUOTE_ORDER_SUBMIT_TEMPLATE_ID, buyerTemplateData);
            }
        }

        private async Task SendSupplierOrderSubmitEmails(HSOrderWorksheet orderWorksheet)
        {
            ListPage<HSSupplier> suppliers = null;
            if (orderWorksheet.Order.xp.SupplierIDs != null)
            {
                var filterString = String.Join("|", orderWorksheet.Order.xp.SupplierIDs);
                suppliers = await _oc.Suppliers.ListAsync<HSSupplier>(filters: $"ID={filterString}");
            }
            foreach(var supplier in suppliers.Items)
            {
                if(supplier?.xp?.NotificationRcpts?.Count() >0)
                {
                    // get orderworksheet for supplier order and fill in some information from buyer order worksheet
                    var supplierOrderWorksheet = await BuildSupplierOrderWorksheet(orderWorksheet, supplier.ID);
                    EmailTemplate supplierTemplateData = new EmailTemplate()
                    {
                        Data = GetOrderTemplateData(supplierOrderWorksheet.Order, supplierOrderWorksheet.LineItems),
                        Message = OrderSubmitEmailConstants.GetOrderSubmitText(orderWorksheet.Order.ID, supplierOrderWorksheet.Order.FromUser.FirstName, supplierOrderWorksheet.Order.FromUser.LastName, VerifiedUserType.supplier)
                    };

                    // SEB-Specific Data
                    ((OrderTemplateData)supplierTemplateData.Data).BillTo = new Address()
                    {
                        CompanyName = "SEB Vendor Portal - BES",
                        Street1 = "8646 Eagle Creek Circle",
                        Street2 = "Suite 107",
                        City = "Savage",
                        State = "MN",
                        Zip = "55378",
                        Phone = "877-771-9123",
                        xp =
                        {
                            Email = "accounting@sebvendorportal.com"
                        }
                    };

                    var supplierTos = new List<EmailAddress>();
                    foreach (var rcpt in supplier.xp.NotificationRcpts)
                    {
                        supplierTos.Add(new EmailAddress(rcpt));
                    };
                    await SendSingleTemplateEmailMultipleRcpts(_settings.SendgridSettings.FromEmail, supplierTos, ORDER_SUBMIT_TEMPLATE_ID, supplierTemplateData);
                }   
            }
        }

        private async Task<HSOrderWorksheet> BuildSupplierOrderWorksheet(HSOrderWorksheet orderWorksheet, string supplierID)
        {
            var supplierOrderWorksheet = await _oc.IntegrationEvents.GetWorksheetAsync<HSOrderWorksheet>(OrderDirection.Outgoing, $"{orderWorksheet.Order.ID}-{supplierID}");
            supplierOrderWorksheet.Order.BillingAddress = orderWorksheet.Order.BillingAddress;
            supplierOrderWorksheet.Order.FromUser = orderWorksheet.Order.FromUser;
            return supplierOrderWorksheet;
        }

        private async Task<List<EmailAddress>> GetSupplierEmails(HSOrderWorksheet orderWorksheet)
        {
            ListPage<HSSupplier> suppliers = null;
            if (orderWorksheet.Order.xp.SupplierIDs != null)
            {
                var filterString = String.Join("|", orderWorksheet.Order.xp.SupplierIDs);
                suppliers = await _oc.Suppliers.ListAsync<HSSupplier>(filters: $"ID={filterString}");
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
            var sellerUsers = await _oc.AdminUsers.ListAsync<HSSellerUser>();
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

        public async Task SendLineItemStatusChangeEmail(LineItemStatusChange lineItemStatusChange, List<HSLineItem> lineItems, string firstName, string lastName, string email, EmailDisplayText lineItemEmailDisplayText)
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
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, email, LINE_ITEM_STATUS_CHANGE, templateData);
        }

        public async Task SendContactSupplierAboutProductEmail(ContactSupplierBody template)
        {
            var supplier = await _oc.Suppliers.GetAsync<HSSupplier>(template.Product.DefaultSupplierID);
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
            await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, supplierEmail, INFORMATION_REQUEST, templateData);
            var sellerUsers = await ListAllAsync.List((page) => _oc.AdminUsers.ListAsync<HSUser>(
                    filters: $"xp.RequestInfoEmails=true",
                    page: page,
                    pageSize: 100
                 ));
            foreach (var sellerUser in sellerUsers)
            {
                await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, sellerUser.Email, INFORMATION_REQUEST, templateData);
                if (sellerUser.xp.AddtlRcpts.Any())
                {
                    foreach (var rcpt in sellerUser.xp.AddtlRcpts)
                    {
                        await SendSingleTemplateEmail(_settings.SendgridSettings.FromEmail, rcpt, INFORMATION_REQUEST, templateData);
                    }
                }
            }
        }

        // helper functions 
        private List<string> GetSupplierInfo(ListPage<HSLineItem> lineItems)
        {
            var supplierList = lineItems.Items.Select(item => item.SupplierID)
                .Distinct()
                .ToList();
            return supplierList;
        }

        private OrderTemplateData GetOrderTemplateData(HSOrder order, IList<HSLineItem> lineItems)
        {
            var productsList = lineItems.Select(lineItem =>
            {
                return new ProductInfo()
                {
                    ProductName = lineItem.Product.Name,
                    ImageURL = lineItem.xp.ImageUrl,
                    ProductID = lineItem.ProductID,
                    Quantity = lineItem.Quantity,
                    LineTotal = lineItem.LineTotal,
                    SpecCombo = GetSpecCombo(lineItem.Specs)
                };
            });
            var shippingAddress = GetShippingAddress(lineItems);
            var currencyString = order.xp?.Currency?.ToString();
            return new OrderTemplateData()
            {
                FirstName = order.FromUser.FirstName,
                LastName = order.FromUser.LastName,
                OrderID = order.ID,
                DateSubmitted = order.DateSubmitted.ToString(),
                ShippingAddressID = order.ShippingAddressID,
                ShippingAddress = shippingAddress,
                BillingAddressID = order.BillingAddressID,
                BillingAddress = new Address()
                {
                    Street1 =order.BillingAddress?.Street1,
                    Street2 = order.BillingAddress?.Street2,
                    City = order.BillingAddress?.City,
                    State = order.BillingAddress?.State,
                    Zip = order.BillingAddress?.Zip
                },
                BillTo = null,
                Products = productsList,
                Subtotal =order.Subtotal,
                TaxCost = order.TaxCost,
                ShippingCost = order.ShippingCost,
                PromotionalDiscount = order.PromotionDiscount,
                Total = order.Total,
                Currency = currencyString
            };
        }

        private string GetSpecCombo (IList<LineItemSpec> specs)
        {
            if (specs == null || !specs.Any())
            {
                return null;
            }
            string specCombo = "(" + string.Join(", ", specs.Select(spec => spec.Value).ToArray()) + ")";
            return specCombo;
        }

        private QuoteOrderTemplateData GetQuoteOrderTemplateData(HSOrder order, IList<HSLineItem> lineItems)
        {
            return new QuoteOrderTemplateData()
            {
                FirstName = order.xp?.QuoteOrderInfo?.FirstName,
                LastName = order.xp?.QuoteOrderInfo?.LastName,
                Phone = order.xp?.QuoteOrderInfo?.Phone,
                Email = order.xp?.QuoteOrderInfo?.Email,
                Location = order.xp?.QuoteOrderInfo?.BuyerLocation,
                ProductID = lineItems.FirstOrDefault().Product.ID,
                ProductName = lineItems.FirstOrDefault().Product.Name,
                Order = order,
            };
        }

        private List<object> MapLineItemsToProducts(ListPage<HSLineItem> lineItems, string actionType)
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

        private ProductInfo MapReturnedLineItemToProduct(HSLineItem lineItem) =>
        new ProductInfo()
        {
            ProductName = lineItem.Product.Name,
            ImageURL = lineItem.xp.ImageUrl,
            ProductID = lineItem.ProductID,
            Quantity = lineItem.Quantity,
            LineTotal = lineItem.LineTotal,
        };

        private ProductInfo MapCanceledLineItemToProduct(HSLineItem lineItem) =>
        new ProductInfo()
        {
            ProductName = lineItem.Product.Name,
            ImageURL = lineItem.xp.ImageUrl,
            ProductID = lineItem.ProductID,
            Quantity = lineItem.Quantity,
            LineTotal = lineItem.LineTotal,
        };

        private ProductInfo MapLineItemToProduct(HSLineItem lineItem) =>
          new ProductInfo()
          {
              ProductName = lineItem.Product.Name,
              ImageURL = lineItem.xp.ImageUrl,
              ProductID = lineItem.ProductID,
              Quantity = lineItem.Quantity,
              LineTotal = lineItem.LineTotal,
          };

        private Address GetShippingAddress(IList<HSLineItem> lineItems)
        {
            return new Address()
            {
                Street1 = lineItems[0].ShippingAddress.Street1,
                Street2 = lineItems[0].ShippingAddress.Street2,
                City = lineItems[0].ShippingAddress.City,
                State = lineItems[0].ShippingAddress.State,
                Zip = lineItems[0].ShippingAddress.Zip
            };
        }
    }
}