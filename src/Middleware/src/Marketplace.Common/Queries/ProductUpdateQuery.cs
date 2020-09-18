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
        Task<List<ProductHistory>> List(string supplierID, DateTime reporttime);
        Task<ProductHistory> Post(ProductHistory productUpdate);
    }

    public class ProductUpdateQuery : IProductUpdateQuery
    {
        private readonly ICosmosStore<ProductHistory> _productStore;
        public ProductUpdateQuery(ICosmosStore<ProductHistory> productStore)
        {
            _productStore = productStore;
        }

        public async Task<List<ProductHistory>> List(string productID, DateTime reporttime)
        {
            // list product history given product ID. TODO: get the most RECENT record.
            return await _productStore.Query().Where(x => x.Product.ID == productID).ToListAsync();
        }

        public async Task<ProductHistory> Post(ProductHistory update)
        {
            var newProductUpdate = await _productStore.AddAsync(update);
            return newProductUpdate;
        }
    }
}
