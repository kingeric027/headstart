using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Queries
{
    public interface IProductHistoryQuery
    {
        Task<List<ProductHistory>> ListProducts(string supplierID);
        Task<ProductHistory> PostProduct(ProductHistory productUpdate);
        Task<ProductHistory> PutProduct(ProductHistory productUpdate);

    }

    public class ProductHistoryQuery : IProductHistoryQuery
    {
        private readonly ICosmosStore<ProductHistory> _productStore;
        public ProductHistoryQuery(ICosmosStore<ProductHistory> productStore)
        {
            _productStore = productStore;
        }

        public async Task<List<ProductHistory>> ListProducts(string productID)
        {
            // list product history given product ID. TODO: get the most RECENT record.
            var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(productID) };
            return await _productStore.Query(feedOptions).ToListAsync();
        }

        public async Task<ProductHistory> PostProduct(ProductHistory update)
        {
            var time = DateTime.Now;
            update.DateLastUpdated = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
            var newProductUpdate = await _productStore.AddAsync(update);
            return newProductUpdate;
        }

        public async Task<ProductHistory> PutProduct(ProductHistory update)
        {
            var time = DateTime.Now;
            update.DateLastUpdated = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
            var productToUpdate = await _productStore.Query().FirstOrDefaultAsync(record => record.ProductID == update.Product.ID && record.DateLastUpdated == update.DateLastUpdated);
            try
            {
                update.id = productToUpdate.id;
                return await _productStore.UpdateAsync(update);
            }
            catch
            {
                return await _productStore.AddAsync(update);
            }
        }

        public async Task<List<ProductHistory>> ListProductsByDate(DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var nextDay = date.AddDays(1);
            var end = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
            var products = await _productStore.Query()
                .Where(product => product.DateLastUpdated >= start && product.DateLastUpdated < end).ToListAsync();
            return products;
        }

        public async Task DeleteProduct(string cosmosID)
        {
            await _productStore.RemoveAsync(product => product.id == cosmosID);
        }
    }
}
