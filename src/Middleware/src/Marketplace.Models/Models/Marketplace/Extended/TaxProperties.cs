using System.ComponentModel.DataAnnotations;
using Marketplace.Helpers.Attributes;

namespace Marketplace.Models.Extended
{
    [SwaggerModel]
    public class TaxProperties
    {
        [Required]
        public string Category { get; set; }
        [Required]
        public string Code { get; set; }
        [MaxLength(100, ErrorMessage = "Tax Description cannot exceed 100 characters")]
        public string Description { get; set; }
    }
}
