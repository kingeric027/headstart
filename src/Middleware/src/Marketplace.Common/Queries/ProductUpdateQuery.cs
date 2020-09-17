using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Queries
{
    public interface IProductUpdateQuery
    {
        Task<List<ProductUpdate>> List(string supplierID, DateTime reporttime);
        Task<ProductUpdate> Post(ProductUpdate productUpdate);
    }

    public class ProductUpdateQuery : IProductUpdateQuery
    {
        private readonly ICosmosStore<ProductUpdate> _store;
        public ProductUpdateQuery(ICosmosStore<ProductUpdate> store)
        {
            _store = store;
        }

        public async Task<List<ProductUpdate>> List(string supplierID, DateTime reporttime)
        {
            var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(supplierID) };
            return await _store.Query(feedOptions).Where(x => x.timeStamp <= reporttime.AddHours(-24)).ToListAsync();
        }

        public async Task<ProductUpdate> Post(ProductUpdate productUpdate)
        {
            var newProductUpdate = await _store.AddAsync(productUpdate);
            return newProductUpdate;
        }
    }
}
