using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;
using Marketplace.Common.Commands.Crud;

namespace Marketplace.Common.Controllers
{
	[DocComments("\"Marketplace Catalogs\" for product groupings and visibility in Marketplace")]
	[MarketplaceSection.Marketplace(ListOrder = 2)]
	[Route("buyers")]
	public class CatalogController : BaseController
	{

		private readonly IMarketplaceCatalogCommand _command;
		public CatalogController(IMarketplaceCatalogCommand command, AppSettings settings) : base(settings)
		{
			_command = command;
		}

		[DocName("GET a list of Catalogs")]
		[HttpGet, Route("{buyerID}/catalogs"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
		public async Task<ListPage<MarketplaceCatalog>> List(ListArgs<MarketplaceCatalog> args, string buyerID)
		{
			return await _command.List(buyerID, args, VerifiedUserContext);
		}

		[DocName("GET a single Catalog")]
		[HttpGet, Route("{buyerID}/catalogs/{catalogID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
		public async Task<MarketplaceCatalog> Get(string buyerID, string catalogID)
		{
			return await _command.Get(buyerID, catalogID, VerifiedUserContext);
		}

		[DocName("Create a new Catalog")]
		[HttpPost, Route("{buyerID}/catalogs"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<MarketplaceCatalog> Post([FromBody] MarketplaceCatalog obj, string buyerID)
		{
			return await _command.Post(buyerID, obj, VerifiedUserContext);
		}

		[DocName("Get a list of catalog location assignments")]
		[HttpPost, Route("{buyerID}/catalogs/assignments"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<ListPage<MarketplaceCatalogAssignment>> GetAssignments(string buyerID, [FromQuery(Name = "catalogID")] string catalogID = "", [FromQuery(Name = "locationID")] string locationID = "")
		{
			return await _command.GetAssignments(buyerID, locationID, VerifiedUserContext);
		}

		[DocName("Set catalog assignments for a location")]
		[HttpPost, Route("{buyerID}/{locationID}/catalogs/assignments"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin)]
		public async Task SetAssignments(string buyerID, string locationID, [FromBody] MarketplaceCatalogAssignmentRequest assignmentRequest)
		{
			await _command.SetAssignments(buyerID, locationID, assignmentRequest.CatalogIDs, VerifiedUserContext);
		}

		[DocName("PUT Catalog")]
		[HttpPut, Route("{buyerID}/catalogs/{catalogID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<MarketplaceCatalog> Put([FromBody] MarketplaceCatalog obj, string buyerID, string catalogID)
		{
			return await _command.Put(buyerID, catalogID, obj, this.VerifiedUserContext);
		}

		[DocName("DELETE Catalog")]
		[HttpDelete, Route("{buyerID}/catalogs/{catalogID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task Delete(string buyerID, string catalogID)
		{
			await _command.Delete(buyerID, catalogID, VerifiedUserContext);
		}

		[DocName("SYNC User Location Catalogs On Add To Location")]
		[HttpPost, Route("{buyerID}/catalogs/user/{userID}/location/{locationID}/Add"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task SyncOnAddToLocation(string buyerID, string userID, string locationID)
		{
			await _command.SyncUserCatalogAssignmentsForUserOnAddToLocation(buyerID, locationID, userID);
		}

		[DocName("SYNC User Location Catalogs On Remove From Location")]
		[HttpPost, Route("{buyerID}/catalogs/user/{userID}/location/{locationID}/Remove"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task SyncOnRemoveFromLocation(string buyerID, string userID, string locationID)
		{
			await _command.SyncUserCatalogAssignmentsForUserOnRemoveFrom(buyerID, locationID, userID);
		}
	}
}
