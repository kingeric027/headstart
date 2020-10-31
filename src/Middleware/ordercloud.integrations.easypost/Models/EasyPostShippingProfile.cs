using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.easypost
{
    public abstract class EasyPostShippingProfiles
    {
        public IList<EasyPostShippingProfile> ShippingProfiles { get; set; } = new List<EasyPostShippingProfile>();

        protected EasyPostShippingProfiles()
        {
           
        }

        public EasyPostShippingProfile GetByIDOrDefault(string id)
        {
            if (ShippingProfiles.All(p => !p.Default)) throw new InvalidOperationException("No default carrier account specified");
            return ShippingProfiles.FirstOrDefault(p => p.SupplierID == id) ?? ShippingProfiles.First(p => p.Default);
        }
    }

    
    public class EasyPostShippingProfile
    {
        public string ID { get; set; }
        public string SupplierID { get; set; }
        public bool Default { get; set; }
        public string CarrierAccountID { get; set; }
        public string Customs_Signer { get; set; }
        public string Restriction_Type { get; set; }
        public string EEL_PFC { get; set; }
        public bool Customs_Certify { get; set; }
        public string HS_Tariff_Number { get; set; } = null;
        public decimal Markup { get; set; }
    }
}
