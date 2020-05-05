using Integrations.CMS.Models;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
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

namespace Marketplace.Common.Controllers.CMS
{
	[DocComments("\"Integration\" represents Assets")]
	[MarketplaceSection.Integration(ListOrder = 6)]
	[Route("assets/assignments")]
	public class AssetAssignmentController : BaseController
	{
		private readonly IAssetAssignmentQuery _assignments;

		public AssetAssignmentController(AppSettings settings, IAssetAssignmentQuery assignments) : base(settings)
		{
			_assignments = assignments;
		}

		[DocName("Deliver Assets"), MarketplaceUserAuth]
		[HttpGet, Route("")]
		public async Task<ListPage<Asset>> DeliverAssets(ResourceType resourceType, string resourceID, ListArgs<Asset> args)
		{
			return await _assignments.DeliverAssets(resourceType, resourceID, null, args);
		}

		// Route is available to anyone right now, but if your token does not give access to an OC resource, you will get 401 response.
		[DocName("Create Asset Assignment")]
		[HttpPost, Route(""), MarketplaceUserAuth]
		public async Task SaveAssignment([FromBody] AssetAssignment assignment)
		{
			await _assignments.SaveAssignment(assignment, VerifiedUserContext);
		}

		[DocName("Delete Asset Assignment"), MarketplaceUserAuth]
		[HttpDelete, Route("")]
		public async Task DeleteAssignment([FromBody] AssetAssignment assignment)
		{
			await _assignments.DeleteAssignment(assignment, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), MarketplaceUserAuth]
		[HttpPost, Route("moveTo/{position}")]
		public async Task MoveAssignment([FromBody] AssetAssignment assignment, int position)
		{
			await _assignments.MoveAssignment(assignment, position, VerifiedUserContext);
		}
	}
}
