using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

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
		public Address CCBillingAddress { get; set; }
	}
}
