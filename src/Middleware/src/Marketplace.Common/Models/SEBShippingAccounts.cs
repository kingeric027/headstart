using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ordercloud.integrations.easypost;

namespace Marketplace.Common.Models
{
    public class SelfEsteemBrandsShippingProfiles
    {
        private readonly AppSettings _settings;
        public IList<EasyPostShippingProfile> ShippingProfiles { get; set; } = new List<EasyPostShippingProfile>();

        public SelfEsteemBrandsShippingProfiles(AppSettings settings)
        {
            _settings = settings;
            this.ShippingProfiles.Add(new EasyPostShippingProfile()
            {
                ID = "Provision",
                SupplierID = settings.OrderCloudSettings.ProvisionSupplierID,
                CarrierAccountID = _settings.EasyPostSettings.ProvisionFedexAccountId,
                Customs_Signer = "Christa Zaspel",
                Restriction_Type = "none",
                EEL_PFC = "NOEEI30.37(a)",
                Customs_Certify = true,
                Markup = 1M
            });
            this.ShippingProfiles.Add(new EasyPostShippingProfile()
            {
                ID = "SMG",
                SupplierID = null,
                CarrierAccountID = _settings.EasyPostSettings.SMGFedexAccountId,
                Customs_Signer = "Bob Bernier",
                Restriction_Type = "none",
                EEL_PFC = "NOEEI30.37(a)",
                Customs_Certify = true,
                Markup = 1.4M
            });
            this.ShippingProfiles.Add(new EasyPostShippingProfile()
            {
                ID = "SEB",
                SupplierID = _settings.OrderCloudSettings.SEBDistributionSupplierID,
                CarrierAccountID = _settings.EasyPostSettings.SEBDistributionFedexAccountId,
                Customs_Signer = "Christa Zaspel",
                Restriction_Type = "none",
                EEL_PFC = "NOEEI30.37(a)",
                Customs_Certify = true,
                Markup = 1.1M
            });
        }
    }

    public static class SelfEsteemBrandsShippingProfilesExtensions
    {
        public static EasyPostShippingProfile GetBySupplierId(this SelfEsteemBrandsShippingProfiles profiles, string id)
        {
            return profiles.ShippingProfiles.FirstOrDefault(p => p.SupplierID == id) ?? profiles.ShippingProfiles.First(p => p.ID == "SMG");
        }
    }

    
}
