using Marketplace.Common.TemporaryAppConstants;
using Marketplace.Models;
using Marketplace.Models.Misc;
using OrderCloud.SDK;
using System.Linq;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using System.Collections.Generic;
using NPOI.HPSF;
using System;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;

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
            //TO-DO - HOW TO HANDLE REQUESTS WITH MORE THAN 100 ITEMS NEEDING TO BE RETURNED?

            //Need to get locations for all Buyer IDs provided.  In this example, filters contains 0002, 0003, and 0005.
            //Loop through each of the filters.BuyerIDs.  For each buyer ID, run the ListAsync call.  Results of that call (items) must push into allBuyerLocations variable.
            var allBuyerLocations = new List<MarketplaceAddressBuyer>();
            foreach (var buyerID in filters.Filters.BuyerID)
            {
                var buyerLocationsForBuyerID = await _oc.Addresses.ListAsync<MarketplaceAddressBuyer>(buyerID, accessToken: user.AccessToken, pageSize: 100);
                allBuyerLocations.AddRange(buyerLocationsForBuyerID.Items);
            }

            //When we have allBuyerLocations, we must push the ones that pass the remaining filter tests into filteredBuyerLocations variable.
            var filteredBuyerLocations = new List<MarketplaceAddressBuyer>();
            foreach (var location in allBuyerLocations)
            {
                // slow performance, use array for filters, tighter code
                //var locationProperty = typeof(MarketplaceAddressBuyer).GetProperty("City").GetValue(location);
                
                // faster performance, more code
                if (CityFilter(location, filters))  { continue; }
                if (StateFilter(location, filters)) { continue; }
                if (ZipFilter(location, filters)) { continue; }
                filteredBuyerLocations.Add(location);
            }

            //We must return filtered buyer locations
            //var buyerLocations = await _oc.Addresses.ListAsync<MarketplaceAddressBuyer>("0003", accessToken: user.AccessToken, pageSize: 100);

            return filteredBuyerLocations;
        }

        public bool CityFilter(MarketplaceAddressBuyer location, MarketplaceReportFilter filters)
        {
            if (filters.Filters.City != null)
            {
                if (location.City == null || !filters.Filters.City.Contains(location.City))
                {
                    return true;
                }
            }
            return false;
        }

        public bool StateFilter(MarketplaceAddressBuyer location, MarketplaceReportFilter filters)
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

        public bool ZipFilter(MarketplaceAddressBuyer location, MarketplaceReportFilter filters)
        {
            if (filters.Filters.Zip != null)
            {
                if (location.Zip == null || !filters.Filters.Zip.Contains(location.Zip))
                {
                    return true;
                }
            }
            return false;
        }
    }


}
