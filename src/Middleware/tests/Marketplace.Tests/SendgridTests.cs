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

        //[SetUp]
        //public void Setup()
        //{
        //    _oc = Substitute.For<IOrderCloudClient>();
        //    _settings = Substitute.For<AppSettings>();
        //    _command = new SendgridService(_settings, _oc);

        //}

        public class TestConstants {
            public const string orderID = "testorder";
            public const string lineItem1ID = "testlineitem1";
            public const string lineItem2ID = "testlineitem2";
            public const decimal lineItem1Total = 15;
            public const decimal lineItem2Total = 10;
            public const string product1ID = "testproduct1";
            public const string product1Name = "shirt";
            public const string product2ID = "testproduct2";
            public const string product2Name = "pants";
            public const string supplier1ID = "001";
            public static readonly string[] supplier1NotificationRcpts = {"001user@test.com", "001user2@test.com"};
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
            var _oc = Substitute.For<IOrderCloudClient>();
            var _settings = Substitute.For<AppSettings>();

            var orderWorksheet = GetOrderWorksheet();
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Outgoing, $"{TestConstants.orderID}-{TestConstants.supplier1ID}").Returns(GetSupplierWorksheet(TestConstants.supplier1ID, TestConstants.lineItem1ID, TestConstants.lineItem1Total));
            _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Outgoing, $"{TestConstants.orderID}-{TestConstants.supplier2ID}").Returns(GetSupplierWorksheet(TestConstants.supplier2ID, TestConstants.lineItem2ID, TestConstants.lineItem2Total));
            _oc.Suppliers.ListAsync<MarketplaceSupplier>(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(GetSupplierList()));
            _oc.AdminUsers.ListAsync<MarketplaceSellerUser>().ReturnsForAnyArgs(Task.FromResult(GetSellerUserList()));
            var _commandSub = Substitute.ForPartsOf<SendgridService>(_settings, _oc);
            _commandSub.WhenForAnyArgs(x => x.SendSingleTemplateEmailMultipleRcpts(default, default, default, default)).DoNotCallBase();
            //_commandSub.Configure().SendSingleTemplateEmailMultipleRcpts(Arg.Any<string>(), Arg.Any<List<EmailAddress>>(), Arg.Any<string>(), Arg.Any<object>()).Returns(Task.FromResult);

            await _commandSub.SendOrderSubmitEmail(orderWorksheet);

            //assert
            await _commandSub.Received().SendSingleTemplateEmailMultipleRcpts(Arg.Any<string>(), Arg.Any<List<EmailAddress>>(), Arg.Any<string>(), Arg.Any<object>());
        }


        private MarketplaceOrderWorksheet GetOrderWorksheet()
        {
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
                        LastName = "johnson"
                    },
                    BillingAddressID="testbillingaddressid",
                    BillingAddress = new MarketplaceAddressBuyer()
                    {
                        ID = "testbillingaddress",
                        Street1 = "210 lakewood drive",
                        City = "springfield",
                        State = "MN"
                    },
                    xp = new OrderXp()
                    {
                        OrderType = OrderType.Standard,
                        SupplierIDs = new List<string>()
                        {
                            TestConstants.supplier1ID,
                            TestConstants.supplier2ID
                        }
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
                        ShippingAddress = new MarketplaceAddressBuyer()
                        {
                            Street1 = "234 Hennepin Ave",
                            City = "Minneapolis",
                            State = "MN"
                        },
                        xp=new LineItemXp()
                        {
                            ImageUrl="image.jpg"
                        }
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
                        ShippingAddress = new MarketplaceAddressBuyer()
                        {
                            Street1 = "234 Hennepin Ave",
                            City = "Minneapolis",
                            State = "MN"
                        },
                        xp=new LineItemXp()
                        {
                            ImageUrl="image.jpg"
                        }
                    }
                },
                ShipEstimateResponse = new ShipEstimateResponse()
                {
                    ShipEstimates = new List<ShipEstimate>()
                    {
                        new ShipEstimate()
                        {
                            SelectedShipMethodID=TestConstants.selectedShipMethod1ID,
                            xp = shipEstimatexp1,
                            ShipMethods = new List<ShipMethod>()
                            {
                                new ShipMethod()
                                {
                                    ID=TestConstants.selectedShipMethod1ID,
                                    Cost=TestConstants.selectedShipMethod1Cost
                                },
                                new ShipMethod()
                                {
                                    ID="unselectedmethod001",
                                    Cost=15
                                }
                            }
                        },
                        new ShipEstimate()
                        {
                            SelectedShipMethodID=TestConstants.selectedShipMethod2ID,
                            xp = shipEstimatexp2,
                            ShipMethods = new List<ShipMethod>()
                            {
                                new ShipMethod()
                                {
                                    ID=TestConstants.selectedShipMethod2ID,
                                    Cost=TestConstants.selectedShipMethod2Cost
                                },
                                new ShipMethod()
                                {
                                    ID="unselectedmethod002",
                                    Cost=15
                                }
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
                        xp = new LineItemXp()
                        {
                            ImageUrl="image.jpg"
                        },
                        ShippingAddress = new MarketplaceAddressBuyer()
                        {
                            Street1="street",
                            City="city",
                            State="state"
                        }
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
