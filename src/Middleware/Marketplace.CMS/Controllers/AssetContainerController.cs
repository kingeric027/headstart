using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Common;
using Marketplace.Common.Controllers;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
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
	[DocComments("\"Integration\" represents Avalara Tax Functionality")]
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
		[HttpGet, Route("")]
		public async Task<ListPage<AssetContainer>> List(ListArgs<AssetContainer> args)
		{
			return await _query.List(args);
		}

		[DocName("Get an Asset Container")]
		[HttpGet, Route("{containerID}")]
		public async Task<AssetContainer> Get(string containerID)
		{
			return await _query.Get(containerID);
		}

		[DocName("Create an Asset Container")]
		[HttpPost, Route("")]
		public async Task<AssetContainer> Create([FromBody] AssetContainer container)
		{
			return await _query.Create(container);
		}

		[DocName("Create or Update an Asset Container")]
		[HttpPut, Route("{containerID}")]
		public async Task<AssetContainer> CreateOrUpdate(string containerID, [FromBody] AssetContainer container)
		{
			return await _query.CreateOrUpdate(containerID, container);
		}

		[DocName("Delete an Asset Container")]
		[HttpDelete, Route("{containerID}")]
		public async Task Delete(string containerID)
		{
			await _query.Delete(containerID);
		}
	}
}
