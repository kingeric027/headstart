using System.Collections.Generic;

namespace Marketplace.Common.Services.FreightPop.Models
{
    public class GetRatesData
    {
        public List<ShippingRate> Rates { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
