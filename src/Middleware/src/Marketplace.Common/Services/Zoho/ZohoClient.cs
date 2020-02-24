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
        IZohoItemResource Items { get; }
        IZohoSalesOrderResource SalesOrders { get; }
        IZohoPurchaseOrderResource PurchaseOrders { get;  }
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

        internal IFlurlRequest Request(object[] segments) => ApiClient
            .Request(segments)
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

        internal IFlurlRequest Put(object obj, object[] segments) => WriteRequest(obj, segments);
        internal IFlurlRequest Post(object obj, object[] segments) => WriteRequest(obj, segments);

        private IFlurlRequest WriteRequest(object obj, object[] segments) =>  ApiClient
            .Request(segments)
            .SetQueryParam("authtoken", Config.AuthToken)
            .SetQueryParam("organization_id", Config.OrganizationID)
            .SetQueryParam("JSONString", JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            }))
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
            Items = new ZohoItemResource(this);
            SalesOrders = new ZohoSalesOrderResource(this);
            PurchaseOrders = new ZohoPurchaseOrderResource(this);
        }

        public IZohoContactResource Contacts { get; private set; }
        public IZohoCurrencyResource Currencies { get; private set; }
        public IZohoItemResource Items { get; private set; }
        public IZohoSalesOrderResource SalesOrders { get; private set; }
        public IZohoPurchaseOrderResource PurchaseOrders { get; private set; }
    }
}
