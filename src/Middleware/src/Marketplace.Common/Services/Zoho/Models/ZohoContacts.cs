using System.Collections.Generic;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Models
{
    public class ZohoListContactList : ZohoListResponse
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

    public class ZohoAddress
    {
        public string attention { get; set; }
        public string address { get; set; }
        public string street2 { get; set; }
        public string state_code { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string fax { get; set; }
        public string phone { get; set; }
    }

    public class ZohoDefaultTemplates
    {
        public string invoice_template_id { get; set; }
        public string estimate_template_id { get; set; }
        public string creditnote_template_id { get; set; }
        public string purchaseorder_template_id { get; set; }
        public string salesorder_template_id { get; set; }
        public string retainerinvoice_template_id { get; set; }
        public string paymentthankyou_template_id { get; set; }
        public string retainerinvoice_paymentthankyou_template_id { get; set; }
        public string invoice_email_template_id { get; set; }
        public string estimate_email_template_id { get; set; }
        public string creditnote_email_template_id { get; set; }
        public string purchaseorder_email_template_id { get; set; }
        public string salesorder_email_template_id { get; set; }
        public string retainerinvoice_email_template_id { get; set; }
        public string paymentthankyou_email_template_id { get; set; }
        public string retainerinvoice_paymentthankyou_email_template_id { get; set; }
    }

    public class ZohoContactPerson
    {
        public string salutation { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string designation { get; set; }
        public string department { get; set; }
        public string skype { get; set; }
        public bool is_primary_contact { get; set; } = false;
        public bool enable_portal { get; set; }
    }

    public class ZohoCustomFields
    {
        public int index { get; set; }
        public string value { get; set; }
    }

    //public class ZohoContact
    //{
    //    /// <summary>
    //    /// Gets or sets the contact_id.
    //    /// </summary>
    //    /// <value>The contact_id.</value>
    //    public string contact_id { get; set; }
    //    /// <summary>
    //    /// Gets or sets the contact_name.
    //    /// </summary>
    //    /// <value>The contact_name.</value>
    //    public string contact_name { get; set; }
    //    /// <summary>
    //    /// Gets or sets a value indicating whether this <see cref="Models.ZohoContact" /> is has_transaction.
    //    /// </summary>
    //    /// <value><c>true</c> if has_transaction; otherwise, <c>false</c>.</value>
    //    public bool has_transaction { get; set; }
    //    /// <summary>
    //    /// Gets or sets the contact_type.
    //    /// </summary>
    //    /// <value>The contact_type.</value>
    //    public string contact_type { get; set; }

    //    /// <summary>
    //    /// Gets or sets the primary_contact_id.
    //    /// </summary>
    //    /// <value>The primary_contact_id.</value>
    //    public string primary_contact_id { get; set; }
    //    /// <summary>
    //    /// Gets or sets the payment_terms.
    //    /// </summary>
    //    /// <value>The payment_terms.</value>
    //    public int payment_terms { get; set; }
    //    /// <summary>
    //    /// Gets or sets the payment_terms_label.
    //    /// </summary>
    //    /// <value>The payment_terms_label.</value>
    //    public string payment_terms_label { get; set; }
    //    /// <summary>
    //    /// Gets or sets the currency_id.
    //    /// </summary>
    //    /// <value>The currency_id.</value>
    //    public string currency_id { get; set; }
    //    /// <summary>
    //    /// Gets or sets the currency_code.
    //    /// </summary>
    //    /// <value>The currency_code.</value>
    //    public string currency_code { get; set; }
    //    /// <summary>
    //    /// Gets or sets the currency_symbol.
    //    /// </summary>
    //    /// <value>The currency_symbol.</value>
    //    public string currency_symbol { get; set; }
    //    /// <summary>
    //    /// Gets or sets the outstanding_receivable_amount.
    //    /// </summary>
    //    /// <value>The outstanding_receivable_amount.</value>
    //    public double outstanding_receivable_amount { get; set; }
    //    /// <summary>
    //    /// Gets or sets the outstanding_receivable_amount_bcy.
    //    /// </summary>
    //    /// <value>The outstanding_receivable_amount_bcy.</value>
    //    public double outstanding_receivable_amount_bcy { get; set; }
    //    /// <summary>
    //    /// Gets or sets the outstanding_payable_amount.
    //    /// </summary>
    //    /// <value>The outstanding_payable_amount.</value>
    //    public double outstanding_payable_amount { get; set; }
    //    /// <summary>
    //    /// Gets or sets the outstanding_payable_amount_bcy.
    //    /// </summary>
    //    /// <value>The outstanding_payable_amount_bcy.</value>
    //    public double outstanding_payable_amount_bcy { get; set; }
    //    /// <summary>
    //    /// Gets or sets the unused_credits_receivable_amount.
    //    /// </summary>
    //    /// <value>The unused_credits_receivable_amount.</value>
    //    public double unused_credits_receivable_amount { get; set; }
    //    /// <summary>
    //    /// Gets or sets the unused_credits_receivable_amount_bcy.
    //    /// </summary>
    //    /// <value>The unused_credits_receivable_amount_bcy.</value>
    //    public double unused_credits_receivable_amount_bcy { get; set; }
    //    /// <summary>
    //    /// Gets or sets the unused_credits_payable_amount.
    //    /// </summary>
    //    /// <value>The unused_credits_payable_amount.</value>
    //    public double unused_credits_payable_amount { get; set; }
    //    /// <summary>
    //    /// Gets or sets the unused_credits_payable_amount_bcy.
    //    /// </summary>
    //    /// <value>The unused_credits_payable_amount_bcy.</value>
    //    public double unused_credits_payable_amount_bcy { get; set; }
    //    /// <summary>
    //    /// Gets or sets the status.
    //    /// </summary>
    //    /// <value>The status.</value>
    //    public string status { get; set; }
    //    /// <summary>
    //    /// Gets or sets a value indicating whether this <see cref="Models.ZohoContact" /> is payment_reminder_enabled.
    //    /// </summary>
    //    /// <value><c>true</c> if payment_reminder_enabled; otherwise, <c>false</c>.</value>
    //    public bool payment_reminder_enabled { get; set; }
    //    /// <summary>
    //    /// Gets or sets the custom_fields.
    //    /// </summary>
    //    /// <value>The custom_fields.</value>
    //    public List<ZohoCustomField> custom_fields { get; set; }
    //    /// <summary>
    //    /// Gets or sets the BillingZohoAddress.
    //    /// </summary>
    //    /// <value>The BillingZohoAddress.</value>
    //    public ZohoAddress billing_address { get; set; }
    //    /// <summary>
    //    /// Gets or sets the ShippingZohoAddress.
    //    /// </summary>
    //    /// <value>The ShippingZohoAddress.</value>
    //    public ZohoAddress shipping_address { get; set; }
    //    /// <summary>
    //    /// Gets or sets the contact_persons.
    //    /// </summary>
    //    /// <value>The contact_persons.</value>
    //    public List<ZohoContactPerson> contact_persons { get; set; }
    //    /// <summary>
    //    /// Gets or sets the ZohoDefaultTemplates.
    //    /// </summary>
    //    /// <value>The ZohoDefaultTemplates.</value>
    //    public ZohoDefaultTemplates default_templates { get; set; }
    //    /// <summary>
    //    /// Gets or sets the notes.
    //    /// </summary>
    //    /// <value>The notes.</value>
    //    public string notes { get; set; }
    //    /// <summary>
    //    /// Gets or sets the created_time.
    //    /// </summary>
    //    /// <value>The created_time.</value>
    //    public string created_time { get; set; }
    //    /// <summary>
    //    /// Gets or sets the last_modified_time.
    //    /// </summary>
    //    /// <value>The last_modified_time.</value>
    //    public string last_modified_time { get; set; }
    //    /// <summary>
    //    /// Gets or sets a value indicating whether this <see cref="Models.ZohoContact" /> is track_1099.
    //    /// </summary>
    //    /// <value><c>true</c> if track_1099; otherwise, <c>false</c>.</value>
    //    public bool track_1099 { get; set; }
    //    /// <summary>
    //    /// Gets or sets the tax_id_type.
    //    /// </summary>
    //    /// <value>The tax_id_type.</value>
    //    public string tax_id_type { get; set; }
    //    /// <summary>
    //    /// Gets or sets the tax_id_value.
    //    /// </summary>
    //    /// <value>The tax_id_value.</value>
    //    public string tax_id_value { get; set; }
    //    /// <summary>
    //    /// Gets or sets the email.
    //    /// </summary>
    //    /// <value>The email.</value>
    //    public string email { get; set; }
    //    /// <summary>
    //    /// Gets or sets a value indicating whether this <see cref="Models.ZohoContact" /> is snail_mail.
    //    /// </summary>
    //    /// <value><c>true</c> if snail_mail; otherwise, <c>false</c>.</value>
    //    public bool snail_mail { get; set; }
    //    /// <summary>
    //    /// Gets or sets the company_name.
    //    /// </summary>
    //    /// <value>The company_name.</value>
    //    public string company_name { get; set; }
    //    /// <summary>
    //    /// Gets or sets the first_name.
    //    /// </summary>
    //    /// <value>The first_name.</value>
    //    public string first_name { get; set; }
    //    /// <summary>
    //    /// Gets or sets the last_name.
    //    /// </summary>
    //    /// <value>The last_name.</value>
    //    public string last_name { get; set; }
    //    /// <summary>
    //    /// Gets or sets the phone.
    //    /// </summary>
    //    /// <value>The phone.</value>
    //    public string phone { get; set; }
    //    /// <summary>
    //    /// Gets or sets the mobile.
    //    /// </summary>
    //    /// <value>The mobile.</value>
    //    public string mobile { get; set; }
    //    /// <summary>
    //    /// Gets or sets a value indicating whether this <see cref="Models.ZohoContact"/> is is_linked_with_zohocrm.
    //    /// </summary>
    //    /// <value><c>true</c> if is_linked_with_zohocrm; otherwise, <c>false</c>.</value>
    //    public bool is_linked_with_zohocrm { get; set; }
    //    /// <summary>
    //    /// Gets or sets the website.
    //    /// </summary>
    //    /// <value>The website.</value>
    //    public string website { get; set; }

    //    /// <summary>
    //    /// Gets or sets the contact_salutation.
    //    /// </summary>
    //    /// <value>The contact_salutation.</value>
    //    public string contact_salutation { get; set; }

    //    /// <summary>
    //    /// Gets or sets the source.
    //    /// </summary>
    //    /// <value>The source.</value>
    //    public string source { get; set; }
    //    /// <summary>
    //    /// Gets or sets a value indicating whether this <see cref="Models.ZohoContact"/> is is_crm_customer.
    //    /// </summary>
    //    /// <value><c>true</c> if is_crm_customer; otherwise, <c>false</c>.</value>
    //    public bool is_crm_customer { get; set; }

    //    /// <summary>
    //    /// Gets or sets the price_precision.
    //    /// </summary>
    //    /// <value>The price_precision.</value>
    //    public int price_precision { get; set; }


    //    /// <summary>
    //    /// Gets or sets the pricebook_id.
    //    /// </summary>
    //    /// <value>The pricebook_id.</value>
    //    public string pricebook_id { get; set; }
    //    /// <summary>
    //    /// Gets or sets the pricebook_name.
    //    /// </summary>
    //    /// <value>The pricebook_name.</value>
    //    public string pricebook_name { get; set; }

    //    /// <summary>
    //    /// Gets or sets a value indicating whether this <see cref="Models.ZohoContact"/> is associated_with_square.
    //    /// </summary>
    //    /// <value><c>true</c> if associated_with_square; otherwise, <c>false</c>.</value>
    //    public bool associated_with_square { get; set; }
    //    /// <summary>
    //    /// Gets or sets the cards.
    //    /// </summary>
    //    /// <value>The cards.</value>
    //    public List<object> cards { get; set; }
    //    //public string avatax_exempt_no { get; set; }
    //    //public string avatax_use_code { get; set; }
    //}
}
