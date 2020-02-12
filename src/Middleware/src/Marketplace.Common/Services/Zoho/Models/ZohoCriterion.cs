namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoCriterion.
    /// </summary>
    public class ZohoCriterion
    {
        /// <summary>
        /// Gets or sets the criteria_id.
        /// </summary>
        /// <value>The criteria_id.</value>
        public string criteria_id { get; set; }
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>The field.</value>
        public string field { get; set; }
        /// <summary>
        /// Gets or sets the comparator.
        /// </summary>
        /// <value>The comparator.</value>
        public string comparator { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object value { get; set; }
    }
}
