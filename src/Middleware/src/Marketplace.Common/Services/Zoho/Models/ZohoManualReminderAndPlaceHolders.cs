namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class ZohoManualReminderAndPlaceHolders.
    /// </summary>
    public class ZohoManualReminderAndPlaceHolders
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public int code { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string message { get; set; }
        /// <summary>
        /// Gets or sets the manualreminder.
        /// </summary>
        /// <value>The manualreminder.</value>
        public ZohoManualReminder manualreminder { get; set; }
        /// <summary>
        /// Gets or sets the placeholders.
        /// </summary>
        /// <value>The placeholders.</value>
        public ZohoPlaceHolders placeholders { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoManualReminderAndPlaceHolders"/> is show_org_address_as_one_field.
        /// </summary>
        /// <value><c>true</c> if show_org_address_as_one_field; otherwise, <c>false</c>.</value>
        public bool show_org_address_as_one_field { get; set; }
    }
}
