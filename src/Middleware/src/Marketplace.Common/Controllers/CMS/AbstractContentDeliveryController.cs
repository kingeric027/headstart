using Integrations.CMS.Models;
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
		public string ResourceParentIDField { get; set; } = null;
	}

	public abstract class AbstractContentDeliveryController : BaseController
	{
		private readonly IAssetedResourceQuery _assetedResources;

		protected abstract ContentControllerConfig config { get; set; }

		public AbstractContentDeliveryController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings)
		{
			_assetedResources = assetedResources;
		}

		// TODO - add list page and list args
		[HttpGet, Route("{resourceID}/assets"), MarketplaceUserAuth]
		public async Task<List<AssetForDelivery>> ListAssets(string resourceID)
		{
			//TODO - use list args
			var resourceParentID = GetResourceParentID();
			return await _assetedResources.DeliverAssets(config.ResourceType, resourceID, resourceParentID, VerifiedUserContext);
		}

		[HttpGet, Route("{resourceID}/image")] // No auth
		public async Task GetFirstImage(string resourceID)
		{
			var resourceParentID = GetResourceParentID();
			var url = await _assetedResources.DeliverFirstImageUrl(config.ResourceType, resourceID, resourceParentID, VerifiedUserContext);
			Response.Redirect(url);
		}

		private string GetResourceParentID()
		{
			var resourceParentID = config.ResourceParentIDField == null ? null : RouteData.Values[config.ResourceParentIDField].ToString();
			return resourceParentID;
		}
	}


}
