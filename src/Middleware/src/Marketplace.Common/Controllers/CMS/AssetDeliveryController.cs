using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Controllers.CMS
{

	[DocComments("\"Integration\" represents Asset Delivery")]
	[MarketplaceSection.Integration(ListOrder = 6)]
	[Route("containers/{containerID}/assets")]
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
		public async Task<ListPage<Asset>> List(string containerID, ListArgs<Asset> args)
		{
			return await _assets.List(containerID, args);
		}
	}
