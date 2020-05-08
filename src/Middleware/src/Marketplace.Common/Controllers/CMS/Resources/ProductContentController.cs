using Integrations.CMS.Models;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers.CMS.Resources
{
	[MarketplaceSection.Content(ListOrder = 1)]
	[Route("products/{productID}")]
	public class ProductContentController : BaseController
	{
		private readonly IAssetedResourceQuery _assetedResources;
		private ResourceType type { get; } = ResourceType.Products;

		public ProductContentController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings)
		{
			_assetedResources = assetedResources;
		}

		// Content Delivery

		// TODO - add list page and list args
		[DocName("Get Assets Assigned to Resource")]
		[HttpGet, Route("assets"), MarketplaceUserAuth]
		public async Task<List<AssetForDelivery>> ListAssets(string productID)
		{
			var resource = new Resource(type, productID);
			return await _assetedResources.ListAssets(resource, VerifiedUserContext);
		}

		[DocName("Get Resource's primary image")]
		[HttpGet, Route("image")] // No auth
		public async Task GetFirstImage(string productID)
		{
			var resource = new Resource(type, productID);
			var url = await _assetedResources.GetFirstImage(resource, VerifiedUserContext);
			Response.Redirect(url);
		}

		// Content Admin
		[DocName("Assign Asset to Resource")]
		[HttpPost, Route("assets/{assetID}/assignments"), MarketplaceUserAuth]
		public async Task SaveAssetAssignment(string productID, string assetID)
		{
			var resource = new Resource(type, productID);
			await _assetedResources.SaveAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Remove Asset from Resource"), MarketplaceUserAuth]
		[HttpDelete, Route("assets/{assetID}/assignments")]
		public async Task DeleteAssetAssignment(string productID, string assetID)
		{
			var resource = new Resource(type, productID);
			await _assetedResources.DeleteAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), MarketplaceUserAuth]
		[HttpPost, Route("assets/{assetID}/assignments/moveto/{listOrderWithinType}")]
		public async Task MoveAssetAssignment(string productID, string assetID, int listOrderWithinType)
		{
			var resource = new Resource(type, productID);
			await _assetedResources.MoveAssignment(resource, assetID, listOrderWithinType, VerifiedUserContext);
		}
	}
}
