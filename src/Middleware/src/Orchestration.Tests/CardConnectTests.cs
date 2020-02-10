using System.Collections;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Marketplace.Common;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Mappers.CardConnect;
using Marketplace.Common.Services.CardConnect;
using Marketplace.Common.Services.CardConnect.Models;
using Marketplace.Common.Services.Zoho;
using Marketplace.Helpers.Exceptions;
using Marketplace.Models;
using Marketplace.Models.Misc;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using OrderCloud.SDK;

namespace Orchestration.Tests
{

    public class Mocks
    {
        public MarketplaceOrder MarketplaceOrder { get; set; }
        public WebhookPayloads.Orders.Submit WebhookOrder { get; set; }

        public Mocks()
        {
            this.MarketplaceOrder = JObject.Parse(@"{
                'ID': 'XD0J6GR9f0Gyp3I6u9RphQ',
                'FromUser': {
                    'ID': 'scaseybuyer',
                    'Username': 'scaseybuyer',
                    'Password': null,
                    'FirstName': 'Sydney',
                    'LastName': 'Casey',
                    'Email': 'scasey@four51.com',
                    'Phone': '',
                    'TermsAccepted': '2018-01-01T06:00:00+00:00',
                    'Active': true,
                    'xp': { },
                    'AvailableRoles': null,
                    'DateCreated': '2020-02-05T22:30:52.59+00:00',
                    'PasswordLastSetDate': '2020-02-05T22:30:52.593+00:00'
                },
                'FromCompanyID': 'Default_Marketplace_Buyer',
                'ToCompanyID': 'KrRmq3vC8EeME-srD_dXCw',
                'FromUserID': 'scaseybuyer',
                'BillingAddressID': null,
                'BillingAddress': null,
                'ShippingAddressID': null,
                'Comments': null,
                'LineItemCount': 1,
                'Status': 'Unsubmitted',
                'DateCreated': '2020-02-06T20:32:14.193+00:00',
                'DateSubmitted': null,
                'DateApproved': null,
                'DateDeclined': null,
                'DateCanceled': null,
                'DateCompleted': null,
                'Subtotal': 14.45,
                'ShippingCost': 0,
                'TaxCost': 0,
                'PromotionDiscount': 0,
                'Total': 14.45,
                'IsSubmitted': false,
                'xp': {
                    'AvalaraTaxTransactionCode': '',
                    'ProposedShipmentSelections': []
                }
            }").ToObject<MarketplaceOrder>();
            this.WebhookOrder = JObject.Parse(@"{
                'Route': 'v1/orders/{direction}/{orderID}/submit',
                'RouteParams': {
                            'direction': 'outgoing',
                    'orderID': 'LfMJE74kvUOWrCiNpyI2-Q'
                },
                'Verb': 'POST',
                'Date': '2020-02-06T22:39:49.2779142+00:00',
                'LogID': 'd947fcb9-10e1-4c3b-befe-d1496a64a6cb',
                'UserToken': null,
                'Request': {
                            'Body': { },
                    'Headers': 'Connection: Keep-Alive\r\nContent-Length: 0\r\nAccept: application/json\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\ntokenClaims(from authorization): usr: bhickey@four51.com,cid: 3a5dd92d-0b04-4e62-b8ac-197adf10fbc4,u: 2633253,usrtype: buyer,role: MeAddressAdmin,role: MeAdmin,role: MeCreditCardAdmin,role: MeXpAdmin,role: Shopper,role: SupplierReader,iss: https://auth.ordercloud.io,aud: https://api.ordercloud.io,exp: 1581064641,nbf: 1581028641\r\nHost: api.ordercloud.io\r\nMax-Forwards: 10\r\nReferer: https://marketplace-buyer-ui-qa.azurewebsites.net/checkout\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.87 Safari/537.36\r\nSec-Fetch-Dest: empty\r\nOrigin: https://marketplace-buyer-ui-qa.azurewebsites.net\r\nSec-Fetch-Site: cross-site\r\nSec-Fetch-Mode: cors\r\nX-WAWS-Unencoded-URL: /v1/orders/outgoing/LfMJE74kvUOWrCiNpyI2-Q/submit\r\nCLIENT-IP: 207.225.155.218:56234\r\nX-ARR-LOG-ID: e34feccb-0af9-4a2a-9122-ce38ed2f2593\r\nDISGUISED-HOST: api.ordercloud.io\r\nX-SITE-DEPLOYMENT-ID: ordercloud-api__3c42\r\nWAS-DEFAULT-HOSTNAME: ordercloud-api.azurewebsites.net\r\nX-Original-URL: /v1/orders/outgoing/LfMJE74kvUOWrCiNpyI2-Q/submit\r\nX-Forwarded-For: 207.225.155.218:56234\r\nX-ARR-SSL: 2048|256|C=US, S=Arizona, L=Scottsdale, O=\'GoDaddy.com, Inc.\', OU=http://certs.godaddy.com/repository/, CN=Go Daddy Secure Certificate Authority - G2|OU=Domain Control Validated, CN=*.ordercloud.io\r\nX-Forwarded-Proto: https\r\nX-AppService-Proto: https\r\n'
                },
                'Response': {
                            'Body': {
                                'ID': 'LfMJE74kvUOWrCiNpyI2-Q',
                        'FromUser': {
                                    'ID': 'Zfa0q5nMIEaQkAXymPRBGQ',
                            'Username': 'bhickey@four51.com',
                            'Password': '***',
                            'FirstName': 'Bill',
                            'LastName': 'Hickey',
                            'Email': 'bhickey@four51.com',
                            'Phone': null,
                            'TermsAccepted': null,
                            'Active': true,
                            'xp': null,
                            'AvailableRoles': null,
                            'DateCreated': '2020-01-28T22:19:51.1+00:00',
                            'PasswordLastSetDate': '2020-01-28T22:20:37.48+00:00'
                        },
                        'FromCompanyID': 'Default_Marketplace_Buyer',
                        'ToCompanyID': 'KrRmq3vC8EeME-srD_dXCw',
                        'FromUserID': 'Zfa0q5nMIEaQkAXymPRBGQ',
                        'BillingAddressID': '1DXEx_8ISUysib2GjjRf2A',
                        'BillingAddress': {
                                    'ID': '1DXEx_8ISUysib2GjjRf2A',
                            'DateCreated': null,
                            'CompanyName': null,
                            'FirstName': 'asdf',
                            'LastName': 'fasd',
                            'Street1': 'asdf',
                            'Street2': '',
                            'City': 'fsda',
                            'State': 'PA',
                            'Zip': '19406',
                            'Country': 'US',
                            'Phone': '',
                            'AddressName': null,
                            'xp': null
                        },
                        'ShippingAddressID': '99PF2PX_RkSPwrY3ZMMcUw',
                        'Comments': '',
                        'LineItemCount': 3,
                        'Status': 'Open',
                        'DateCreated': '2020-02-06T22:38:05.56+00:00',
                        'DateSubmitted': '2020-02-06T22:39:49.34+00:00',
                        'DateApproved': null,
                        'DateDeclined': null,
                        'DateCanceled': null,
                        'DateCompleted': null,
                        'Subtotal': 39.79,
                        'ShippingCost': 27.83,
                        'TaxCost': 5.33,
                        'PromotionDiscount': 0,
                        'Total': 72.95,
                        'IsSubmitted': true,
                        'xp': {
                                    'AvalaraTaxTransactionCode': '',
                            'ProposedShipmentSelections': [
                                {
                                    'SupplierID': 'FASTPlatform',
                                    'ShipFromAddressID': 'Fast-Warehouse-MN',
                                    'ShippingRateID': null,
                                    'Rate': 19.02
                                },
                                {
                                    'SupplierID': 'ApparelCo',
                                    'ShipFromAddressID': 'ApparelCo-Warehouse-MN',
                                    'ShippingRateID': null,
                                    'Rate': 8.81
                                }
                            ]
                        }
                    },
                    'Headers': 'Server: Microsoft-IIS/10.0\r\nX-oc-logid: d947fcb9-10e1-4c3b-befe-d1496a64a6cb\r\nAccess-Control-Allow-Origin: https://marketplace-buyer-ui-qa.azurewebsites.net\r\nAccess-Control-Allow-Credentials: true\r\nLocation: https://api.ordercloud.io/v1/orders/outgoing/LfMJE74kvUOWrCiNpyI2-Q/LfMJE74kvUOWrCiNpyI2-Q\r\nContent-Length: 1551\r\nCache-Control: private\r\nX-AspNet-Version: 4.0.30319\r\nContent-Type: application/json; charset=utf-8\r\n'
                },
                'ConfigData': []
            }").ToObject<WebhookPayloads.Orders.Submit>();
        }
    }

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
    //        var command = new ZohoCommand(settings, new ZohoClient(settings), new OrderCloudClient());
    //        await command.ReceiveBuyerOrder(_mocks.WebhookOrder.Response.Body);
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
    }

    public class ResponseCodeFactory
    {
        public static IEnumerable FailCases
        {
            get { 
                yield return new TestCaseData(@"{'respstat': 'B', 'respcode': 'NU', 'cvvresp': 'M', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'C', 'respcode': '05', 'cvvresp': 'M', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'N', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'P', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'U', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'N'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'A'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'Z'}");
            }
        }
    }
}
