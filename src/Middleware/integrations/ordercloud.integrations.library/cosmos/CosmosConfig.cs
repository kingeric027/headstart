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

        public CosmosConfig(
            string db, 
            string uri, 
            string key, 
            int requestTimeoutInSeconds,
            int maxConnectionLimit,
            int idleTcpConnectionTimeoutInMinutes,
            int openTcpConnectionTimeoutInSeconds,
            int maxTcpConnectionsPerEndpoint,
            int maxRequestsPerTcpConnection,
            bool enableTcpConnectionEndpointRediscovery
        )
        {
            this.DatabaseName = db;
            this.EndpointUri = uri;
            this.PrimaryKey = key;
            this.RequestTimeout = TimeSpan.FromSeconds(requestTimeoutInSeconds);
            this.MaxConnectionLimit = maxConnectionLimit;
            this.IdleTcpConnectionTimeout = TimeSpan.FromMinutes(idleTcpConnectionTimeoutInMinutes);
            this.OpenTcpConnectionTimeoutInSeconds = TimeSpan.FromSeconds(openTcpConnectionTimeoutInSeconds);
            this.MaxTcpConnectionsPerEndpoint = maxTcpConnectionsPerEndpoint;
            this.MaxRequestsPerTcpConnection = maxRequestsPerTcpConnection;
            this.EnableTcpConnectionEndpointRediscovery = enableTcpConnectionEndpointRediscovery;

        }
        public string DatabaseName { get; set; }
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public TimeSpan RequestTimeout { get; set; }
        public int MaxConnectionLimit { get; set; }
        public TimeSpan IdleTcpConnectionTimeout { get; set; }
        public TimeSpan OpenTcpConnectionTimeoutInSeconds { get; set; }
        public int MaxTcpConnectionsPerEndpoint { get; set; }
        public int MaxRequestsPerTcpConnection { get; set; }
        public bool EnableTcpConnectionEndpointRediscovery { get; set; }
    }
}
