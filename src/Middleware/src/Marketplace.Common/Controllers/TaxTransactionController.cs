using Avalara.AvaTax.RestClient;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
	[Route("orders/{orderID}/tax-transaction")]
	public class TaxTransactionController: BaseController
	{
		private readonly IAvataxService _taxService;
		private readonly IOrderCheckoutCommand _checkoutCommand;

		public TaxTransactionController(AppSettings settings, IAvataxService taxService, IOrderCheckoutCommand checkoutCommand) : base(settings) 
		{
			_taxService = taxService;
			_checkoutCommand = checkoutCommand;	
		}

		// Needs more authentication. These methods should only work for a specific user's orders.
		[HttpPost, Route(""), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceOrder> CalcTaxAndPatchOrderAsync(string orderID)
		{
			return await _checkoutCommand.CalcTaxAndPatchOrderAsync(orderID);
		}

		[HttpGet, Route("{transactionID}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<TransactionModel> GetSavedTaxTransactionAsync(string orderID, string transactionID)
		{
			return await _taxService.GetTaxTransactionAsync(transactionID);
		}
	}
}
