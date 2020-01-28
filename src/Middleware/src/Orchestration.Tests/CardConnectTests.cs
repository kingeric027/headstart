using System;
using System.Collections;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Marketplace.Common;
using Marketplace.Common.Mappers.CardConnect;
using Marketplace.Common.Models.CardConnect;
using Marketplace.Common.Services.CardConnect;
using Marketplace.Common.Services.CardConnect.Models;
using Marketplace.Helpers.Exceptions;
using NUnit.Framework;

namespace Orchestration.Tests
{
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
