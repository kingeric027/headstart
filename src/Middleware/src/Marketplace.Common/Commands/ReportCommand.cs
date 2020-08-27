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
using ordercloud.integrations.library.helpers;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceReportCommand
    {
        ListPage<ReportTypeResource> FetchAllReportTypes(VerifiedUserContext verifiedUser);
        Task<List<MarketplaceAddressBuyer>> BuyerLocation(string templateID, VerifiedUserContext verifiedUser);
        Task<List<ReportTemplate>> ListReportTemplatesByReportType(ReportTypeEnum reportType, VerifiedUserContext verifiedUser);
        Task<ReportTemplate> PostReportTemplate(ReportTemplate reportTemplate, VerifiedUserContext verifiedUser);
        Task<ReportTemplate> GetReportTemplate(string id, VerifiedUserContext verifiedUser);
        Task<ReportTemplate> UpdateReportTemplate(string id, ReportTemplate reportTemplate, VerifiedUserContext verifiedUser);
        Task DeleteReportTemplate(string id, VerifiedUserContext verifiedUser);
    }
    
    public class MarketplaceReportCommand : IMarketplaceReportCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly ReportTemplateQuery _template;

        public MarketplaceReportCommand(IOrderCloudClient oc, ReportTemplateQuery template)
        {
            _oc = oc;
            _template = template;
        }

        public ListPage<ReportTypeResource> FetchAllReportTypes(VerifiedUserContext verifiedUser)
        {
            var items = ReportTypeResource.ReportTypes.ToList();
            var listPage = new ListPage<ReportTypeResource>
            {
                Items = items,
                Meta = new ListPageMeta
                {
                    Page = 1,
                    PageSize = 100,
                    TotalCount = items.Count,
                    TotalPages = 1
                }
            };
            return listPage;
        }

        public async Task<List<MarketplaceAddressBuyer>> BuyerLocation(string templateID, VerifiedUserContext verifiedUser)
        {
            //Get stored template from Cosmos DB container
            var template = await _template.Get(templateID, verifiedUser);
            var allBuyerLocations = new List<MarketplaceAddressBuyer>();

            //Logic if no Buyer ID is supplied
            if (template.Filters.BuyerID.Count == 0)
            {
                var buyers = await ListAllAsync.List((page) => _oc.Buyers.ListAsync<MarketplaceBuyer>(
                    filters: null,
                    page: page,
                    pageSize: 100
                 ));
                foreach (var buyer in buyers)
                {
                    template.Filters.BuyerID.Add(buyer.ID);
                }
            }

            foreach (var buyerID in template.Filters.BuyerID)
            {
                //For every buyer included in the template filters, grab all buyer locations (exceeding 100 maximum)
                var buyerLocations = await ListAllAsync.List((page) => _oc.Addresses.ListAsync<MarketplaceAddressBuyer>(
                    buyerID,
                    filters: null,
                    page: page,
                    pageSize: 100
                ));
                allBuyerLocations.AddRange(buyerLocations);
            }
            //Use reflection to determine available filters from model
            var filterClassProperties = template.Filters.GetType().GetProperties();
            //Create dictionary of key/value pairings of filters, where provided in the template
            var filtersToEvaluateMap = new Dictionary<PropertyInfo, List<string>>();
            foreach (var property in filterClassProperties)
            {
                //See if there are filters provided on the property.  If no values supplied, do not evaluate the filter.
                List<string> propertyFilters = (List<string>)property.GetValue(template.Filters);
                if (propertyFilters != null && propertyFilters.Count > 0 && property.Name != "BuyerID")
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

        public async Task<List<ReportTemplate>> ListReportTemplatesByReportType(ReportTypeEnum reportType, VerifiedUserContext verifiedUser)
        {
            var template = await _template.List(reportType, verifiedUser);
            return template;
        }

        public async Task<ReportTemplate> PostReportTemplate(ReportTemplate reportTemplate, VerifiedUserContext verifiedUser)
        {
            var template = await _template.Post(reportTemplate, verifiedUser);
            return template;
        }
        public async Task<ReportTemplate> GetReportTemplate(string id, VerifiedUserContext verifiedUser)
        {
            return await _template.Get(id, verifiedUser);
        }

        public async Task<ReportTemplate> UpdateReportTemplate(string id, ReportTemplate reportTemplate, VerifiedUserContext verifiedUser)
        {
            var template = await _template.Put(id, reportTemplate, verifiedUser);
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
    }
}
