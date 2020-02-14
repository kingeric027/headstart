using Flurl.Http;

namespace Marketplace.Common.Services.Zoho
{
    public abstract class ZohoResource
    {
        private readonly ZohoClient _client;
        protected ZohoResource(ZohoClient client) => _client = client;

        protected internal IFlurlRequest Request(params object[] segments) => _client.Request(segments);
    }
}
