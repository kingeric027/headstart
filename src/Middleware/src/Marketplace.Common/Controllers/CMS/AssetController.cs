using Integrations.CMS.Models;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Common;
using Marketplace.Common.Controllers;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Marketplace.CMS.Controllers
{
	[DocComments("\"Integration\" represents Assets")]
	[MarketplaceSection.Integration(ListOrder = 6)]
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
		[HttpGet, Route(""), MarketplaceUserAuth]
		public async Task<ListPage<Asset>> List(ListArgs<Asset> args)
		{
			return await _assets.List(args, VerifiedUserContext);
		}

		[DocName("Get an Asset")]
		[HttpGet, Route("{assetID}"), MarketplaceUserAuth]
		public async Task<Asset> Get(string containerID, string assetID)
		{
			return await _assets.Get(assetID, VerifiedUserContext);
		}

		[DocName("Upoload an Asset")]
		[DocIgnore] // For now, hide from swagger reflection b/c it doesn't handle file uploads well. 
		[HttpPost, Route(""), MarketplaceUserAuth]
		public async Task<Asset> Create([FromForm] AssetUpload form)
		{
			return await _assets.Create(form, VerifiedUserContext);
		}

		[DocName("Update an Asset")]
		[HttpPut, Route("{assetID}"), MarketplaceUserAuth]
		public async Task<Asset> Update(string assetID, [FromBody] Asset asset)
		{
			return await _assets.Update(assetID, asset, VerifiedUserContext);
		}

		[DocName("Delete an Asset")]
		[HttpDelete, Route("{assetID}"), MarketplaceUserAuth]
		public async Task Delete(string assetID)
		{
			await _assets.Delete(assetID, VerifiedUserContext);
		}

		[DocName("Create Asset Assignment")]
		[HttpPost, Route("assignments"), MarketplaceUserAuth]
		public async Task SaveAssignment([FromBody] AssetAssignment assignment)
		{
			await _assetedResources.SaveAssignment(assignment, VerifiedUserContext);
		}

		[DocName("Delete Asset Assignment"), MarketplaceUserAuth]
		[HttpDelete, Route("assignments")]
		public async Task DeleteAssignment([FromBody] AssetAssignment assignment)
		{
			await _assetedResources.DeleteAssignment(assignment, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), MarketplaceUserAuth]
		[HttpPost, Route("assignments/moveto/{listOrder}")]
		public async Task MoveAssignment([FromBody] AssetAssignment assignment, int listOrder)
		{
			await _assetedResources.MoveAssignment(assignment, listOrder, VerifiedUserContext);
		}
	}
}
