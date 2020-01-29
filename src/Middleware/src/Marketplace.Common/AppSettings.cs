using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common
{
	public enum AppEnvironment { Local, Qa, Prod }

    //public interface IAppSettings
    //{
    //    AppEnvironment Env { get; set; }
    //    BlobSettings BlobSettings { get; set; }
    //    CosmosSettings CosmosSettings { get; set; }
    //    OrderCloudSettings OrderCloudSettings { get; set; }
    //}
    public class AppSettings // : IAppSettings
    {
		public AppEnvironment Env { get; set; }
		public int AvalaraSettingsAccountID { get; set; }
		public string AvalaraSettingsLicenseKey { get; set; }
		public string AvalaraSettingsCompanyCode { get; set; }
		public string BlobSettingsConnectionString { get; set; }
		public string BlobSettingsQueueName { get; set; }
		public string BlobSettingsCacheName { get; set; }
		public string BlobSettingsHostUrl { get; set; }
		public string CosmosSettingsPrimaryKey { get; set; }
		public string CosmosSettingsEndpointUri { get; set; }
		public string CosmosSettingsDatabaseName { get; set; }
		public string OrderCloudSettingsAuthUrl { get; set; }
		public string OrderCloudSettingsApiUrl { get; set; }
		public string SendgridSettingsApiKey { get; set; }
		public string FreightPopSettingsBaseUrl { get; set; }
		public string FreightPopSettingsUsername { get; set; }
		public string FreightPopSettingsPassword { get; set; }
		public string CardConnectSettingsSite { get; set; }
		public string CardConnectSettingsBaseUrl { get; set; }
		public string CardConnectSettingsAuthorization { get; set; }
	}
}
