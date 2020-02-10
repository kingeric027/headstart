namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoCustomField.
    /// </summary>
    public class ZohoCustomField
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int index { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoCustomField" /> is show_on_pdf.
        /// </summary>
        /// <value><c>true</c> if show_on_pdf; otherwise, <c>false</c>.</value>
        public bool show_on_pdf { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string value { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string label { get; set; }
    }
}
