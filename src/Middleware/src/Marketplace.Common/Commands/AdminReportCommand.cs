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
        private readonly AppSettings _settings;
        private readonly ReportTemplateQuery _template;

        public MarketplaceAdminReportCommand(AppSettings settings, IOrderCloudClient oc, ReportTemplateQuery template)
        {
            _settings = settings;
            _oc = oc;
            _template = template;
        }

        public async Task<List<MarketplaceAddressBuyer>> BuyerLocation(string templateID, VerifiedUserContext verifiedUser)
        {
            var template = await _template.Get(templateID, verifiedUser);
            var allBuyerLocations = new List<MarketplaceAddressBuyer>();
            foreach (var buyerID in template.Filters.BuyerID)
            {
                var buyerLocations = await _oc.Addresses.ListAsync<MarketplaceAddressBuyer>(buyerID, accessToken: verifiedUser.AccessToken, page: 1, pageSize: 100);
                allBuyerLocations.AddRange(buyerLocations.Items);
                if (buyerLocations.Meta.TotalPages > 1)
                {
                    for (int i = 2; i <= buyerLocations.Meta.TotalPages; i++)
                    {
                        var excessBuyerLocations = await _oc.Addresses.ListAsync<MarketplaceAddressBuyer>(buyerID, accessToken: verifiedUser.AccessToken, page: i, pageSize: 100);
                        allBuyerLocations.AddRange(excessBuyerLocations.Items);
                    }
                }
            }
            var filterClassProperties = template.Filters.GetType().GetProperties();
            var filtersToEvaluateMap = new Dictionary<PropertyInfo, List<string>>();
            foreach (var property in filterClassProperties)
            {
                if (property.GetValue(template.Filters) != null && property.Name != "BuyerID")
                {
                    filtersToEvaluateMap.Add(property, (List<string>) property.GetValue(template.Filters));
                }
            }
            var filteredBuyerLocations = new List<MarketplaceAddressBuyer>();
            foreach (var location in allBuyerLocations)
            {
                var passesFilters = true;
                foreach (var filterProps in filtersToEvaluateMap)
                {
                    var filterKey = filterProps.Key.Name;
                    var filterValues = filterProps.Value;
                    var locValue = location.GetType().GetProperty(filterKey).GetValue(location);
                    if (!filterValues.Contains(locValue))
                    {
                        passesFilters = false;
                        break;
                    }
                }
                if (passesFilters)
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
    }
}
