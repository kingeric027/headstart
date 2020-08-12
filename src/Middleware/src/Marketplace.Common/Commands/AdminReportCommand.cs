using Marketplace.Models;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using System.Collections.Generic;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using System;
using System.Linq;
using System.Reflection;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceAdminReportCommand
    {
        Task<List<MarketplaceAddressBuyer>> BuyerLocation(string templateID, VerifiedUserContext verifiedUser);
        Task<List<ReportTemplate>> ListReportTemplatesByReportType(string reportType, VerifiedUserContext verifiedUser);
        Task<ReportTemplate> PostReportTemplate(ReportTemplate reportTemplate, VerifiedUserContext verifiedUser);
        Task DeleteReportTemplate(string id, VerifiedUserContext verifiedUser);
    }
    
    public class MarketplaceAdminReportCommand : IMarketplaceAdminReportCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly ReportTemplateQuery _template;

        public MarketplaceAdminReportCommand(IOrderCloudClient oc, ReportTemplateQuery template)
        {
            _oc = oc;
            _template = template;
        }

        public async Task<List<MarketplaceAddressBuyer>> BuyerLocation(string templateID, VerifiedUserContext verifiedUser)
        {
            //Get stored template from Cosmos DB container
            var template = await _template.Get(templateID, verifiedUser);
            var allBuyerLocations = new List<MarketplaceAddressBuyer>();
            foreach (var buyerID in template.Filters.BuyerID)
            {
                //For every buyer included in the template filters, grab all buyer locations (exceeding 100 maximum)
                var buyerLocations = await OcListAllAsync((page) => _oc.Addresses.ListAsync<MarketplaceAddressBuyer>(
                    buyerID,
                    filters: null,
                    page: page,
                    pageSize: 1
                ));
                allBuyerLocations.AddRange(buyerLocations);
            }
            //Use reflection to determine available filters from model
            var filterClassProperties = template.Filters.GetType().GetProperties();
            //Create dictionary of key/value pairings of filters, where provided in the template
            var filtersToEvaluateMap = new Dictionary<PropertyInfo, List<string>>();
            foreach (var property in filterClassProperties)
            {
                if (property.GetValue(template.Filters) != null && property.Name != "BuyerID")
                {
                    filtersToEvaluateMap.Add(property, (List<string>) property.GetValue(template.Filters));
                }
            }
            //Filter through collected records, adding only those that pass the PassesFilters check.
            var filteredBuyerLocations = new List<MarketplaceAddressBuyer>();
            foreach (var location in allBuyerLocations)
            {

                if (PassesFilters(location, filtersToEvaluateMap))
                {
                    filteredBuyerLocations.Add(location);
                }
            }
            return filteredBuyerLocations;
        }

        public async Task<List<ReportTemplate>> ListReportTemplatesByReportType(string reportType, VerifiedUserContext verifiedUser)
        {
            var template = await _template.List(reportType, verifiedUser);
            return template;
        }

        public async Task<ReportTemplate> PostReportTemplate(ReportTemplate reportTemplate, VerifiedUserContext verifiedUser)
        {
            var template = await _template.Post(reportTemplate, verifiedUser);
            return template;
        }

        public async Task DeleteReportTemplate(string id, VerifiedUserContext verifiedUser)
        {
            await _template.Delete(id, verifiedUser);
        }

        private bool PassesFilters(object data, Dictionary<PropertyInfo, List<string>> filtersToEvaluate)
        {
            foreach (var filterProps in filtersToEvaluate)
            {
                var filterKey = filterProps.Key.Name;
                var filterValues = filterProps.Value;
                var dataValue = data.GetType().GetProperty(filterKey).GetValue(data);
                if (!filterValues.Contains(dataValue))
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<List<T>> OcListAllAsync<T>(Func<int, Task<ListPage<T>>> listFunc)
        {
            var pageTasks = new List<Task<ListPage<T>>>();
            var totalPages = 0;
            var i = 1;
            do
            {
                pageTasks.Add(listFunc(i++));
                var running = pageTasks.Where(t => !t.IsCompleted && !t.IsFaulted).ToList();
                if (totalPages == 0 || running.Count >= 16) // throttle parallel tasks at 16
                    totalPages = (await await Task.WhenAny(running)).Meta.TotalPages;  //Set total number of pages based on returned Meta.
            } while (i <= totalPages);
            var data = (
                from finalResult in await Task.WhenAll(pageTasks) //When all pageTasks are complete, save items in data variable.
                from item in finalResult.Items
                select item).ToList();
            return data;
        }
    }
}
