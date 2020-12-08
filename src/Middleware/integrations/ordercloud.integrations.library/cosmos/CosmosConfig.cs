using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.library
{
    public class CosmosConfig
    {
        public CosmosConfig()
        {
        }

        public CosmosConfig(string db, string uri, string key, int timeoutInSeconds)
        {
            this.DatabaseName = db;
            this.EndpointUri = uri;
            this.PrimaryKey = key;
            this.RequestTimeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }
        public string DatabaseName { get; set; }
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public TimeSpan RequestTimeout { get; set; }
    }
}
