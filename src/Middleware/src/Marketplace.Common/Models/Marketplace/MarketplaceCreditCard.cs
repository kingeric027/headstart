using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models.Models.Marketplace
{
	[SwaggerModel]
	public class MarketplaceCreditCard: CreditCard<CreditCardXP>
	{

	}

	[SwaggerModel]
	public class MarketplaceBuyerCreditCard : BuyerCreditCard<CreditCardXP>
	{

	}

	[SwaggerModel]
	public class CreditCardXP
	{
		[Required]
		public Address CCBillingAddress { get; set; }
	}
}
