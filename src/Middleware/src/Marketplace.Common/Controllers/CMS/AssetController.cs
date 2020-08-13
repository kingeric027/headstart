using Marketplace.Common;
using Marketplace.Common.Controllers;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using System.Collections.Generic;
using Marketplace.Models.Misc;

namespace Marketplace.CMS.Controllers
{
	[DocComments("\"Integration\" represents Assets")]
	[MarketplaceSection.Content(ListOrder = 1)]
	[Route("assets")]
	public class AssetController : BaseController
	{
		private readonly IAssetQuery _assets;
		private readonly IAssetedResourceQuery _assetedResources;

		public AssetController(AppSettings settings, IAssetQuery assets, IAssetedResourceQuery assetedResources) : base(settings)
		{ 
			_assets = assets;
			_assetedResources = assetedResources;
		}

		[DocName("List Assets")]
		[HttpGet, Route(""), OrderCloudIntegrationsAuth]
		public async Task<ListPage<Asset>> List(ListArgs<Asset> args)
		{
			RequireOneOf(CustomRole.AssetAdmin, CustomRole.AssetReader);
			return await _assets.List(args, VerifiedUserContext);
		}

		[DocName("Get an Asset")]
		[HttpGet, Route("{assetID}"), OrderCloudIntegrationsAuth]
		public async Task<Asset> Get(string assetID)
		{
			RequireOneOf(CustomRole.AssetAdmin, CustomRole.AssetReader);
			return await _assets.Get(assetID, VerifiedUserContext);
		}

		[DocName("Upload an Asset")]
		[DocIgnore] // For now, hide from swagger reflection b/c it doesn't handle file uploads well. 
		[HttpPost, Route(""), OrderCloudIntegrationsAuth]
		public async Task<Asset> Create([FromForm] AssetUpload form)
		{
			RequireOneOf(CustomRole.AssetAdmin);
			return await _assets.Create(form, VerifiedUserContext);
		}

		[DocName("Update an Asset")]
		[HttpPut, Route("{assetID}"), OrderCloudIntegrationsAuth]
		public async Task<Asset> Save(string assetID, [FromBody] Asset asset)
		{
			RequireOneOf(CustomRole.AssetAdmin);
			return await _assets.Save(assetID, asset, VerifiedUserContext);
		}

		[DocName("Delete an Asset")]
		[HttpDelete, Route("{assetID}"), OrderCloudIntegrationsAuth]
		public async Task Delete(string assetID)
		{
			RequireOneOf(CustomRole.AssetAdmin);
			await _assets.Delete(assetID, VerifiedUserContext);
		}

		[DocName("Create Asset Assignment")]
		[HttpPost, Route("assignments"), OrderCloudIntegrationsAuth]
		public async Task SaveAssetAssignment([FromBody] AssetAssignment assignment)
		{
			RequireOneOf(CustomRole.AssetAdmin);
			await _assetedResources.SaveAssignment(assignment, VerifiedUserContext);
		}

		[DocName("Delete Asset Assignment"), OrderCloudIntegrationsAuth]
		[HttpDelete, Route("assignments")]
		public async Task DeleteAssetAssignment([FromQuery] AssetAssignment assignment)
		{
			RequireOneOf(CustomRole.AssetAdmin);
			await _assetedResources.DeleteAssignment(assignment, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), OrderCloudIntegrationsAuth]
		[HttpPost, Route("assignments/moveto/{listOrderWithinType}")]
		public async Task ReorderAssetAssignment(int listOrderWithinType, [FromBody] AssetAssignment assignment)
		{
			RequireOneOf(CustomRole.AssetAdmin);
			await _assetedResources.MoveAssignment(assignment, listOrderWithinType, VerifiedUserContext);
		}

		[DocName("List Assets Assigned to Resource")]
		[HttpGet, Route("{type}/{ID}"), OrderCloudIntegrationsAuth]
		public async Task<ListPage<Asset>> ListAssets(ResourceType type, string ID, [FromQuery] ListArgsPageOnly args)
		{
			var resource = new Resource(type, ID);
			return await _assetedResources.ListAssets(resource, args, VerifiedUserContext);
		}

		[DocName("List Assets Assigned to Resource")]
		[HttpGet, Route("{parentType}/{parentID}/{type}/{ID}"), OrderCloudIntegrationsAuth]
		public async Task<ListPage<Asset>> ListAssets(ParentResourceType parentType, string parentID, ResourceType type, string ID, [FromQuery] ListArgsPageOnly args)
		{
			var resource = new Resource(type, ID, parentType, parentID);
			return await _assetedResources.ListAssets(resource, args, VerifiedUserContext);
		}

		[DocName("Get a Resource's primary image")]
		[HttpGet, Route("{type}/{ID}/thumbnail")] // No auth
		public async Task GetThumbnail(ResourceType type, string ID, [FromQuery] ThumbSize size = ThumbSize.M)
		{
			var resource = new Resource(type, ID);
			var url = await _assetedResources.GetThumbnail(resource, size, VerifiedUserContext);
			Response.Redirect(url);
		}

		[DocName("Get a Resource's primary image")]
		[HttpGet, Route("{parentType}/{parentID}/{type}/{ID}/thumbnail")] // No auth
		public async Task GetThumbnail(ParentResourceType parentType, string parentID, ResourceType type, string ID, [FromQuery] ThumbSize size = ThumbSize.M)
		{
			var resource = new Resource(type, ID, parentType, parentID);
			var url = await _assetedResources.GetThumbnail(resource, size, VerifiedUserContext);
			Response.Redirect(url);
		}
	}
}
