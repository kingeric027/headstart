using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cosmonaut.Attributes;
using Marketplace.Helpers.Attributes;
using Newtonsoft.Json;
using OrderCloud.SDK;

namespace Marketplace.Helpers.Models
{
    public interface ICosmosObject
    {
        string id { get; set; }
        DateTimeOffset timeStamp { get; set; }
    }

    public class CosmosObject : ICosmosObject
    {
        [ApiIgnore]
		[JsonProperty("id"), CosmosPartitionKey]
		public string id { get; set; } = Guid.NewGuid().ToString();
        [ApiIgnore]
        public DateTimeOffset timeStamp { get; set; } = DateTimeOffset.Now;
	}

    public interface ICosmosQuery<T> where T : ICosmosObject
    {
        Task<ListPage<T>> List(IListArgs args);
        Task<T> Get(string id);
        Task<T> Save(T entity);
        Task<List<T>> SaveMany(List<T> entities);
        Task Delete(string id);
    }

    public class CosmosConfig
    {
        public CosmosConfig()
        {
        }

        public CosmosConfig(string db, string uri, string key)
        {
            this.DatabaseName = db;
            this.EndpointUri = uri;
            this.PrimaryKey = key;
        }
        public string DatabaseName { get; set; }
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
    }
}
