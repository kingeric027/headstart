using Marketplace.Models;
using Marketplace.Models.Misc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using System.Collections.Generic;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceAdminReportCommand
    {
        Task<List<MarketplaceAddressBuyer>> BuyerLocationReport(MarketplaceReportFilter filters, VerifiedUserContext verifiedUser);
    }
    
    public class MarketplaceAdminReportCommand : IMarketplaceAdminReportCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;

        public MarketplaceAdminReportCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }

        public async Task<List<MarketplaceAddressBuyer>> BuyerLocationReport(MarketplaceReportFilter filters, VerifiedUserContext user)
        {
            //Loop through each of the filters.BuyerIDs.  For each buyer ID, run the ListAsync call.  Results of that call (items) push into allBuyerLocations variable.
            var allBuyerLocations = new List<MarketplaceAddressBuyer>();
            foreach (var buyerID in filters.Filters.BuyerID)
            {
                var buyerLocationsForBuyerID = await _oc.Addresses.ListAsync<MarketplaceAddressBuyer>(buyerID, accessToken: user.AccessToken, page: 1, pageSize: 100);
                allBuyerLocations.AddRange(buyerLocationsForBuyerID.Items);
                if (buyerLocationsForBuyerID.Meta.TotalPages > 1)
                {
                    for (int i = 2; i <= buyerLocationsForBuyerID.Meta.TotalPages; i++)
                    {
                        var excessBuyerLocationsForBuyerID = await _oc.Addresses.ListAsync<MarketplaceAddressBuyer>(buyerID, accessToken: user.AccessToken, page: i, pageSize: 100);
                        allBuyerLocations.AddRange(excessBuyerLocationsForBuyerID.Items);
                    }
                }
            }

            //When we have allBuyerLocations, push the ones that pass the remaining filter tests into filteredBuyerLocations variable.
            var filteredBuyerLocations = new List<MarketplaceAddressBuyer>();
            foreach (var location in allBuyerLocations)
            {
                if (BuyerStateFilter(location, filters)) { continue; }
                if (BuyerCountryFilter(location, filters)) { continue; }
                filteredBuyerLocations.Add(location);
            }
            return filteredBuyerLocations;
        }

        public bool BuyerStateFilter(MarketplaceAddressBuyer location, MarketplaceReportFilter filters)
        {
            if (filters.Filters.State != null)
            {
                if (location.State == null || !filters.Filters.State.Contains(location.State))
                {
                    return true;
                }
            }
            return false;
        }

        public bool BuyerCountryFilter(MarketplaceAddressBuyer location, MarketplaceReportFilter filters)
        {
            if (filters.Filters.Country != null)
            {
                if (location.Country == null || !filters.Filters.Country.Contains(location.Country))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
