using System;
using System.IO;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Marketplace.Common.Services.Zoho.Resources;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho
{
    public partial interface IZohoClient
    {
        IZohoContactResource Contacts { get; }
        IZohoCurrencyResource Currencies { get; }
    }
    public partial class ZohoClient
    {
        private static readonly IFlurlClientFactory _clientFac = new PerBaseUrlFlurlClientFactory();
        private IFlurlClient ApiClient => _clientFac.Get(Config.ApiUrl);

        public ZohoClientConfig Config { get; }

        public ZohoClient() : this(new ZohoClientConfig()) { }

        public ZohoClient(ZohoClientConfig config)
        {
            Config = config;
            InitResources();
        }

        internal IFlurlRequest Request(params object[] segments) => ApiClient
            .Request(segments)
            .WithHeader("Content-Type", "multipart/form-data; boundary=--------------------------884679394228147704963496")
            .SetQueryParam("authtoken", Config.AuthToken)
            .SetQueryParam("organization_id", Config.OrganizationID)
            .ConfigureRequest(settings =>
            {
                settings.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
            });

    }

    public partial class ZohoClient : IZohoClient
    {
        private void InitResources()
        {
            Contacts = new ZohoContactResource(this);
            Currencies = new ZohoCurrencyResource(this);
        }

        public IZohoContactResource Contacts { get; private set; }
        public IZohoCurrencyResource Currencies { get; private set; }
    }
}
