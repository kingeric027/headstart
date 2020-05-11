using Integrations.CMS.Models;
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

namespace Marketplace.Common.Controllers.CMS.Resources
{
	[MarketplaceSection.Content(ListOrder = 1)]
	[Route("promotion/{promotionID}")]
	public class PromotionContentController : BaseController
	{
		private readonly IAssetedResourceQuery _assetedResources;
		private ResourceType type { get; } = ResourceType.Promotions;

		public PromotionContentController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings)
		{
			_assetedResources = assetedResources;
		}

		// Content Delivery

		// TODO - add list page and list args
		[DocName("Get Assets Assigned to Resource")]
		[HttpGet, Route("assets"), MarketplaceUserAuth]
		public async Task<ListPage<AssetForDelivery>> ListAssets(string promotionID, ListArgs<AssetForDelivery> args)
		{
			var resource = new Resource(type, promotionID);
			return new ListPage<AssetForDelivery>
			{
				Items = await _assetedResources.ListAssets(resource, VerifiedUserContext)
			};
		}

		[DocName("Get Resource's primary image")]
		[HttpGet, Route("image")] // No auth
		public async Task GetFirstImage(string promotionID)
		{
			var resource = new Resource(type, promotionID);
			var url = await _assetedResources.GetFirstImage(resource, VerifiedUserContext);
			Response.Redirect(url);
		}

		// Content Admin
		[DocName("Assign Asset to Resource")]
		[HttpPost, Route("assets/{assetID}/assignments"), MarketplaceUserAuth]
		public async Task SaveAssetAssignment(string promotionID, string assetID)
		{
			var resource = new Resource(type, promotionID);
			await _assetedResources.SaveAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Remove Asset from Resource"), MarketplaceUserAuth]
		[HttpDelete, Route("assets/{assetID}/assignments")]
		public async Task DeleteAssetAssignment(string promotionID, string assetID)
		{
			var resource = new Resource(type, promotionID);
			await _assetedResources.DeleteAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), MarketplaceUserAuth]
		[HttpPost, Route("assets/{assetID}/assignments/moveto/{listOrderWithinType}")]
		public async Task MoveAssetAssignment(string promotionID, string assetID, int listOrderWithinType)
		{
			var resource = new Resource(type, promotionID);
			await _assetedResources.MoveAssignment(resource, assetID, listOrderWithinType, VerifiedUserContext);
		}
	}
}
