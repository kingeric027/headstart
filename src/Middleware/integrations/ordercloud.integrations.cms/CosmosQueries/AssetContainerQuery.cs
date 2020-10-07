using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	public interface IAssetContainerQuery
	{
		Task<AssetContainerDO> CreateDefaultIfNotExists(VerifiedUserContext user);
	}

	public class AssetContainerQuery: IAssetContainerQuery
	{
		private readonly ICosmosStore<AssetContainerDO> _store;
		private readonly IBlobStorage _blob;
		public const string SinglePartitionID = "SinglePartitionID"; // TODO - is there a better way?

		public AssetContainerQuery(ICosmosStore<AssetContainerDO> store, IBlobStorage blob)
		{
			_store = store;
			_blob = blob;
		}

		public async Task<AssetContainerDO> CreateDefaultIfNotExists(VerifiedUserContext user)
		{
			var existingContainer = await _store.Query().FirstOrDefaultAsync(c =>
				c.SellerID == user.SellerID &&
				c.BuyerID == user.BuyerID &&
				c.SupplierID == user.SupplierID);
			return existingContainer ?? await _store.AddAsync(new AssetContainerDO()
			{
				SellerID = user.SellerID,
				BuyerID = user.BuyerID,
				SupplierID = user.SupplierID
			});
		}
	}
}
