using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Headstart.Models.Headstart
{
	[SwaggerModel]
	public class HSCreditCard: CreditCard<CreditCardXP>
	{

	}

	[SwaggerModel]
	public class HSBuyerCreditCard : BuyerCreditCard<CreditCardXP>
	{

	}

	[SwaggerModel]
	public class CreditCardXP
	{
		[Required]
		public Address CCBillingAddress { get; set; }
	}
}
