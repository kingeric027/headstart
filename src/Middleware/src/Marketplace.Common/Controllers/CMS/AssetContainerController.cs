using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Common;
using Marketplace.Common.Controllers;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Marketplace.Models.Attributes;
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
	[DocComments("\"Integration\" represents asset containers")]
	[MarketplaceSection.Integration(ListOrder = 6)]
	[Route("containers")]
	public class AssetContainerController : BaseController
	{
		private readonly IAssetContainerQuery _query;

		public AssetContainerController(AppSettings settings, IAssetContainerQuery query) : base(settings)
		{
			_query = query;
		}

		[DocName("List Asset Containers")]
		[HttpGet, Route(""), MarketplaceUserAuth]
		public async Task<ListPage<AssetContainer>> List(ListArgs<AssetContainer> args)
		{
			return await _query.List(args);
		}

		[DocName("Get an Asset Container")]
		[HttpGet, Route("{containerID}"), MarketplaceUserAuth]
		public async Task<AssetContainer> Get(string containerID)
		{
			return await _query.Get(containerID);
		}

		[DocName("Create an Asset Container")]
		[HttpPost, Route(""), MarketplaceUserAuth]
		public async Task<AssetContainer> Create([FromBody] AssetContainer container)
		{
			return await _query.Create(container);
		}

		[DocName("Update an Asset Container")]
		[HttpPut, Route("{containerID}"), MarketplaceUserAuth]
		public async Task<AssetContainer> Update(string containerID, [FromBody] AssetContainer container)
		{
			return await _query.Update(containerID, container);
		}

		[DocName("Delete an Asset Container")]
		[HttpDelete, Route("{containerID}"), MarketplaceUserAuth]
		public async Task Delete(string containerID)
		{
			await _query.Delete(containerID);
		}
	}
}
