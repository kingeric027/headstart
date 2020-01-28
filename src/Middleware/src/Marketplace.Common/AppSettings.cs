using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common
{
    public enum AppEnvironment { Local, Qa, Prod}

    public interface IAppSettings
    {
        AppEnvironment Env { get; }
        BlobSettings BlobSettings { get; }
        CosmosSettings CosmosSettings { get;}
        OrderCloudSettings OrderCloudSettings { get; }
        string SendgridApiKey { get; }
    }

    public class AppSettings : IAppSettings
    {
        public AppEnvironment Env { get; set; }
        public BlobSettings BlobSettings { get; set; }
        public CosmosSettings CosmosSettings { get; set; } = new CosmosSettings();
        public OrderCloudSettings OrderCloudSettings { get; set; } = new OrderCloudSettings();
        public string SendgridApiKey { get; set; }
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
		public string HostUrl { get; set; }
    }

    public class CosmosSettings
    {
        public string PrimaryKey { get; set; }
        public string EndpointUri { get; set; }
        public string DatabaseName { get; set; }
    }
}
