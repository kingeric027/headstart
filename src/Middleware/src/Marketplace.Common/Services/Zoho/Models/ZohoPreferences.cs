using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class ZohoPreferences.
    /// </summary>
    public class ZohoPreferences
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is convert_to_invoice.
        /// </summary>
        /// <value><c>true</c> if convert_to_invoice; otherwise, <c>false</c>.</value>
        public bool convert_to_invoice { get; set; }
        /// <summary>
        /// Gets or sets the attach_pdf_for_email.
        /// </summary>
        /// <value>The attach_pdf_for_email.</value>
        public string attach_pdf_for_email { get; set; }
        /// <summary>
        /// Gets or sets the estimate_approval_status.
        /// </summary>
        /// <value>The estimate_approval_status.</value>
        public string estimate_approval_status { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is notify_me_on_online_payment.
        /// </summary>
        /// <value><c>true</c> if notify_me_on_online_payment; otherwise, <c>false</c>.</value>
        public bool notify_me_on_online_payment { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is send_payment_receipt_acknowledgement.
        /// </summary>
        /// <value><c>true</c> if send_payment_receipt_acknowledgement; otherwise, <c>false</c>.</value>
        public bool send_payment_receipt_acknowledgement { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is auto_notify_recurring_invoice.
        /// </summary>
        /// <value><c>true</c> if auto_notify_recurring_invoice; otherwise, <c>false</c>.</value>
        public string auto_notify_recurring_invoice { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is snail_mail_include_payment_stub.
        /// </summary>
        /// <value><c>true</c> if snail_mail_include_payment_stub; otherwise, <c>false</c>.</value>
        public bool snail_mail_include_payment_stub { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is is_show_powered_by.
        /// </summary>
        /// <value><c>true</c> if is_show_powered_by; otherwise, <c>false</c>.</value>
        public bool is_show_powered_by { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is attach_expense_receipt_to_invoice.
        /// </summary>
        /// <value><c>true</c> if attach_expense_receipt_to_invoice; otherwise, <c>false</c>.</value>
        public bool attach_expense_receipt_to_invoice { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is is_estimate_enabled.
        /// </summary>
        /// <value><c>true</c> if is_estimate_enabled; otherwise, <c>false</c>.</value>
        public bool is_estimate_enabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is is_project_enabled.
        /// </summary>
        /// <value><c>true</c> if is_project_enabled; otherwise, <c>false</c>.</value>
        public bool is_project_enabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is is_purchaseorder_enabled.
        /// </summary>
        /// <value><c>true</c> if is_purchaseorder_enabled; otherwise, <c>false</c>.</value>
        public bool is_purchaseorder_enabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is is_salesorder_enabled.
        /// </summary>
        /// <value><c>true</c> if is_salesorder_enabled; otherwise, <c>false</c>.</value>
        public bool is_salesorder_enabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is is_pricebooks_enabled.
        /// </summary>
        /// <value><c>true</c> if is_pricebooks_enabled; otherwise, <c>false</c>.</value>
        public bool is_pricebooks_enabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is attach_payment_receipt_with_acknowledgement.
        /// </summary>
        /// <value><c>true</c> if attach_payment_receipt_with_acknowledgement; otherwise, <c>false</c>.</value>
        public bool attach_payment_receipt_with_acknowledgement { get; set; }
        /// <summary>
        /// Gets or sets the auto_reminders.
        /// </summary>
        /// <value>The auto_reminders.</value>
        public List<ZohoAutoReminder> auto_reminders { get; set; }
        /// <summary>
        /// Gets or sets the ZohoTerms.
        /// </summary>
        /// <value>The ZohoTerms.</value>
        public ZohoTerms ZohoTerms { get; set; }
        /// <summary>
        /// Gets or sets the ZohoAddressFormats.
        /// </summary>
        /// <value>The ZohoAddressFormats.</value>
        public ZohoAddressFormats ZohoAddressFormats { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences" /> is show_org_address_as_one_field.
        /// </summary>
        /// <value><c>true</c> if show_org_address_as_one_field; otherwise, <c>false</c>.</value>
        public bool show_org_address_as_one_field { get; set; }
        /// <summary>
        /// Gets or sets the allow_auto_categorize.
        /// </summary>
        /// <value>The allow_auto_categorize.</value>
        public string allow_auto_categorize { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPreferences"/> is is_retainerinvoice_enabled.
        /// </summary>
        /// <value><c>true</c> if is_retainerinvoice_enabled; otherwise, <c>false</c>.</value>
        public bool is_retainerinvoice_enabled { get; set; }
    }
}
