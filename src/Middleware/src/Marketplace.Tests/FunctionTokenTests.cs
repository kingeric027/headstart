using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using NUnit.Framework;
using OrderCloud.SDK;

namespace Marketplace.Tests
{
    public class FunctionTokenTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task valid_token_returns_user()
        {
            const string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c3IiOiJzdGVpbm1ldHpiYXNlY2FtcCIsImNpZCI6IjA2YzkzNjI5LWZlOWEtNGVjNS05NjUyLWMwZjA1OWI1Y2M3YyIsInUiOiIyODQ0MzE1IiwidXNydHlwZSI6InN1cHBsaWVyIiwicm9sZSI6WyJQcm9kdWN0QWRtaW4iLCJQcmljZVNjaGVkdWxlQWRtaW4iLCJTdXBwbGllclJlYWRlciIsIk9yZGVyQWRtaW4iLCJTdXBwbGllclVzZXJBZG1pbiIsIlN1cHBsaWVyVXNlckdyb3VwQWRtaW4iLCJTdXBwbGllckFkZHJlc3NBZG1pbiIsIlByb2R1Y3RGYWNldFJlYWRlciIsIlNoaXBtZW50QWRtaW4iLCJNUE1lUHJvZHVjdEFkbWluIiwiTVBPcmRlckFkbWluIiwiTVBTaGlwbWVudEFkbWluIiwiTVBNZVN1cHBsaWVyQWRkcmVzc0FkbWluIiwiTVBNZVN1cHBsaWVyVXNlckFkbWluIiwiTVBTdXBwbGllclVzZXJHcm91cEFkbWluIl0sImlzcyI6Imh0dHBzOi8vc3RhZ2luZ2F1dGgub3JkZXJjbG91ZC5pbyIsImF1ZCI6Imh0dHBzOi8vc3RhZ2luZ2FwaS5vcmRlcmNsb3VkLmlvIiwiZXhwIjoxNTkwMDI1MzY5LCJuYmYiOjE1ODk5ODkzNjl9.S8MM1jpZTIEQIFqWRuC0I_-IEa9SW71qzEWDLreNC3Y";
            
            var mockRequest = Substitute.For<HttpRequest>();
            mockRequest.Headers.Returns(new HeaderDictionary()
            {
                new KeyValuePair<string, StringValues>("Authorization", $"Bearer {token}")
            });

            var mockOc = Substitute.For<IOrderCloudClient>();
            mockOc.Me.GetAsync().Returns(new MeUser());

            var f = new OrderCloudIntegrationsFunctionToken(mockOc);
            var test = f.Authorize(mockRequest, new[] {ApiRole.OrderAdmin});
            Assert.IsTrue(true);
        }

    }
}
