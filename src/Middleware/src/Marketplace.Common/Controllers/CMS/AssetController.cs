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
	[Route("containers/{containerID}/assets")]
	public class AssetController : BaseController
	{
		private readonly IAssetQuery _assets;
		private readonly IAssetAssignmentQuery _assignments;

		public AssetController(AppSettings settings, IAssetQuery assets, IAssetAssignmentQuery assignments) : base(settings)
		{
			_assets = assets;
			_assignments = assignments;
		}

		[DocName("List Assets")]
		[HttpGet, Route("")]
		public async Task<ListPage<Asset>> List(string containerID, ListArgs<Asset> args)
		{
			return await _assets.List(containerID, args);
		}

		[DocName("Get an Asset")]
		[HttpGet, Route("{assetID}")]
		public async Task<Asset> Get(string containerID, string assetID)
		{
			return await _assets.Get(containerID, assetID);
		}

		[DocName("Upoload an Asset")]
		[DocIgnoreAttribute] // For now, hide from swagger reflection b/c it doesn't handle file uploads well. 
		[HttpPost, Route("")]
		public async Task<Asset> Create(string containerID, [FromForm] AssetUpload form)
		{
			return await _assets.Create(containerID, form);
		}

		[DocName("Update an Asset")]
		[HttpPut, Route("{assetID}")]
		public async Task<Asset> Update(string containerID, string assetID, [FromBody] Asset asset)
		{
			return await _assets.Update(containerID, assetID, asset);
		}

		[DocName("Delete an Asset")]
		[HttpDelete, Route("{assetID}")]
		public async Task Delete(string containerID, string assetID)
		{
			await _assets.Delete(containerID, assetID);
		}

		[DocName("List Asset Assignments")]
		[HttpGet, Route("assignments")]
		public async Task<ListPage<AssetAssignment>> ListAssignments(string containerID, ListArgs<Asset> args)
		{
			return await _assignments.List(containerID, args);
		}

		// Route is available to anyone right now, but if your token does not give access to an OC resource, you will get 401 response.
		[DocName("Save Asset Assignment")]
		[HttpPost, Route("assignments"), MarketplaceUserAuth()]
		public async Task SaveAssignment(string containerID, [FromBody] AssetAssignment assignment)
		{
			await _assignments.Save(containerID, assignment, VerifiedUserContext);
		}

		[DocName("Delete Asset Assignment")]
		[HttpDelete, Route("{assetID}/assignments/{resourceType}/{resourceID}")]
		public async Task DeleteAssignment(string containerID, string assetID, ResourceType resourceType, string resourceID)
		{
			await _assignments.Delete(containerID, assetID, resourceType, resourceID);
		}
	}
}
