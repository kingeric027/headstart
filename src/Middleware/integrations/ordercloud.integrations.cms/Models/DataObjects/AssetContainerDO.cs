using System.Collections.ObjectModel;
using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	[CosmosCollection("assetcontainers")]
	public class AssetContainerDO : CosmosObject 
	{
		[CosmosPartitionKey]
		public string SinglePartitionID => AssetContainerQuery.SinglePartitionID; // TODO - is there a better way to indicate there should only be one partition?
		public string SellerID { get; set; }
		public string BuyerID { get; set; }
		public string SupplierID { get; set; }
		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/SellerID", "/BuyerID", "/SupplierID" }}
			};
		}
	}
}
