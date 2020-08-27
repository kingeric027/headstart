using System;
using System.Collections.ObjectModel;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace ordercloud.integrations.library
{
	public interface ICosmosObject
	{
		string id { get; set; }
		DateTimeOffset timeStamp { get; set; }
	}

	public abstract class CosmosObject : ICosmosObject
	{
		[ApiIgnore]
		//[JsonProperty("id")] Oliver, let's discuss this!  Had to comment this out today as a workaround (though it was working before as is).
		public string id { get; set; } = Guid.NewGuid().ToString();
		[ApiIgnore]
		public DateTimeOffset timeStamp { get; set; } = DateTimeOffset.Now;
		// Note, Cosmos unique keys are only unique within the partition.
		public static Collection<UniqueKey> GetUniqueKeys() => null;
	}
}
