using Marketplace.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Common.Services.AvaTax;
using Marketplace.Helpers.SwaggerTools;
using Marketplace.Models;
using Marketplace.Models.Attributes;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Tax Transactions\" represents tax transactions for Orders")]
    [MarketplaceSection.OrdersAndFulfillment(ListOrder = 2)]
    [Route("orders/{orderID}/tax-transaction")]
	public class TaxTransactionController: BaseController
	{
		private readonly IAvataxService _taxService;
		private readonly ITaxCommand _taxCommand;

		public TaxTransactionController(AppSettings settings, IAvataxService taxService, ITaxCommand taxCommand) : base(settings) 
		{
			_taxService = taxService;
			_taxCommand = taxCommand;	
		}

		// Needs more authentication. These methods should only work for a specific user's orders.
        [DocName("POST Tax Estimate")]
		[HttpPost, Route(""), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceOrder> ApplyTaxEstimate(string orderID)
		{
			return await _taxCommand.ApplyTaxEstimate(orderID);
		}
	}
}
