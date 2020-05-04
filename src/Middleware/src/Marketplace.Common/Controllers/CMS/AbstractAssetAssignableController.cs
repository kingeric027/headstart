using Integrations.CMS.Models;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Common;
using Marketplace.Common.Controllers;
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

namespace Integrations.CMS
{

	public class AssetAssignableControllerConfig
	{
		public ResourceType ResourceType { get; set; }
		public string ResourceIDFieldName { get; set; }
		public string ResourceParentIDFieldName { get; set; }
	}

	public abstract class AbstractAssetAssignableController : BaseController
	{
		protected abstract AssetAssignableControllerConfig config { get; set; }

		private readonly IAssetAssignmentQuery _assignments;

		public AbstractAssetAssignableController(AppSettings settings, IAssetAssignmentQuery assignments) : base(settings)
		{
			_assignments = assignments;
		}

		[DocName("List assets")]
		[HttpGet, Route("container/{containerID}/assets"), MarketplaceUserAuth]
		public async Task<ListPage<Asset>> ListAssets(string containerID, ListArgs<Asset> args)
		{
			var (resourceID, resourceParentID) = GetResourceIDs();

		}

		[DocName("Save an asset assignment")]
		[HttpPost, Route("container/{containerID}/assets/{assetID}"), MarketplaceUserAuth]
		public async Task SaveAssetAssignment(string containerID, string assetID)
		{
			var (resourceID, resourceParentID) = GetResourceIDs();
			await _assignments.Save(containerID, new AssetAssignment()
			{
				ResourceID = resourceID,
				ResourceType = config.ResourceType,
				ResourceParentID = resourceParentID,
				AssetID = assetID
			}, VerifiedUserContext);
		}

		[DocName("Delete an asset assignment")]
		[HttpDelete, Route("container/{containerID}/assets/{assetID}"), MarketplaceUserAuth]
		public async Task DeleteAssetAssignment(string containerID, string assetID)
		{
			var (resourceID, resourceParentID) = GetResourceIDs();
			await _assignments.Delete(containerID, new AssetAssignment()
			{
				ResourceID = resourceID,
				ResourceType = config.ResourceType,
				ResourceParentID = resourceParentID,
				AssetID = assetID
			}, VerifiedUserContext);
		}

		private (string, string) GetResourceIDs()
		{
			var resourceID = RouteData.Values[config.ResourceIDFieldName].ToString();
			var resourceParentID = config.ResourceParentIDFieldName == null ? null : RouteData.Values[config.ResourceParentIDFieldName].ToString();
			return (resourceID, resourceParentID);
		}
	}

	[DocComments("\"Integration\" represent Product Assets")]
	[MarketplaceSection.Integration(ListOrder = 6)]
	[Route("products/{productID}")]
	public class ProductAssetController : AbstractAssetAssignableController
	{
		protected override AssetAssignableControllerConfig config { get; set; } =
			new AssetAssignableControllerConfig()
			{
				ResourceType = ResourceType.Products,
				ResourceIDFieldName = "productID",
				ResourceParentIDFieldName = null,
			};
		public ProductAssetController(AppSettings settings, IAssetAssignmentQuery assignments) : base(settings, assignments) {}
	}
}
