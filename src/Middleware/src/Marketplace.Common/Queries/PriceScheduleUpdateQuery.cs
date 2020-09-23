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

    public class PriceScheduleHistoryQuery
    {
        private readonly ICosmosStore<PriceScheduleHistory> _priceScheduleStore;
        public PriceScheduleHistoryQuery(ICosmosStore<PriceScheduleHistory> priceScheduleStore)
        {
            _priceScheduleStore = priceScheduleStore;
        }

        public async Task<List<PriceScheduleHistory>> ListPriceSchedules(string priceScheduleID)
        {
            // list product history given product ID. TODO: get the most RECENT record.
            var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(priceScheduleID) };
            return await _priceScheduleStore.Query(feedOptions).ToListAsync();
        }

        public async Task<PriceScheduleHistory> PostPriceSchedule(PriceScheduleHistory update)
        {
            var time = DateTime.Now;
            update.DateLastUpdated = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
            var newProductUpdate = await _priceScheduleStore.AddAsync(update);
            return newProductUpdate;
        }

        public async Task<PriceScheduleHistory> PutPriceSchedule(PriceScheduleHistory update)
        {
            var time = DateTime.Now;
            update.DateLastUpdated = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
            var productToUpdate = await _priceScheduleStore.Query().FirstOrDefaultAsync(record => record.PriceScheduleID == update.PriceSchedule.ID && record.DateLastUpdated == update.DateLastUpdated);
            try
            {
                update.id = productToUpdate.id;
                return await _priceScheduleStore.UpdateAsync(update);
            }
            catch
            {
                return await _priceScheduleStore.AddAsync(update);
            }
        }

        public async Task<List<PriceScheduleHistory>> ListPriceSchedulesByDate(DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var nextDay = date.AddDays(1);
            var end = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
            var products = await _priceScheduleStore.Query()
                .Where(product => product.DateLastUpdated >= start && product.DateLastUpdated < end).ToListAsync();
            return products;
        }

        public async Task DeletePriceSchedules(string cosmosID)
        {
            await _priceScheduleStore.RemoveAsync(product => product.id == cosmosID);
        }
    }
}

