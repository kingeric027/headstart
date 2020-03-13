using System.Collections;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Mappers.CardConnect;
using Marketplace.Common.Services.CardConnect;
using Marketplace.Common.Services.CardConnect.Models;
using Marketplace.Helpers.Exceptions;
using Marketplace.Models.Misc;
using Newtonsoft.Json.Bson;
using NSubstitute;
using NUnit.Framework;
using OrderCloud.SDK;

namespace Orchestration.Tests
{

    //public class Mocks
    //{
    //    public MarketplaceOrder MarketplaceOrder { get; set; }
    //    public WebhookPayloads.Orders.Submit WebhookOrder { get; set; }
    //    public OrderSplitResult SplitOrders { get; set; }

    //    public Mocks()
    //    {
    //        this.MarketplaceOrder = JObject.Parse(@"{
    //          'ID': 'RJg-ODINq0SY_CmJ8VoCvQ',
    //          'FromUser': {
    //            'ID': 'tIDpKXtFN0SH8rPbLmeu3w',
    //            'Username': 'erosen-buyer',
    //            'Password': null,
    //            'FirstName': 'Edmund',
    //            'LastName': 'Rosen',
    //            'Email': 'erosen@four51.com',
    //            'Phone': null,
    //            'TermsAccepted': null,
    //            'Active': true,
    //            'xp': {
    //              'FavoriteProducts': [
    //                'ss000b70487500',
    //                'ss000b07787500',
    //                '0a-test-product-dj',
    //                'ss000b13587350',
    //                'sanmar01123781'
    //              ]
    //            },
    //            'AvailableRoles': null,
    //            'DateCreated': '2020-01-29T23:01:05.627+00:00',
    //            'PasswordLastSetDate': '2020-01-29T23:01:05.663+00:00'
    //          },
    //          'FromCompanyID': 'Default_Marketplace_Buyer',
    //          'ToCompanyID': 'KrRmq3vC8EeME-srD_dXCw',
    //          'FromUserID': 'tIDpKXtFN0SH8rPbLmeu3w',
    //          'BillingAddressID': 'LZ5Ex1C6xESBLTl2ZtFApQ',
    //          'BillingAddress': {
    //            'ID': 'LZ5Ex1C6xESBLTl2ZtFApQ',
    //            'DateCreated': null,
    //            'CompanyName': null,
    //            'FirstName': 'ewrgweg',
    //            'LastName': 'wregwegw',
    //            'Street1': '2529 Garfield Ave',
    //            'Street2': '',
    //            'City': 'Minneapolis',
    //            'State': 'PA',
    //            'Zip': '19406',
    //            'Country': 'US',
    //            'Phone': '',
    //            'AddressName': null,
    //            'xp': null
    //          },
    //          'ShippingAddressID': 'T385pqwKbUqOG2iBAIDCzw',
    //          'Comments': '',
    //          'LineItemCount': 2,
    //          'Status': 'Open',
    //          'DateCreated': '2020-02-10T19:21:27.45+00:00',
    //          'DateSubmitted': '2020-02-10T19:25:02.133+00:00',
    //          'DateApproved': null,
    //          'DateDeclined': null,
    //          'DateCanceled': null,
    //          'DateCompleted': null,
    //          'Subtotal': 1918.56,
    //          'ShippingCost': 759.2,
    //          'TaxCost': 214.9,
    //          'PromotionDiscount': 0,
    //          'Total': 2892.66,
    //          'IsSubmitted': true,
    //          'xp': {
    //            'AvalaraTaxTransactionCode': '',
    //            'ProposedShipmentSelections': [
    //              {
    //                'SupplierID': 'FASTPlatform',
    //                'ShipFromAddressID': 'Fast-Warehouse-MN',
    //                'ProposedShipmentOptionID': 'FedexParcel-983b3727-c7ef-4701-bbf0-a6f3e86985c8',
    //                'Rate': 759.2
    //              }
    //            ]
    //          }
    //        }").ToObject<MarketplaceOrder>();
    //        this.WebhookOrder = JObject.Parse(@"{
    //            'Route': 'v1/orders/{direction}/{orderID}/submit',
    //            'RouteParams': {
    //                'direction': 'outgoing',
    //                'orderID': '1_PM1aJGkEerIDqE3sIQyg'
    //            },
    //            'Verb': 'POST',
    //            'Date': '2020-02-18T19:58:00.5145745+00:00',
    //            'LogID': 'dbc9507b-cb56-40dc-86e2-28efea0f096f',
    //            'UserToken': null,
    //            'Request': {
    //                'Body': {},
    //                'Headers': 'Connection: Keep-Alive\r\nContent-Length: 0\r\nAccept: application/json, text/plain, */*\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\ntokenClaims(from authorization): usr: Default_Buyer,cid: 7febc4e1-14c5-4050-b509-48e29794a584,imp: 1018,u: 2629734,usrtype: buyer,role: MeAddressAdmin,role: MeAdmin,role: MeCreditCardAdmin,role: MeXpAdmin,role: ProductFacetReader,role: Shopper,role: SupplierAddressReader,role: SupplierReader,iss: https://auth.ordercloud.io,aud: https://api.ordercloud.io,exp: 1582084636,nbf: 1582055836\r\nHost: api.ordercloud.io\r\nMax-Forwards: 10\r\nReferer: https://developer.ordercloud.io/console\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.106 Safari/537.36\r\nSec-Fetch-Dest: empty\r\nDNT: 1\r\nConsoleLog: true\r\nOrigin: https://developer.ordercloud.io\r\nSec-Fetch-Site: same-site\r\nSec-Fetch-Mode: cors\r\nX-WAWS-Unencoded-URL: /v1/orders/outgoing/1_PM1aJGkEerIDqE3sIQyg/submit\r\nCLIENT-IP: 207.225.155.218:53319\r\nX-ARR-LOG-ID: 00ab516f-649b-425d-9d26-03fc8942a46a\r\nDISGUISED-HOST: api.ordercloud.io\r\nX-SITE-DEPLOYMENT-ID: ordercloud-api\r\nWAS-DEFAULT-HOSTNAME: ordercloud-api.azurewebsites.net\r\nX-Original-URL: /v1/orders/outgoing/1_PM1aJGkEerIDqE3sIQyg/submit\r\nX-Forwarded-For: 207.225.155.218:53319\r\nX-ARR-SSL: 2048|256|C=US, S=Arizona, L=Scottsdale, O=\'GoDaddy.com, Inc.\', OU=http://certs.godaddy.com/repository/, CN=Go Daddy Secure Certificate Authority - G2|OU=Domain Control Validated, CN=*.ordercloud.io\r\nX-Forwarded-Proto: https\r\nX-AppService-Proto: https\r\n'
    //            },
    //            'Response': {
    //                'Body': {
    //                    'ID': '1_PM1aJGkEerIDqE3sIQyg',
    //                    'FromUser': {
    //                        'ID': 'UFWFO-k47kacGAJDB7Xtlw',
    //                        'Username': 'Default_Buyer',
    //                        'Password': '***',
    //                        'FirstName': 'BuyerFirstName',
    //                        'LastName': 'BuyerLastName',
    //                        'Email': 'buyeruser@noemail.com',
    //                        'Phone': null,
    //                        'TermsAccepted': null,
    //                        'Active': true,
    //                        'xp': null,
    //                        'AvailableRoles': null,
    //                        'DateCreated': '2020-01-23T14:48:54.723+00:00',
    //                        'PasswordLastSetDate': '2020-01-23T14:48:54.733+00:00'
    //                    },
    //                    'FromCompanyID': 'Default_Marketplace_Buyer',
    //                    'ToCompanyID': 'KrRmq3vC8EeME-srD_dXCw',
    //                    'FromUserID': 'UFWFO-k47kacGAJDB7Xtlw',
    //                    'BillingAddressID': '0df_HhldQU-u8V349X_d2g',
    //                    'BillingAddress': {
    //                        'ID': '0df_HhldQU-u8V349X_d2g',
    //                        'DateCreated': null,
    //                        'CompanyName': null,
    //                        'FirstName': 'Bill',
    //                        'LastName': 'Osteraas',
    //                        'Street1': '110 N 5th St',
    //                        'Street2': 'Floor 2',
    //                        'City': 'Minneapolis',
    //                        'State': 'MN',
    //                        'Zip': '19406',
    //                        'Country': 'US',
    //                        'Phone': '16024058666',
    //                        'AddressName': null,
    //                        'xp': null
    //                    },
    //                    'ShippingAddressID': '0df_HhldQU-u8V349X_d2g',
    //                    'Comments': 'Zoho integration mock',
    //                    'LineItemCount': 2,
    //                    'Status': 'Open',
    //                    'DateCreated': '2020-02-18T19:57:31.797+00:00',
    //                    'DateSubmitted': '2020-02-18T19:58:00.593+00:00',
    //                    'DateApproved': null,
    //                    'DateDeclined': null,
    //                    'DateCanceled': null,
    //                    'DateCompleted': null,
    //                    'Subtotal': 32.9,
    //                    'ShippingCost': 0,
    //                    'TaxCost': 0,
    //                    'PromotionDiscount': 0,
    //                    'Total': 32.9,
    //                    'IsSubmitted': true,
    //                    'xp': null
    //                },
    //                'Headers': 'Server: Microsoft-IIS/10.0\r\nX-oc-logid: dbc9507b-cb56-40dc-86e2-28efea0f096f\r\nAccess-Control-Allow-Origin: https://developer.ordercloud.io\r\nAccess-Control-Allow-Credentials: true\r\nLocation: https://api.ordercloud.io/v1/orders/outgoing/1_PM1aJGkEerIDqE3sIQyg/1_PM1aJGkEerIDqE3sIQyg\r\nContent-Length: 1246\r\nCache-Control: private\r\nX-AspNet-Version: 4.0.30319\r\nContent-Type: application/json; charset=utf-8\r\n'
    //            },
    //            'ConfigData': {}
    //        }").ToObject<WebhookPayloads.Orders.Submit>();
    //        this.SplitOrders = JObject.Parse(@"{
    //          'OutgoingOrders': [
    //            {
    //              'ID': 'XN5oGuqFbUmFm9UbmZoXWg',
    //              'FromUser': {
    //                'ID': 'uYoQe2OcuEWeWV857Li8yQ',
    //                'Username': 'Default_Admin',
    //                'Password': null,
    //                'FirstName': null,
    //                'LastName': null,
    //                'Email': 'serlleruser@nomail.com',
    //                'Phone': null,
    //                'TermsAccepted': null,
    //                'Active': true,
    //                'xp': null,
    //                'AvailableRoles': null,
    //                'DateCreated': '2020-01-23T14:48:54.88+00:00',
    //                'PasswordLastSetDate': '2020-01-29T22:38:15.68+00:00'
    //              },
    //              'FromCompanyID': 'KrRmq3vC8EeME-srD_dXCw',
    //              'ToCompanyID': 'FASTPlatform',
    //              'FromUserID': 'uYoQe2OcuEWeWV857Li8yQ',
    //              'BillingAddressID': null,
    //              'BillingAddress': null,
    //              'ShippingAddressID': '0df_HhldQU-u8V349X_d2g',
    //              'Comments': null,
    //              'LineItemCount': 1,
    //              'Status': 'Open',
    //              'DateCreated': '2020-02-18T21:52:18.98+00:00',
    //              'DateSubmitted': '2020-02-18T21:52:19.1+00:00',
    //              'DateApproved': null,
    //              'DateDeclined': null,
    //              'DateCanceled': null,
    //              'DateCompleted': null,
    //              'Subtotal': 21.4,
    //              'ShippingCost': 0,
    //              'TaxCost': 0,
    //              'PromotionDiscount': 0,
    //              'Total': 21.4,
    //              'IsSubmitted': true,
    //              'xp': null
    //            },
    //            {
    //              'ID': 'Kkmb1guSck6lpZQqS296hw',
    //              'FromUser': {
    //                'ID': 'uYoQe2OcuEWeWV857Li8yQ',
    //                'Username': 'Default_Admin',
    //                'Password': null,
    //                'FirstName': null,
    //                'LastName': null,
    //                'Email': 'serlleruser@nomail.com',
    //                'Phone': null,
    //                'TermsAccepted': null,
    //                'Active': true,
    //                'xp': null,
    //                'AvailableRoles': null,
    //                'DateCreated': '2020-01-23T14:48:54.88+00:00',
    //                'PasswordLastSetDate': '2020-01-29T22:38:15.68+00:00'
    //              },
    //              'FromCompanyID': 'KrRmq3vC8EeME-srD_dXCw',
    //              'ToCompanyID': 'ApparelCo',
    //              'FromUserID': 'uYoQe2OcuEWeWV857Li8yQ',
    //              'BillingAddressID': null,
    //              'BillingAddress': null,
    //              'ShippingAddressID': '0df_HhldQU-u8V349X_d2g',
    //              'Comments': null,
    //              'LineItemCount': 1,
    //              'Status': 'Open',
    //              'DateCreated': '2020-02-18T21:52:19.497+00:00',
    //              'DateSubmitted': '2020-02-18T21:52:19.603+00:00',
    //              'DateApproved': null,
    //              'DateDeclined': null,
    //              'DateCanceled': null,
    //              'DateCompleted': null,
    //              'Subtotal': 11.5,
    //              'ShippingCost': 0,
    //              'TaxCost': 0,
    //              'PromotionDiscount': 0,
    //              'Total': 11.5,
    //              'IsSubmitted': true,
    //              'xp': null
    //            }
    //          ],
    //          'RemainingLineItemIDs': []
    //        }").ToObject<OrderSplitResult>();
    //    }
    //}

    //public class ZohoTests
    //{
    //    private readonly Mocks _mocks = new Mocks();
    //    [SetUp]
    //    public void Setup()
    //    {

    //    }

    //    [Test]
    //    public async Task zoho_order_test()
    //    {
    //        var settings = Substitute.For<AppSettings>();
    //        settings.ZohoSettings = new ZohoSettings()
    //        {
    //            AuthToken = "90f15d12b441726b50524c79acbe8e12",
    //            OrgID = "708539139"
    //        };
    //        settings.OrderCloudSettings = new OrderCloudSettings()
    //        {
    //            ClientSecret = "d576450ca8f89967eea0d3477544ea4bee60af051a5c173be09db08c562b",
    //            ClientID = "97349A89-E279-45BE-A072-83DF8F7043F4",
    //            AuthUrl = "https://auth.ordercloud.io",
    //            ApiUrl = "https://api.ordercloud.io"
    //        };

    //        #region Create OC Order
    //        //var oc = new OrderCloudClient(new OrderCloudClientConfig()
    //        //{
    //        //    ApiUrl = settings.OrderCloudSettings.ApiUrl,
    //        //    AuthUrl = settings.OrderCloudSettings.AuthUrl,
    //        //    ClientSecret = settings.OrderCloudSettings.ClientSecret,
    //        //    ClientId = settings.OrderCloudSettings.ClientID,
    //        //    GrantType = GrantType.ClientCredentials,
    //        //    Roles = new [] { ApiRole.Shopper }
    //        //});
    //        //var token = await oc.AuthenticateAsync();
    //        //// submit an order to trigger webhook
    //        //var oc_order = await oc.Orders.CreateAsync(OrderDirection.Outgoing, new MarketplaceOrder()
    //        //{
    //        //    BillingAddressID = "0df_HhldQU-u8V349X_d2g",
    //        //    ShippingAddressID = "0df_HhldQU-u8V349X_d2g",
    //        //    Comments = "Zoho integration mock"
    //        //});

    //        //await oc.LineItems.CreateAsync(OrderDirection.Outgoing, oc_order.ID, new LineItem()
    //        //{
    //        //    ID = "fast_lineitem",
    //        //    ProductID = "L0708695142349",
    //        //    Quantity = 2,
    //        //    //ShipFromAddressID = "Fast-Warehouse-MN",
    //        //    Specs = new List<LineItemSpec>()
    //        //    {
    //        //        new LineItemSpec() { Name = "logo", SpecID = "fast_logo_id", Value = "logo_id"},
    //        //        new LineItemSpec() { Name = "color", SpecID = "fast_color_l0708695142349", OptionID = "fast_l0708695142349_wwy"},
    //        //        new LineItemSpec() { Name = "size", SpecID = "fast_size_l0708695142349", OptionID = "fast_l0708695142349_med"},
    //        //        new LineItemSpec() { Name = "decoration", SpecID = "fast_decoration_method", OptionID = "digital_vinyl_1d1dada1-5a5e-48af-9537-743ea861e290"},
    //        //        new LineItemSpec() { Name = "type", SpecID = "fast_decoration_type", OptionID = "name_b90c7bd1-fd1c-40b3-b6b8-79e6a45ab49a"},
    //        //        new LineItemSpec() { Name = "imprint", SpecID = "fast_imprint_location", OptionID = "above_the_pocket_3736c6cb-e78f-4ed4-a3ff-70d514fbcf69"}
    //        //    }
    //        //});

    //        //await oc.LineItems.CreateAsync(OrderDirection.Outgoing, oc_order.ID, new LineItem()
    //        //{
    //        //    ID = "apparelco_lineitem",
    //        //    ProductID = "sp724841371777",
    //        //    Quantity = 2,
    //        //    //ShipFromAddressID = "ApparelCo-Warehouse-MN",
    //        //    Specs = new List<LineItemSpec>()
    //        //    {
    //        //        new LineItemSpec() { Name = "logo", SpecID = "fast_logo_id", Value = "logo_id"},
    //        //        new LineItemSpec() { Name = "color", SpecID = "fast_color_sp724841371777", OptionID = "fast_sp724841371777_burgundy"},
    //        //        new LineItemSpec() { Name = "size", SpecID = "fast_size_sp724841371777", OptionID = "fast_sp724841371777_xl"},
    //        //        new LineItemSpec() { Name = "decoration", SpecID = "fast_decoration_method", OptionID = "digital_vinyl_0cfdee85-7186-42c9-9a82-6840d2459b4c"},
    //        //        new LineItemSpec() { Name = "type", SpecID = "fast_decoration_type", OptionID = "logo_0bfb0f67-1ec2-4028-aba8-bf4622a6625e"},
    //        //        new LineItemSpec() { Name = "imprint", SpecID = "fast_imprint_location", OptionID = "yoke_f57bb350-54bf-4ac2-a5d7-2c21c5669d07"}
    //        //    }
    //        //});

    //        // await oc.Orders.SubmitAsync(OrderDirection.Outgoing, oc_order.ID);
    //        #endregion 
    //        var command = new ZohoCommand(settings);

    //        var order = await command.CreateSalesOrder(_mocks.MarketplaceOrder);
    //        var purchase_orders = await command.CreatePurchaseOrder(order, _mocks.SplitOrders);
    //        Assert.IsTrue(purchase_orders.Count > 0);
    //    }
    //}

    public class CardConnectTests
    {
        private HttpTest _http;
        private AppSettings _settings;
        private CardConnectService _service;

        [SetUp]
        public void Setup()
        {
            _http = new HttpTest();
            _settings = new AppSettings()
            {
                CardConnectSettings = new CardConnectSettings()
                {
                    Authorization = "",
                    Site = "fts-uat",
                    BaseUrl = "cardconnect.com"
                }
            };
            _service = new CardConnectService(_settings);
        }

        [TearDown]
        public void DisposeHttp()
        {
            _http.Dispose();
        }

        [Test]
        public async Task verify_correct_auth_url_called()
        {
            _http.RespondWith(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'X'}");
            await _service.AuthWithoutCapture(new AuthorizationRequest());
            _http.ShouldHaveCalled("https://fts-uat.cardconnect.com/cardconnect/rest/auth");
        }

        [Test]
        public async Task verify_correct_tokenize_url_called()
        {
            _http.RespondWith(@"{'account': 'test'}");
            await _service.Tokenize(new AccountRequest());
            _http.ShouldHaveCalled("https://fts-uat.cardconnect.com/cardsecure/api/v1/ccn/tokenize");
        }

        [Test]
        [TestCase(@"{'respstat': 'A', 'respcode': '0', 'cvvresp': 'M', 'avsresp': 'X'}", ResponseStatus.Approved)]
        [TestCase(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'X'}", ResponseStatus.Approved)]
        [TestCase(@"{'respstat': 'A', 'respcode': '000', 'cvvresp': 'M', 'avsresp': 'X'}", ResponseStatus.Approved)]
        public async Task auth_success_attempt_test(string body, ResponseStatus result)
        {
            _http.RespondWith(body);
            var call = await _service.AuthWithoutCapture(new AuthorizationRequest());
            Assert.That(call.respstat.ToResponseStatus() == result);
        }

        [Test, TestCaseSource(typeof(ResponseCodeFactory), nameof(ResponseCodeFactory.FailCases))]
        public void auth_failure_attempt_tests(string body)
        {
            _http.RespondWith(body);
            var ex = Assert.ThrowsAsync<ApiErrorException>(() => _service.AuthWithoutCapture(new AuthorizationRequest()));
            Assert.That(ex.ApiError.StatusCode == HttpStatusCode.BadRequest);
        }

        [Test, TestCaseSource(typeof(CvvFactory), nameof(CvvFactory.CvvCases))]
        public void require_cvv_for_me_creditcards(CreditCardPayment payment, BuyerCreditCard cc, bool isValid)
        {
            Assert.That(payment.IsValidCvv(cc) == isValid);
        }
    }

    public static class CvvFactory
    {
        public static IEnumerable CvvCases
        {
            get
            {
                yield return new TestCaseData(new CreditCardPayment() {CVV = "112"}, new BuyerCreditCard() {Editable = true}, true);
                yield return new TestCaseData(new CreditCardPayment() {CVV = null}, new BuyerCreditCard() {Editable = true}, false);
                yield return new TestCaseData(new CreditCardPayment()
                {
                    CreditCardDetails = new CreditCardToken() { AccountNumber = "00"}, CVV = null
                }, new BuyerCreditCard() {Editable = true}, false);
                yield return new TestCaseData(new CreditCardPayment()
                {
                    CreditCardDetails = new CreditCardToken() { AccountNumber = "00" },
                    CVV = "112"
                }, new BuyerCreditCard() { Editable = true }, true);
            }
        }
    }

    public static class ResponseCodeFactory
    {
        public static IEnumerable FailCases
        {
            get { 
                yield return new TestCaseData(@"{'respstat': 'B', 'respcode': 'NU', 'cvvresp': 'M', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'C', 'respcode': '05', 'cvvresp': 'M', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'N', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'U', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'N'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'A'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'Z'}");
            }
        }
    }
}
