using System.Collections.Generic;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Models
{
    public class ZohoContactList : ZohoListResponse
    {
        [JsonProperty(PropertyName = "contacts")]
        public List<ZohoContact> Items { get; set; }
    }

    public class ZohoContact
    {
        public string contact_id { get; set; }
        public string contact_name { get; set; }
        public string company_name { get; set; }
        public string website { get; set; }
        public string contact_type { get; set; }
        public bool is_portal_enabled { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; } // added during order mapping
        public string currency_symbol { get; set; } // added during order mapping
        public int payment_terms { get; set; }
        public string payment_terms_label { get; set; }
        public string notes { get; set; }
        public ZohoAddress billing_address { get; set; }
        public ZohoAddress shipping_address { get; set; }
        public List<ZohoContactPerson> contact_persons { get; set; }
        public ZohoDefaultTemplates default_templates { get; set; }
        public List<ZohoCustomFields> custom_fields { get; set; }
        public string owner_id { get; set; }
        //public string tax_reg_no { get; set; }
        //public string place_of_contact { get; set; }
        //public string gst_no { get; set; }
        //public string gst_treatment { get; set; }
        public string tax_exemption_id { get; set; }
        public string tax_authority_id { get; set; }
        public string tax_id { get; set; }
        public bool is_taxable { get; set; } = true;
        public string facebook { get; set; }
        public string twitter { get; set; }
    }
}
