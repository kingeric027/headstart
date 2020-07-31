using ordercloud.integrations.library;
using OrderCloud.SDK;
using Marketplace.Models.Extended;
using System.Collections.Generic;

namespace Marketplace.Models.Misc
{
    public class MarketplaceReportFilter
    {
        public string ReportType { get; set; }
        public ReportFilters Filters { get; set; }

        public class ReportFilters
        {
            //Master list of all filters that can be used with Admin reports.  May want to break up into subsets for each report?
            public List<string> BuyerID { get; set; }
            public List<string> BuyerLocationID { get; set; }
            public List<string> City { get; set; }
            public List<string> State { get; set; }
            public List<string> Zip { get; set; }
        }
    }
}
