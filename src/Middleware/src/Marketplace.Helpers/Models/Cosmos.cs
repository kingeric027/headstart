using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Cosmonaut.Attributes;
using Marketplace.Helpers.Attributes;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using OrderCloud.SDK;

namespace Marketplace.Helpers.Models
{
    public interface ICosmosObject
    {
        string id { get; set; }
        DateTimeOffset timeStamp { get; set; }
    }

	public abstract class CosmosObject : ICosmosObject
	{
		[ApiIgnore]
		[JsonProperty("id")]
		public string id { get; set; } = Guid.NewGuid().ToString();
		[ApiIgnore]
		public DateTimeOffset timeStamp { get; set; } = DateTimeOffset.Now; 
		// Note, Cosmos Unqiue keys are only unique within the partition.
		public static Collection<UniqueKey> GetUniqueKeys() => null;

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

	[AttributeUsage(AttributeTargets.Property)]
	public class CosmosIgnoreAttribute : Attribute { }
}
