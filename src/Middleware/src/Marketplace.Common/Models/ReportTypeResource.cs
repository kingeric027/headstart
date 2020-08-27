using ordercloud.integrations.library;

namespace Marketplace.Common.Models
{
    [SwaggerModel]
    public class ReportTypeResource
    {
        public ReportTypeEnum ID { get; set; }
        public string Name { get; set; }
        public string ReportCategory { get; set; }

        //Explicitly setting available report types
        public static ReportTypeResource[] ReportTypes = {
            new ReportTypeResource
            {
                ID = ReportTypeEnum.BuyerLocation,
                Name = "Buyer Location Report",
                ReportCategory = "Buyer"
            }
        };
    }
}
