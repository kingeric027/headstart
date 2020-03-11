using System.ComponentModel.DataAnnotations;

namespace Marketplace.Models.Misc
{
    public class CreditCardToken
    {
        public string AccountNumber { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "Invalid expiration date format: MMYY or MMYYYY")]
        [MaxLength(6, ErrorMessage = "Invalid expiration date format: MMYY or MMYYYY")]
        public string ExpirationDate { get; set; }
        public string CardholderName { get; set; }
        public string CardType { get; set; }
    }

	public class CreditCardPayment
	{
		[Required]
		public string OrderID { get; set; }
		public string CreditCardID { get; set; } // Use for saved Credit Cards
		public CreditCardToken CreditCardDetails { get; set; }  // Use for one-time Credit Cards
		[Required]
		[MinLength(3, ErrorMessage = "Invalid currency specified: Must be 3 digit code. Ex: USD or CAD")]
		[MaxLength(3, ErrorMessage = "Invalid currency specified: Must be 3 digit code. Ex: USD or CAD")]
		public string Currency { get; set; }
		[MinLength(3, ErrorMessage = "Invalid CVV: Must be 3 or 4 digit code.")]
		[MaxLength(4, ErrorMessage = "Invalid CVV: Must be 3 or 4 digit code.")]
		public string CVV { get; set; }
		[System.ComponentModel.DataAnnotations.Required]
		public string MerchantID { get; set; }
	}
}
