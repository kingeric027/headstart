using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using ordercloud.integrations.library;
using System.Linq;

namespace ordercloud.integrations.cms
{
	public interface IAssetContainerQuery
	{

		Task<AssetContainer> CreateDefaultIfNotExists(VerifiedUserContext user);
		Task<Customer> GetCustomer(string sellerID);
	}

	public class AssetContainerQuery: IAssetContainerQuery
	{
		private readonly ICosmosStore<AssetContainerDO> _store;
		public const string SinglePartitionID = "SinglePartitionID"; // TODO - is there a better way?

		public AssetContainerQuery(ICosmosStore<AssetContainerDO> store)
		{
			_store = store;
		}

		public async Task<AssetContainer> CreateDefaultIfNotExists(VerifiedUserContext user)
		{
			AssetContainerDO container;
			var customer = await GetCustomer(user.SellerID);

			if (customer.AssetPartitionStrategy == AssetPartitionStrategy.PartitionByCompanyID)
			{
				var existingContainer = await _store.Query().FirstOrDefaultAsync(c =>
				c.SellerID == user.SellerID &&
				c.BuyerID == user.BuyerID &&
				c.SupplierID == user.SupplierID);
				container = existingContainer ?? await _store.AddAsync(new AssetContainerDO()
				{
					SellerID = user.SellerID,
					BuyerID = user.BuyerID,
					SupplierID = user.SupplierID
				});
			} else
			{
				var existingContainer = await _store.Query().FirstOrDefaultAsync(c => c.SellerID == user.SellerID);
				container = existingContainer ?? await _store.AddAsync(new AssetContainerDO() { SellerID = user.SellerID });
			}
			return AssetContainerMapper.MapTo(container, customer);
		}

		public async Task<Customer> GetCustomer(string sellerID)
		{
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
