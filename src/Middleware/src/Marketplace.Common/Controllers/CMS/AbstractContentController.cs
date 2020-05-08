using Integrations.CMS.Models;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers.CMS
{
	public abstract class AbstractContentController : BaseController
	{
		private readonly IAssetedResourceQuery _assetedResources;

		protected abstract Resource ContentConfig { get; set; }

		public AbstractContentController(AppSettings settings, IAssetedResourceQuery assetedResources) : base(settings)
		{
			_assetedResources = assetedResources;
		}

		// Content Delivery

		// TODO - add list page and list args
		[DocName("Get Assets Assigned to Resource")]
		[HttpGet, Route("{resourceID}/assets"), MarketplaceUserAuth]
		public async Task<List<AssetForDelivery>> ListAssets(string resourceID)
		{
			return await _assetedResources.ListAssets(GetResource(resourceID), VerifiedUserContext);
		}

		[DocName("Get Resource's primary image")]
		[HttpGet, Route("{resourceID}/image")] // No auth
		public async Task GetFirstImage(string resourceID)
		{
			var url = await _assetedResources.GetFirstImage(GetResource(resourceID), VerifiedUserContext);
			Response.Redirect(url);
		}

		// Content Admin
		[DocName("Assign Asset to Resource")]
		[HttpPost, Route("{resourceID}/assets/{assetID}/assignments"), MarketplaceUserAuth]
		public async Task SaveAssignment(string resourceID, string assetID)
		{
			await _assetedResources.SaveAssignment(GetResource(resourceID), assetID, VerifiedUserContext);
		}

		[DocName("Remove Asset from Resource"), MarketplaceUserAuth]
		[HttpDelete, Route("{resourceID}/assets/{assetID}/assignments")]
		public async Task DeleteAssignment(string resourceID, string assetID)
		{
			await _assetedResources.DeleteAssignment(GetResource(resourceID), assetID, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), MarketplaceUserAuth]
		[HttpPost, Route("{resourceID}/assets/{assetID}/assignments/moveto/{listOrderWithinType}")]
		public async Task MoveAssignment(string resourceID, string assetID, int listOrderWithinType)
		{
			await _assetedResources.MoveAssignment(GetResource(resourceID), assetID, listOrderWithinType, VerifiedUserContext);
		}

		private Resource GetResource(string resourceID)
		{
			var parentID = ContentConfig.ParentID == null ? null : RouteData.Values[ContentConfig.ParentID].ToString();
			return new Resource(ContentConfig.Type, resourceID, parentID);
		}
	}


}
