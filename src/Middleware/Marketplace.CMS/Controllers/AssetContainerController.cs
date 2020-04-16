using Marketplace.Common;
using Marketplace.Common.Controllers;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.CMS.Controllers
{
	[DocComments("\"Integration\" represents Avalara Tax Functionality")]
	[MarketplaceSection.Integration(ListOrder = 6)]
	[Route("containers")]
	public class AssetContainerController : BaseController
	{
		public AssetContainerController(AppSettings settings) : base(settings)
		{
		}

		[DocName("List Asset Containers")]
		[HttpGet]
		public async Task<object> Get()
		{
			// Testing settings and routes work in different project.
			var publicSettings = new { env = Settings.Env.ToString(), cosmosdb = Settings.CosmosSettings.DatabaseName };
			return await Task.FromResult(publicSettings);
		}
	}
}
