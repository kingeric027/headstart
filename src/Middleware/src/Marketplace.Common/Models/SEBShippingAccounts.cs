using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using ordercloud.integrations.easypost;

namespace Marketplace.Common.Models
{
    public class SelfEsteemBrandsShippingProfiles : EasyPostShippingProfiles
    {
        public SelfEsteemBrandsShippingProfiles(AppSettings settings)
        {
            this.ShippingProfiles.Add(new EasyPostShippingProfile()
            {
                ID = "Provision",
                SupplierID = settings.OrderCloudSettings.ProvisionSupplierID,
                CarrierAccountID = settings.EasyPostSettings.ProvisionFedexAccountId,
                Customs_Signer = "Christa Zaspel",
                Restriction_Type = "none",
                EEL_PFC = "NOEEI30.37(a)",
                Customs_Certify = true,
                Markup = 1M,
                Default = false,
                HS_Tariff_Number = "8523.59.0000"
            });
            this.ShippingProfiles.Add(new EasyPostShippingProfile()
            {
                ID = "SMG",
                SupplierID = null,
                CarrierAccountID = settings.EasyPostSettings.SMGFedexAccountId,
                Customs_Signer = "Bob Bernier",
                Restriction_Type = "none",
                EEL_PFC = "NOEEI30.37(a)",
                Customs_Certify = true,
                Markup = 1.5M,
                Default = true
            });
            this.ShippingProfiles.Add(new EasyPostShippingProfile()
            {
                ID = "SMG",
                SupplierID = "050",
                CarrierAccountID = settings.EasyPostSettings.SMGFedexAccountId,
                Customs_Signer = "Bob Bernier",
                Restriction_Type = "none",
                EEL_PFC = "NOEEI30.37(a)",
                Customs_Certify = true,
                Markup = 1.5M,
                Default = false,
                AllowedServiceFilter = new List<string>() { "FEDEX_GROUND" }
            });
            this.ShippingProfiles.Add(new EasyPostShippingProfile()
            {
                ID = "SEB",
                SupplierID = settings.OrderCloudSettings.SEBDistributionSupplierID,
                CarrierAccountID = settings.EasyPostSettings.SEBDistributionFedexAccountId,
                Customs_Signer = "Christa Zaspel",
                Restriction_Type = "none",
                EEL_PFC = "NOEEI30.37(a)",
                Customs_Certify = true,
                Markup = 1.1M,
                Default = false
            });
        }

        public override EasyPostShippingProfile FirstOrDefault(string id)
        {
            return ShippingProfiles.FirstOrDefault(p => p.SupplierID == id) ?? ShippingProfiles.First(p => p.ID == "SMG");
        }
    }
}
