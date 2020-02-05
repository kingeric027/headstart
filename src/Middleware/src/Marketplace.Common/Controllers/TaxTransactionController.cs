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
using Marketplace.Common.Services.AvaTax;

namespace Marketplace.Common.Controllers
{
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
		[HttpPost, Route(""), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceOrder> ApplyTaxEstimate(string orderID)
		{
			return await _taxCommand.ApplyTaxEstimate(orderID);
		}
	}
}
