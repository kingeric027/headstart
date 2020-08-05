using Marketplace.Common;
using Marketplace.Common.Controllers;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using System.Collections.Generic;

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
			return await _assets.List(args, VerifiedUserContext);
		}

		[DocName("Get an Asset")]
		[HttpGet, Route("{assetID}"), OrderCloudIntegrationsAuth]
		public async Task<Asset> Get(string assetID)
		{
			return await _assets.Get(assetID, VerifiedUserContext);
		}

		[DocName("Upload an Asset")]
		[DocIgnore] // For now, hide from swagger reflection b/c it doesn't handle file uploads well. 
		[HttpPost, Route(""), OrderCloudIntegrationsAuth]
		public async Task<Asset> Create([FromForm] AssetUpload form)
		{
			return await _assets.Create(form, VerifiedUserContext);
		}

		[DocName("Update an Asset")]
		[HttpPut, Route("{assetID}"), OrderCloudIntegrationsAuth]
		public async Task<Asset> Save(string assetID, [FromBody] Asset asset)
		{
			return await _assets.Save(assetID, asset, VerifiedUserContext);
		}

		[DocName("Delete an Asset")]
		[HttpDelete, Route("{assetID}"), OrderCloudIntegrationsAuth]
		public async Task Delete(string assetID)
		{
			await _assets.Delete(assetID, VerifiedUserContext);
		}

		[DocName("Create Asset Assignment")]
		[HttpPost, Route("assignments"), OrderCloudIntegrationsAuth]
		public async Task SaveAssetAssignment([FromBody] AssetAssignment assignment)
		{
			await _assetedResources.SaveAssignment(assignment, VerifiedUserContext);
		}

		[DocName("Delete Asset Assignment"), OrderCloudIntegrationsAuth]
		[HttpDelete, Route("assignments")]
		public async Task DeleteAssetAssignment([FromQuery] AssetAssignment assignment)
		{
			await _assetedResources.DeleteAssignment(assignment, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), OrderCloudIntegrationsAuth]
		[HttpPost, Route("assignments/moveto/{listOrderWithinType}")]
		public async Task ReorderAssetAssignment(int listOrderWithinType, [FromBody] AssetAssignment assignment)
		{
			await _assetedResources.MoveAssignment(assignment, listOrderWithinType, VerifiedUserContext);
		}

		// TODO - add list page and list args
		[DocName("List Assets Assigned to Resource")]
		[HttpGet, Route("resource"), OrderCloudIntegrationsAuth]
		public async Task<List<AssetForDelivery>> ListAssets([FromQuery] Resource resource)
		{
			return await _assetedResources.ListAssets(resource, VerifiedUserContext);
		}

		[DocName("Get a Resource's primary image")]
		[HttpGet, Route("resource/primary-image")] // No auth
		public async Task GetFirstImage([FromQuery] Resource resource)
		{
			var url = await _assetedResources.GetFirstImage(resource, VerifiedUserContext);
			Response.Redirect(url);
		}
	}
}
