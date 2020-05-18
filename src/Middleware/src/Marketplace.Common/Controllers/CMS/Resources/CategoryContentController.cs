using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.cms;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;

namespace Marketplace.Common.Controllers.CMS.Resources
{
	[MarketplaceSection.Content(ListOrder = 1)]
	[Route("/catalogs/{catalogID}/categories/{categoryID}")]
	public class CategoryContentController : BaseController
	{
		private readonly IAssetedResourceQuery _assetedResources;
		private ResourceType type { get; } = ResourceType.Categories;

		public CategoryContentController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings)
		{
			_assetedResources = assetedResources;
		}

		// Content Delivery

		// TODO - add list page and list args
		[DocName("Get Assets Assigned to Resource")]
		[HttpGet, Route("assets"), OrderCloudIntegrationsAuth]
		public async Task<ListPage<AssetForDelivery>> ListAssets(string catalogID, string categoryID, ListArgs<AssetForDelivery> args)
		{
			var resource = new Resource(type, categoryID, catalogID);
			return new ListPage<AssetForDelivery> {
				Items = await _assetedResources.ListAssets(resource, VerifiedUserContext)
			};
		}

		[DocName("Get Resource's primary image")]
		[HttpGet, Route("image")] // No auth
		public async Task GetFirstImage(string catalogID, string categoryID)
		{
			var resource = new Resource(type, categoryID, catalogID);
			var url = await _assetedResources.GetFirstImage(resource, VerifiedUserContext);
			Response.Redirect(url);
		}

		// Content Admin
		[DocName("Assign Asset to Resource")]
		[HttpPost, Route("assets/{assetID}/assignments"), OrderCloudIntegrationsAuth]
		public async Task SaveAssetAssignment(string catalogID, string categoryID, string assetID)
		{
			var resource = new Resource(type, categoryID, catalogID);
			await _assetedResources.SaveAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Remove Asset from Resource"), OrderCloudIntegrationsAuth]
		[HttpDelete, Route("assets/{assetID}/assignments")]
		public async Task DeleteAssetAssignment(string catalogID, string categoryID, string assetID)
		{
			var resource = new Resource(type, categoryID, catalogID);
			await _assetedResources.DeleteAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), OrderCloudIntegrationsAuth]
		[HttpPost, Route("assets/{assetID}/assignments/moveto/{listOrderWithinType}")]
		public async Task MoveAssetAssignment(string catalogID, string categoryID, string assetID, int listOrderWithinType)
		{
			var resource = new Resource(type, categoryID, catalogID);
			await _assetedResources.MoveAssignment(resource, assetID, listOrderWithinType, VerifiedUserContext);
		}
	}
}

