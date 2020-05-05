using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Helpers;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers.CMS
{
	public class ContentControllerConfig
	{
		public ResourceType ResourceType { get; set; }
		public string ResourceIDField { get; set; }
		public string ResourceParentIDField { get; set; }
	}

	public abstract class AbstractContentController : BaseController
	{
		private readonly IAssetedResourceQuery _assetedResources;

		protected abstract ContentControllerConfig config { get; set; }

		public AbstractContentController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings)
		{
			_assetedResources = assetedResources;
		}

		[HttpGet, Route("assets"), MarketplaceUserAuth]
		public async Task<List<Asset>> ListAssets(ListArgs<Asset> args)
		{
			//TODO - use list args
			var (resourceID, resourceParentID) = GetResourceIDs();
			return await _assetedResources.ListAssets(config.ResourceType, resourceID, resourceParentID, VerifiedUserContext);
		}

		[HttpGet, Route("image")] // No auth
		public async Task GetPrimaryImage()
		{
			//TODO - use list args
			var (resourceID, resourceParentID) = GetResourceIDs();
			var url = await _assetedResources.GetPrimaryImageUrl(config.ResourceType, resourceID, resourceParentID, VerifiedUserContext);
			Response.Redirect(url);
		}

		private (string, string) GetResourceIDs()
		{
			var resourceID = RouteData.Values[config.ResourceIDField].ToString();
			var resourceParentID = config.ResourceParentIDField == null ? null : RouteData.Values[config.ResourceParentIDField].ToString();
			return (resourceID, resourceParentID);
		}
	}


}
