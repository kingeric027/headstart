using System;
using System.Collections.Generic;
using System.Text;
using Integrations.SmartyStreets;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.openapispec;

namespace Marketplace.Common
{
	public enum AppEnvironment { Qa, Demo, Prod }

    [DocIgnore]
    public class AppSettings
    {
		public AppEnvironment Env { get; set; }
		public EnvironmentSettings EnvironmentSettings { get; set; } = new EnvironmentSettings();
		public AvalaraSettings AvalaraSettings { get; set; }
        public BlobSettings BlobSettings { get; set; }
        public CosmosSettings CosmosSettings { get; set; } = new CosmosSettings();
        public OrderCloudSettings OrderCloudSettings { get; set; } = new OrderCloudSettings();
        public FreightPopSettings FreightPopSettings { get; set; }

        // additional field for production settings because we can only test third
        // party shipping on FreigthPOP prd
        public FreightPopSettings FreightPopSettingsProd { get; set; }
        public OrderCloudIntegrationsCardConnectConfig CardConnectSettings { get; set; } = new OrderCloudIntegrationsCardConnectConfig();
        public ZohoSettings ZohoSettings { get; set; } = new ZohoSettings();
		public SmartyStreetsConfig SmartyStreetSettings { get; set; } = new SmartyStreetsConfig();
        public string SendgridApiKey { get; set; }
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
        public string AuthToken { get; set; }
        public string OrgID { get; set; }
    }

    public class FreightPopSettings
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

	public class OrderCloudSettings
	{
		public string AuthUrl { get; set; }
		public string ApiUrl { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string WebhookHashKey { get; set; }
    }

	public class AvalaraSettings
	{
		public int AccountID { get; set; }
		public string LicenseKey { get; set; }
		public string CompanyCode { get; set; }
	}
    public class BlobSettings
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
        public string CacheName { get; set; }
		public string HostUrl { get; set; }
    }

	public class CosmosSettings
	{
		public string PrimaryKey { get; set; }
		public string EndpointUri { get; set; }
		public string DatabaseName { get; set; }
	}
}
