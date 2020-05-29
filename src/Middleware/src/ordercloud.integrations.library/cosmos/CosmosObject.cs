using System;
using System.Collections.ObjectModel;
using Microsoft.Azure.Documents;

namespace ordercloud.integrations.library
{
	public interface ICosmosObject
	{
		string id { get; set; }
		DateTimeOffset timeStamp { get; set; }
	}

	public abstract class CosmosObject : ICosmosObject
	{
		//[ApiIgnore]
		//[JsonProperty("id")]
		public string id { get; set; } = Guid.NewGuid().ToString();
		//[ApiIgnore]
		public DateTimeOffset timeStamp { get; set; } = DateTimeOffset.Now;
		// Note, Cosmos unique keys are only unique within the partition.
		public static Collection<UniqueKey> GetUniqueKeys() => null;

	}
}
