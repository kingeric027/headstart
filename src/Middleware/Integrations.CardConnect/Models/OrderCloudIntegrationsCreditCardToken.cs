using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ordercloud.integrations.cardconnect.Models
{
	[SwaggerModel]
	public class OrderCloudIntegrationsCreditCardToken
    {
        public string AccountNumber { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "Invalid expiration date format: MMYY or MMYYYY")]
        [MaxLength(6, ErrorMessage = "Invalid expiration date format: MMYY or MMYYYY")]
        public string ExpirationDate { get; set; }
        public string CardholderName { get; set; }
        public string CardType { get; set; }
        [Required]
        public Address CCBillingAddress { get; set; }
    }

    
}
