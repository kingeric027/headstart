using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Common;
using Marketplace.Common.Controllers;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Controllers
{
	[DocComments("\"Integration\" represents Assets")]
	[MarketplaceSection.Integration(ListOrder = 6)]
	[Route("containers")]
	public class AssetController : BaseController
	{
		private readonly IAssetQuery _query;

		public AssetController(AppSettings settings, IAssetQuery query) : base(settings)
		{
			_query = query;
		}

		[DocName("List Assets")]
		[HttpGet, Route("{containerID}/assets")]
		public async Task<ListPage<Asset>> List(string containerID, ListArgs<Asset> args)
		{
			return await _query.List(containerID, args);
		}

		[DocName("Get an Asset ")]
		[HttpGet, Route("{containerID}/assets/{assetID}")]
		public async Task<Asset> Get(string containerID, string assetID)
		{
			return await _query.Get(containerID, assetID);
		}

		[DocName("Upoload an Asset")]
		[HttpPost, Route("{containerID}/assets")]
		public async Task<Asset> Create(string containerID, [FromForm] AssetUploadForm form)
		{
			return await _query.Create(containerID, form);
		}

		[DocName("Create or Update an Asset")]
		[HttpPut, Route("{containerID}/assets/{assetID}")]
		public async Task<Asset> Update(string containerID, string assetID, [FromBody] Asset asset)
		{
			return await _query.Update(containerID, assetID, asset);
		}

		[DocName("Delete an Asset Container")]
		[HttpDelete, Route("{containerID}/assets/{assetID}")]
		public async Task Delete(string containerID, string assetID)
		{
			await _query.Delete(containerID, assetID);
		}
	}
}
