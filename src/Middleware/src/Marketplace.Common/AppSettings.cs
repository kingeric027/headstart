using System;
using System.Collections.Generic;
using System.Text;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using ordercloud.integrations.smartystreets;
using ordercloud.integrations.tecra;

namespace Marketplace.Common
{
	public enum AppEnvironment { Qa, Demo, Prod }

    [DocIgnore]
    public class AppSettings
    {
		public AppEnvironment Env { get; set; }
        public UI UI { get; set; }
		public EnvironmentSettings EnvironmentSettings { get; set; } = new EnvironmentSettings();
		public AvalaraSettings AvalaraSettings { get; set; }
        public BlobSettings BlobSettings { get; set; }
        public CosmosSettings CosmosSettings { get; set; } = new CosmosSettings();
        public OrderCloudSettings OrderCloudSettings { get; set; } = new OrderCloudSettings();
        public OrderCloudIntegrationsCardConnectConfig CardConnectSettings { get; set; } = new OrderCloudIntegrationsCardConnectConfig();
        public OrderCloudTecraConfig TecraSettings { get; set; } = new OrderCloudTecraConfig();
        public ZohoSettings ZohoSettings { get; set; } = new ZohoSettings();
		public SmartyStreetsConfig SmartyStreetSettings { get; set; } = new SmartyStreetsConfig();
        public ExchangeRatesSettings ExchangeRatesSettings { get; set; }
        public string SendgridApiKey { get; set; }
        public ChiliPublishSettings ChiliPublishSettings { get; set; } = new ChiliPublishSettings();
        public EasyPostSettings EasyPostSettings { get; set; } = new EasyPostSettings();
    }

    public class UI
    {
        public string BaseBuyerUrl { get; set; }
        public string BaseAdminUrl { get; set; }
    }

	public class EnvironmentSettings
	{
		public string BaseUrl { get; set; }
	}

	public class SmartyStreetSettings
	{
		public string AuthID { get; set; }
		public string AuthToken { get; set; }
		public string RefererHost { get; set; } // The autocomplete pro endpoint requires the Referer header to be a pre-set value 
		public string WebsiteKey { get; set; }
	}

    public class ZohoSettings
    {
        public string AccessToken { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string OrgID { get; set; }
    }

	public class OrderCloudSettings
	{
		public string ApiUrl { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string WebhookHashKey { get; set; }
        public string SEBDistributionSupplierID { get; set; }
        public string ProvisionSupplierID { get; set; }
    }

	public class AvalaraSettings
	{
		public int AccountID { get; set; }
		public string LicenseKey { get; set; }
		public string CompanyCode { get; set; }
	}

    public class ChiliPublishSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class EasyPostSettings
	{
        public string APIKey { get; set; }
        public string SMGFedexAccountId { get; set; }
        public string ProvisionFedexAccountId { get; set; } 
        public string SEBDistributionFedexAccountId { get; set; }

    }
}
