using System.Collections;
using System.Threading.Tasks;
using Flurl.Http.Configuration;
using Flurl.Http.Testing;
using NUnit.Framework;
using ordercloud.integrations.cardconnect;

namespace CardConnect.Tests
{
    public class CardConnectTests
    {
        private HttpTest _http;
        private OrderCloudIntegrationsCardConnectService _service;

        [SetUp]
        public void Setup()
        {
            _http = new HttpTest();
            _service = new OrderCloudIntegrationsCardConnectService(new OrderCloudIntegrationsCardConnectConfig()
            {
                Authorization = "",
                Site = "fts-uat",
                BaseUrl = "cardconnect.com"
            }, new PerBaseUrlFlurlClientFactory());
        }

        [TearDown]
        public void DisposeHttp()
        {
            _http.Dispose();
        }

        [Test]
        public void to_credit_card_display_test()
        {
            var visa = "4444333322221111".ToCreditCardDisplay();
            Assert.AreEqual("1111", visa);

            var amex = "373485467448025".ToCreditCardDisplay();
            Assert.AreEqual("8025", amex);
        }

        [Test]
        public async Task verify_correct_auth_url_called()
        {
            _http.RespondWith(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'X'}");
            await _service.AuthWithoutCapture(new CardConnectAuthorizationRequest());
            _http.ShouldHaveCalled("https://fts-uat.cardconnect.com/cardconnect/rest/auth");
        }

        [Test]
        public async Task verify_correct_tokenize_url_called()
        {
            _http.RespondWith(@"{'account': 'test'}");
            await _service.Tokenize(new CardConnectAccountRequest());
            _http.ShouldHaveCalled("https://fts-uat.cardconnect.com/cardsecure/api/v1/ccn/tokenize");
        }

        [Test]
        [TestCase(@"{'respstat': 'A', 'respcode': '0', 'cvvresp': 'M', 'avsresp': 'X'}", ResponseStatus.Approved)]
        [TestCase(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'X'}", ResponseStatus.Approved)]
        [TestCase(@"{'respstat': 'A', 'respcode': '000', 'cvvresp': 'M', 'avsresp': 'X'}", ResponseStatus.Approved)]
        [TestCase(@"{'respstat': 'A', 'respcode': '0', 'cvvresp': 'N', 'avsresp': 'Z'}", ResponseStatus.Approved)]
        public async Task auth_success_attempt_test(string body, ResponseStatus result)
        {
            _http.RespondWith(body);
            var call = await _service.AuthWithoutCapture(new CardConnectAuthorizationRequest());
            Assert.That(call.respstat.ToResponseStatus() == result);
        }

        [Test, TestCaseSource(typeof(ResponseCodeFactory), nameof(ResponseCodeFactory.FailCases))]
        public void auth_failure_attempt_tests(string body)
        {
            _http.RespondWith(body);
            var ex = Assert.ThrowsAsync<CreditCardIntegrationException>(() => _service.AuthWithoutCapture(new CardConnectAuthorizationRequest() { cvv2 = "112" }));
        }

        [Test]
        [TestCase(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': 'P', 'avsresp': 'Y'}", ResponseStatus.Approved)]
        [TestCase(@"{'respstat': 'A', 'respcode': '00', 'cvvresp': null, 'avsresp': 'Y'}", ResponseStatus.Approved)]
        public async Task auth_success_attempt_without_cvv_auth_test(string body, ResponseStatus result)
        {
            _http.RespondWith(body);
            var call = await _service.AuthWithoutCapture(new CardConnectAuthorizationRequest()
            {
                cvv2 = null
            });
            Assert.That(call.respstat.ToResponseStatus() == result);
        }
    }

    public static class ResponseCodeFactory
    {
        public static IEnumerable FailCases
        {
            get
            {
                yield return new TestCaseData(@"{'respstat': 'B', 'respcode': 'NU', 'cvvresp': 'M', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'C', 'respcode': '05', 'cvvresp': 'M', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '101', 'cvvresp': 'N', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'C', 'respcode': '00', 'cvvresp': 'P', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'C', 'respcode': '101', 'cvvresp': 'U', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'C', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'N'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '101', 'cvvresp': 'M', 'avsresp': 'A'}");
                yield return new TestCaseData(@"{'respstat': 'B', 'respcode': '00', 'cvvresp': 'M', 'avsresp': 'Z'}");
                yield return new TestCaseData(@"{'respstat': 'B', 'respcode': '100', 'cvvresp': 'M', 'avsresp': 'Z'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '101', 'cvvresp': 'P', 'avsresp': 'Y'}");
                yield return new TestCaseData(@"{'respstat': 'A', 'respcode': '500', 'cvvresp': 'P', 'avsresp': 'Y'}");
            }
        }
    }
}