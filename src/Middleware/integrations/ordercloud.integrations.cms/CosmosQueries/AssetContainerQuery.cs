using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using ordercloud.integrations.library;
using ordercloud.integrations.cms.Models;
using System.Linq;
using Microsoft.AspNetCore.Server.IIS.Core;

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
			var customer = await GetCustomer(user);
			if (customer.AssetPartitionStrategy == AssetPartitionStrategy.PartitionByCompanyID)
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
			} else
			{
				var existingContainer = await _store.Query().FirstOrDefaultAsync(c => c.SellerID == user.SellerID);
				return existingContainer ?? await _store.AddAsync(new AssetContainerDO() { SellerID = user.SellerID });
			}			
		}

		private async Task<Customer> GetCustomer(VerifiedUserContext user)
		{
			var sellerID = user.SellerID;
			var customers = await _store.QuerySingleAsync<CustomerSettingsObject>("SELECT * FROM c WHERE c.id = 'Customer_Settings_Object'");
			if (customers == null) throw new MissingConfigationException();
			var customer = customers.Customers?.FirstOrDefault(c => 
				sellerID == c.SellerID || 
				sellerID == $"{c.SellerID}_Staging" ||
				sellerID == $"{c.SellerID}_Sandbox");
			if (customer == null) throw new NotConfiguredForAssetsException(sellerID);
			return customer;
		}
	}
}
