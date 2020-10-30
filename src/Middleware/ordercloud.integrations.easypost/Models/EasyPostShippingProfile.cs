using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.easypost
{
    public class EasyPostShippingProfile
    {
        public string ID { get; set; }
        public string SupplierID { get; set; }
        public string CarrierAccountID { get; set; }
        public string Customs_Signer { get; set; }
        public string Restriction_Type { get; set; }
        public string EEL_PFC { get; set; }
        public bool Customs_Certify { get; set; }
        public string HS_Tariff_Number { get; set; } = null;
        public decimal Markup { get; set; }
    }
}
