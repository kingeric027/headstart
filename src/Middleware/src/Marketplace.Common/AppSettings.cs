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
		public AvalaraSettings AvalaraSettings { get; set; }
		public BlobSettings BlobSettings { get; set; }
		public CosmosSettings CosmosSettings { get; set; } = new CosmosSettings();
		public OrderCloudSettings OrderCloudSettings { get; set; } = new OrderCloudSettings();
	}


	public class AvalaraSettings
	{
		public int AccountID { get; set; }
		public string LicenseKey { get; set; }
	}

	public class OrderCloudSettings
	{
		public string AuthUrl { get; set; }
		public string ApiUrl { get; set; }
	}

	public class BlobSettings
	{
		public string ConnectionString { get; set; }
		public string QueueName { get; set; }
		public string CacheName { get; set; }
	}

	public class CosmosSettings
	{
		public string PrimaryKey { get; set; }
		public string EndpointUri { get; set; }
		public string DatabaseName { get; set; }
	}
}
