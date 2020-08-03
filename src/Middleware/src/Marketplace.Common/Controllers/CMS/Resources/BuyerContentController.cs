using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Controllers.CMS.Resources
{
	[MarketplaceSection.Content(ListOrder =1)]
	[Route("buyers/{buyerID}")]
	public class BuyerContentController: BaseController
	{
		private readonly IAssetedResourceQuery _assetedResources;
		private readonly IDocumentAssignmentQuery _documentAssignments;

		private ResourceType type { get; } = ResourceType.Buyers;

		public BuyerContentController(AppSettings settings, IAssetedResourceQuery assetedResources, IDocumentAssignmentQuery documentAssignments) : base(settings)
		{
			_assetedResources = assetedResources;
			_documentAssignments = documentAssignments;
		}

		// Content Delivery

		// TODO - add list page and list args
		[DocName("Get Assets Assigned to Resource")]
		[HttpGet, Route("assets"), OrderCloudIntegrationsAuth]
		public async Task<ListPage<AssetForDelivery>> ListAssets(string buyerID, ListArgs<AssetForDelivery> args)
		{
			var resource = new Resource(type, buyerID);
			return new ListPage<AssetForDelivery>
			{
				Items = await _assetedResources.ListAssets(resource, VerifiedUserContext)
			};
		}

		[DocName("Get Resource's primary image")]
		[HttpGet, Route("image")] // No auth
		public async Task GetFirstImage(string buyerID)
		{
			var resource = new Resource(type, buyerID);
			var url = await _assetedResources.GetFirstImage(resource, VerifiedUserContext);
			Response.Redirect(url);
		}

		// Content Admin
		[DocName("Assign Asset to Resource")]
		[HttpPost, Route("assets/{assetID}/assignments"), OrderCloudIntegrationsAuth]
		public async Task SaveAssetAssignment(string buyerID, string assetID)
		{
			var resource = new Resource(type, buyerID);
			await _assetedResources.SaveAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Remove Asset from Resource"), OrderCloudIntegrationsAuth]
		[HttpDelete, Route("assets/{assetID}/assignments")]
		public async Task DeleteAssetAssignment(string buyerID, string assetID)
		{
			var resource = new Resource(type, buyerID);
			await _assetedResources.DeleteAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), OrderCloudIntegrationsAuth]
		[HttpPost, Route("assets/{assetID}/assignments/moveto/{listOrderWithinType}")]
		public async Task MoveAssetAssignment(string buyerID, string assetID, int listOrderWithinType)
		{
			var resource = new Resource(type, buyerID);
			await _assetedResources.MoveAssignment(resource, assetID, listOrderWithinType, VerifiedUserContext);
		}

		[DocName("Get Documents Assigned to Resource"), OrderCloudIntegrationsAuth]
		[HttpGet, Route("schemas/{schemaID}/documents")]
		public async Task<ListPage<Document<JObject>>> ListDocuments(string buyerID, string schemaID, ListArgs<Document<JObject>> args)
		{
			var resource = new Resource(type, buyerID);
			return await _documentAssignments.ListDocuments<JObject>(schemaID, resource, args, VerifiedUserContext);
		}

		[DocName("Assign Document to Resource"), OrderCloudIntegrationsAuth]
		[HttpPost, Route("schemas/{schemaID}/documents/{documentID}/assignments")]
		public async Task SaveDocumentAssignment(string buyerID, string schemaID, string documentID)
		{
			var resource = new Resource(type, buyerID);
			await _documentAssignments.SaveAssignment<JObject>(schemaID, documentID, resource, VerifiedUserContext);
		}

		[DocName("Remove Document from Resource"), OrderCloudIntegrationsAuth]
		[HttpDelete, Route("schemas/{schemaID}/documents/{documentID}/assignments")]
		public async Task DeleteDocumentAssignment(string buyerID, string schemaID, string documentID)
		{
			var resource = new Resource(type, buyerID);
			await _documentAssignments.DeleteAssignment<JObject>(schemaID, documentID, resource, VerifiedUserContext);
		}
	}
}
