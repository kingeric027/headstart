﻿using Integrations.CMS.Models;
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
		[HttpGet, Route(""), MarketplaceUserAuth]
		public async Task<ListPage<Asset>> List(ListArgs<Asset> args)
		{
			return await _assets.List(args, VerifiedUserContext);
		}

		[DocName("Get an Asset")]
		[HttpGet, Route("{assetID}"), MarketplaceUserAuth]
		public async Task<Asset> Get(string assetID)
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
	}
}
