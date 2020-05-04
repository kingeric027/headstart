using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers.CMS
{

	[DocComments("\"Integration\" represents Asset Delivery")]
	[MarketplaceSection.Integration(ListOrder = 6)]
	public class AssetDeliveryController : BaseController
	{
		private readonly IAssetQuery _assets;
		private readonly IAssetAssignmentQuery _assignments;

		public AssetDeliveryController(AppSettings settings, IAssetQuery assets, IAssetAssignmentQuery assignments) : base(settings)
		{
			_assets = assets;
			_assignments = assignments;
		}

		[DocName("List Assets")]
		[HttpGet, Route(""), MarketplaceUserAuth]
		public async Task<ListPage<Asset>> List(ListArgs<Asset> args)
		{
			return await _assets.List(args, VerifiedUserContext);
		}
	}
}
