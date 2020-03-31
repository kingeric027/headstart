using System.ComponentModel.DataAnnotations;
using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Marketplace.Models.Misc
{
    [SwaggerModel]
    public class CreditCardToken
    {
        public string AccountNumber { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        [MinLength(4, ErrorMessage = "Invalid expiration date format: MMYY or MMYYYY")]
        [MaxLength(6, ErrorMessage = "Invalid expiration date format: MMYY or MMYYYY")]
        public string ExpirationDate { get; set; }
        public string CardholderName { get; set; }
        public string CardType { get; set; }
		[Required]
		public Address CCBilingAddress { get; set; }
    }

    [SwaggerModel]
	public class CreditCardPayment
	{
		[System.ComponentModel.DataAnnotations.Required]
		public string OrderID { get; set; }
		public string CreditCardID { get; set; } // Use for saved Credit Cards
		public CreditCardToken CreditCardDetails { get; set; }  // Use for one-time Credit Cards
		[System.ComponentModel.DataAnnotations.Required]
		[MinLength(3, ErrorMessage = "Invalid currency specified: Must be 3 digit code. Ex: USD or CAD")]
		[MaxLength(3, ErrorMessage = "Invalid currency specified: Must be 3 digit code. Ex: USD or CAD")]
		public string Currency { get; set; }
		[MinLength(3, ErrorMessage = "Invalid CVV: Must be 3 or 4 digit code.")]
		[MaxLength(4, ErrorMessage = "Invalid CVV: Must be 3 or 4 digit code.")]
		public string CVV { get; set; }
		[System.ComponentModel.DataAnnotations.Required]
		public string MerchantID { get; set; }
	}

	public static class CreditCardPaymentExtensions
	{
		public static bool IsValidCvv(this CreditCardPayment payment, BuyerCreditCard cc)
		{
			// if credit card is direct without using a saved card then consider it a ME card and should enforce CVV
			// saved credit cards for ME just require CVV
			return (payment.CreditCardDetails == null || payment.CVV != null) && (!cc.Editable || payment.CVV != null);
		}
	}
}
