﻿using System;
using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
	[DocComments("Me and my stuff")]
	[MarketplaceSection.Marketplace(ListOrder = 10)]
	[Route("me")]
	public class MeController : BaseController
	{

		private readonly IMeProductCommand _meProductCommand;
		private readonly IMarketplaceKitProductCommand _kitProductCommand;
		public MeController(AppSettings settings, IMeProductCommand meProductCommand, IMarketplaceKitProductCommand kitProductCommand) : base(settings)
		{
			_meProductCommand = meProductCommand;
			_kitProductCommand = kitProductCommand;
		}

		[DocName("GET Super Product")]
		[HttpGet, Route("products/{productID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<SuperMarketplaceMeProduct> GetSuperProduct(string productID)
		{
			return await _meProductCommand.Get(productID, VerifiedUserContext);
		}

		[DocName("LIST products")]
		[HttpGet, Route("products"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<ListPageWithFacets<MarketplaceMeProduct>> ListMeProducts(ListArgs<MarketplaceMeProduct> args)
		{
			return await _meProductCommand.List(args, VerifiedUserContext);
		}

		[DocName("POST request information about product")]
		[HttpPost, Route("products/requestinfo"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task RequestProductInfo([FromBody] ContactSupplierBody template)
        {
			await _meProductCommand.RequestProductInfo(template);
        }

		[DocName("GET Kit Product")]
		[HttpGet, Route("kitproducts/{kitProductID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<MarketplaceMeKitProduct> GetMeKit(string kitProductID)
		{
			return await _kitProductCommand.GetMeKit(kitProductID, VerifiedUserContext);
		}
	}
}
