using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services;
using ordercloud.integrations.avalara;
using OrderCloud.SDK;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models;
using Marketplace.Common;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.Common;
using SendGrid.Helpers.Mail;
using System.Dynamic;
using NSubstitute.Extensions;
using AutoFixture;

namespace Marketplace.Tests
{
    class SendgridTests
    {
        private IOrderCloudClient _oc;
        private AppSettings _settings;
        private ISendgridService _command;
        private const string ORDER_SUBMIT_TEMPLATE_ID = "order_submit_template_id";
        private const string LINE_ITEM_STATUS_CHANGE = "line_item_status_change";
        private const string QUOTE_ORDER_SUBMIT_TEMPLATE_ID = "quote_order_submit_template_id";
        private const string BUYER_NEW_USER_TEMPLATE_ID = "buyer_new_user_template_id";
        private const string BUYER_PASSWORD_RESET_TEMPLATE_ID = "buyer_password_reset_template_id";
        private const string INFORMATION_REQUEST = "information_request";
        private const string PRODUCT_UPDATE_TEMPLATE_ID = "product_update_template_id";

        [SetUp]
        public void Setup()
        {
            _oc = Substitute.For<IOrderCloudClient>();
            _settings = Substitute.For<AppSettings>();
            _command = new SendgridService(_settings, _oc);
        }

        public class TestConstants
        {
            public const string orderID = "testorder";
            public const string buyerEmail = "buyer@test.com";
            public const string lineItem1ID = "testlineitem1";
            public const string lineItem2ID = "testlineitem2";
            public const decimal lineItem1Total = 15;
            public const decimal lineItem2Total = 10;
            public const string product1ID = "testproduct1";
            public const string product1Name = "shirt";
            public const string product2ID = "testproduct2";
            public const string product2Name = "pants";
            public const string supplier1ID = "001";
            public static readonly string[] supplier1NotificationRcpts = { "001user@test.com", "001user2@test.com" };
            public static readonly string[] supplier2NotificationRcpts = { "002user@test.com" };
            public const string supplier2ID = "002";
            public const string selectedShipMethod1ID = "selectedmethod001";
            public const string selectedShipMethod2ID = "selectedmethod002";
            public const decimal selectedShipMethod1Cost = 10;
            public const decimal selectedShipMethod2Cost = 15;
            public const string sellerUser1email = "selleruser1@test.com";
            public static readonly string[] sellerUser1AdditionalRcpts = { "additionalrecipient1@test.com" };
            public const string selleruser2email = "selleruser2@test.com";
            public const decimal lineItem1Tax = 5;
            public const decimal lineItem2Tax = 7;
            public const decimal lineItem1ShipmentTax = 2;
            public const decimal lineItem2ShipmentTax = 2;
        }

        [Test]
        public async Task TestOrderSubmitEmail()
        {
            var orderWorksheet = GetOrderWorksheet();
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Outgoing, $"{TestConstants.orderID}-{TestConstants.supplier1ID}").Returns(GetSupplierWorksheet(TestConstants.supplier1ID, TestConstants.lineItem1ID, TestConstants.lineItem1Total));
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Outgoing, $"{TestConstants.orderID}-{TestConstants.supplier2ID}").Returns(GetSupplierWorksheet(TestConstants.supplier2ID, TestConstants.lineItem2ID, TestConstants.lineItem2Total));
            _oc.Suppliers.ListAsync<MarketplaceSupplier>(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(GetSupplierList()));
            _oc.AdminUsers.ListAsync<MarketplaceSellerUser>().ReturnsForAnyArgs(Task.FromResult(GetSellerUserList()));
            var _commandSub = Substitute.ForPartsOf<SendgridService>(_settings, _oc);
            _commandSub.Configure().WhenForAnyArgs(x => x.SendSingleTemplateEmailMultipleRcpts(default, default, default, default)).DoNotCallBase();
            _commandSub.Configure().WhenForAnyArgs(x => x.SendSingleTemplateEmail(default, default, default, default)).DoNotCallBase();

            //act
            await _commandSub.SendOrderSubmitEmail(orderWorksheet);

            //assert
            var expectedSellerEmailList = new List<EmailAddress>()
            {
                new EmailAddress() { Email = TestConstants.sellerUser1email},
                new EmailAddress() { Email = TestConstants.sellerUser1AdditionalRcpts[0] }
            };
            var expectedSupplier1EmailList = new List<EmailAddress>()
            {
                new EmailAddress() { Email=TestConstants.supplier1NotificationRcpts[0] },
                new EmailAddress() { Email=TestConstants.supplier1NotificationRcpts[1] },
            };
            var expectedSupplier2EmailList = new List<EmailAddress>()
            {
                new EmailAddress() { Email=TestConstants.supplier2NotificationRcpts[0] }
            };
            //  confirm emails sent to buyer, seller users, supplier 1 notification recipients, supplier 2 notification recipients
            await _commandSub.Configure().Received().SendSingleTemplateEmail(Arg.Any<string>(), TestConstants.buyerEmail, Arg.Any<string>(), Arg.Any<object>());
            await _commandSub.Configure().Received().SendSingleTemplateEmailMultipleRcpts(Arg.Any<string>(), Arg.Is<List<EmailAddress>>(x => EqualEmailLists(x, expectedSellerEmailList)), Arg.Any<string>(), Arg.Any<object>());
            await _commandSub.Configure().Received().SendSingleTemplateEmailMultipleRcpts(Arg.Any<string>(), Arg.Is<List<EmailAddress>>(x => EqualEmailLists(x, expectedSupplier1EmailList)), Arg.Any<string>(), Arg.Any<object>());
            await _commandSub.Configure().Received().SendSingleTemplateEmailMultipleRcpts(Arg.Any<string>(), Arg.Is<List<EmailAddress>>(x => EqualEmailLists(x, expectedSupplier2EmailList)), Arg.Any<string>(), Arg.Any<object>());

        }

        private bool EqualEmailLists(List<EmailAddress> list1, List<EmailAddress> list2)
        {
            if (list1.Count() != list2.Count())
            {
                return false;
            }
            else
            {
                var isEqual = true;
                var list2Emails = list2.Select(item => item.Email);
                var list1Emails = list1.Select(item => item.Email);
                foreach (var item in list1)
                {
                    if (!list2Emails.Contains(item.Email))
                    {
                        isEqual = false;
                    }
                }
                foreach (var item in list2)
                {
                    if (!list1Emails.Contains(item.Email))
                    {
                        isEqual = false;
                    }
                }
                return isEqual;
            }
        }


        private MarketplaceOrderWorksheet GetOrderWorksheet()
        {
            Fixture fixture = new Fixture();

            dynamic shipEstimatexp1 = new ExpandoObject();
            dynamic shipEstimatexp2 = new ExpandoObject();
            shipEstimatexp1.SupplierID = TestConstants.supplier1ID;
            shipEstimatexp2.SupplierID = TestConstants.supplier2ID;

            dynamic OrderCalculateXp = new ExpandoObject();
            dynamic TaxResponse = new ExpandoObject();
            dynamic lines = new List<ExpandoObject>();
            dynamic lineItem1Taxline = new ExpandoObject();
            dynamic lineItem2Taxline = new ExpandoObject();
            dynamic selectedShipMethod1Taxline = new ExpandoObject();
            dynamic selectedShipMethod2Taxline = new ExpandoObject();

            lineItem1Taxline.lineNumber = TestConstants.lineItem1ID;
            lineItem1Taxline.tax = TestConstants.lineItem1Tax;
            lineItem2Taxline.lineNumber = TestConstants.lineItem2ID;
            lineItem2Taxline.tax = TestConstants.lineItem2Tax;
            selectedShipMethod1Taxline.lineNumber = TestConstants.selectedShipMethod1ID;
            selectedShipMethod1Taxline.tax = TestConstants.lineItem1ShipmentTax;
            selectedShipMethod2Taxline.lineNumber = TestConstants.selectedShipMethod2ID;
            selectedShipMethod2Taxline.tax = TestConstants.lineItem2ShipmentTax;

            lines.Add(lineItem1Taxline);
            lines.Add(lineItem2Taxline);
            lines.Add(selectedShipMethod1Taxline);
            lines.Add(selectedShipMethod2Taxline);
            TaxResponse.lines = lines;
            OrderCalculateXp.TaxResponse = TaxResponse;

            return new MarketplaceOrderWorksheet()
            {
                Order = new MarketplaceOrder()
                {
                    ID = TestConstants.orderID,
                    FromUser = new MarketplaceUser()
                    {
                        FirstName = "john",
                        LastName = "johnson",
                        Email = TestConstants.buyerEmail
                    },
                    BillingAddressID = "testbillingaddressid",
                    BillingAddress = fixture.Create<MarketplaceAddressBuyer>(),
                    xp = new OrderXp()
                    {
                        OrderType = OrderType.Standard,
                        SupplierIDs = new List<string>()
                        {
                            TestConstants.supplier1ID,
                            TestConstants.supplier2ID
                        },
                        Currency = ordercloud.integrations.exchangerates.CurrencySymbol.USD
                    },
                    DateSubmitted = new DateTimeOffset()
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem()
                    {
                        ID = TestConstants.lineItem1ID,
                        ProductID = TestConstants.product1ID,
                        Quantity=1,
                        LineTotal = TestConstants.lineItem1Total,
                        Product = new MarketplaceLineItemProduct()
                        {
                            Name=TestConstants.product1Name
                        },
                        ShippingAddress = fixture.Create<MarketplaceAddressBuyer>(),
                        xp = fixture.Create<LineItemXp>(),
                    },
                    new MarketplaceLineItem()
                    {
                        ID = TestConstants.lineItem2ID,
                        ProductID = TestConstants.product2ID,
                        Quantity=1,
                        LineTotal = TestConstants.lineItem2Total,
                        Product = new MarketplaceLineItemProduct()
                        {
                            Name=TestConstants.product2Name
                        },
                        ShippingAddress = fixture.Create<MarketplaceAddressBuyer>(),
                        xp = fixture.Create<LineItemXp>()
                    }
                },
                ShipEstimateResponse = new MarketplaceShipEstimateResponse()
                {
                    ShipEstimates = new List<MarketplaceShipEstimate>()
                    {
                        new MarketplaceShipEstimate()
                        {
                            SelectedShipMethodID=TestConstants.selectedShipMethod1ID,
                            xp = shipEstimatexp1,
                            ShipMethods = new List<MarketplaceShipMethod>()
                            {
                                new MarketplaceShipMethod()
                                {
                                    ID=TestConstants.selectedShipMethod1ID,
                                    Cost=TestConstants.selectedShipMethod1Cost
                                },
                                fixture.Create<MarketplaceShipMethod>()
                            }
                        },
                        new MarketplaceShipEstimate()
                        {
                            SelectedShipMethodID=TestConstants.selectedShipMethod2ID,
                            xp = shipEstimatexp2,
                            ShipMethods = new List<MarketplaceShipMethod>()
                            {
                                new MarketplaceShipMethod()
                                {
                                    ID=TestConstants.selectedShipMethod2ID,
                                    Cost=TestConstants.selectedShipMethod2Cost
                                },
                                fixture.Create<MarketplaceShipMethod>()
                            }
                        }
                    }
                },
                OrderCalculateResponse = new OrderCalculateResponse()
                {
                    xp = OrderCalculateXp
                }
            };
        }

        private MarketplaceOrderWorksheet GetSupplierWorksheet(string supplierID, string lineItemID, decimal total)
        {
            Fixture fixture = new Fixture();
            return new MarketplaceOrderWorksheet()
            {
                Order = new MarketplaceOrder()
                {
                    ID = $"{TestConstants.orderID}-{supplierID}",
                    Total = total
                },
                LineItems = new List<MarketplaceLineItem>()
                {
                    new MarketplaceLineItem()
                    {
                        ID = lineItemID,
                        Quantity =1,
                        LineTotal=total,
                        ProductID = lineItemID == TestConstants.lineItem1ID ? TestConstants.product1ID : TestConstants.product2ID,
                        Product = new MarketplaceLineItemProduct()
                        {
                            Name = lineItemID == TestConstants.lineItem1ID ? TestConstants.product1Name : TestConstants.product2Name
                        },
                        xp = fixture.Create<LineItemXp>(),
                        ShippingAddress = fixture.Create<MarketplaceAddressBuyer>()
                    }
                }
            };
        }

        private ListPage<MarketplaceSupplier> GetSupplierList()
        {
            return new ListPage<MarketplaceSupplier>()
            {
                Items = new List<MarketplaceSupplier>()
                {
                    new MarketplaceSupplier()
                    {
                        ID = TestConstants.supplier1ID,
                        xp = new SupplierXp()
                        {
                            NotificationRcpts = TestConstants.supplier1NotificationRcpts.ToList()
                        }
                    },
                    new MarketplaceSupplier()
                    {
                        ID = TestConstants.supplier2ID,
                        xp = new SupplierXp()
                        {
                            NotificationRcpts = TestConstants.supplier2NotificationRcpts.ToList()
                        }
                    }
                }
            };
        }

        private ListPage<MarketplaceSellerUser> GetSellerUserList()
        {
            return new ListPage<MarketplaceSellerUser>()
            {
                Items = new List<MarketplaceSellerUser>()
                {
                    new MarketplaceSellerUser()
                    {
                        ID="selleruser1",
                        Email=TestConstants.sellerUser1email,
                        xp=new SellerUserXp()
                        {
                            OrderEmails = true,
                            AddtlRcpts = TestConstants.sellerUser1AdditionalRcpts.ToList()
                        }
                    },
                    new MarketplaceSellerUser()
                    {
                        ID="selleruser1",
                        Email=TestConstants.selleruser2email,
                        xp=new SellerUserXp()
                        {
                            OrderEmails = false
                        }
                    }
                }
            };
        }

    }
}
