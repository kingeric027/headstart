using Cosmonaut;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Threading.Tasks;
using Marketplace.Models.Extended;

namespace Marketplace.Common.Queries
{
    public interface ISupplierCategoryConfigQuery
    {
        Task<SupplierCategoryConfig> Get(string id);
    }
    public class SupplierCategoryConfigQuery : ISupplierCategoryConfigQuery
    {
        private readonly ICosmosStore<SupplierCategoryConfig> _store;

        public SupplierCategoryConfigQuery(ICosmosStore<SupplierCategoryConfig> store)
        {
            _store = store;
        }

        public async Task<SupplierCategoryConfig> Get(string id)
        {
            var options = new RequestOptions { PartitionKey = new PartitionKey(id) };
            var item = await _store.FindAsync(id, options);
            return item;
        }
    }
}
