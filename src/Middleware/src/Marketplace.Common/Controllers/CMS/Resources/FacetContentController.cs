using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;

namespace Marketplace.Common.Controllers.CMS.Resources
{
	[MarketplaceSection.Content(ListOrder = 1)]
	[Route("productFacets/{productFacetID}")]
	public class FacetContentController : BaseController
	{
		private readonly IAssetedResourceQuery _assetedResources;
		private readonly IDocumentAssignmentQuery _documentAssignments;

		private ResourceType type { get; } = ResourceType.ProductFacets;

		public FacetContentController(AppSettings settings, IAssetedResourceQuery assetedResources, IDocumentAssignmentQuery documentAssignments) : base(settings)
		{
			_assetedResources = assetedResources; 
			_documentAssignments = documentAssignments;
		}

		// Content Delivery

		// TODO - add list page and list args
		[DocName("Get Assets Assigned to Resource")]
		[HttpGet, Route("assets"), OrderCloudIntegrationsAuth]
		public async Task<ListPage<AssetForDelivery>> ListAssets(string productFacetID, ListArgs<AssetForDelivery> args)
		{
			var resource = new Resource(type, productFacetID);
			return new ListPage<AssetForDelivery>
			{
				Items = await _assetedResources.ListAssets(resource, VerifiedUserContext)
			};
		}

		[DocName("Get Resource's primary image")]
		[HttpGet, Route("image")] // No auth
		public async Task GetFirstImage(string productFacetID)
		{
			var resource = new Resource(type, productFacetID);
			var url = await _assetedResources.GetFirstImage(resource, VerifiedUserContext);
			Response.Redirect(url);
		}

		// Content Admin
		[DocName("Assign Asset to Resource")]
		[HttpPost, Route("assets/{assetID}/assignments"), OrderCloudIntegrationsAuth]
		public async Task SaveAssetAssignment(string productFacetID, string assetID)
		{
			var resource = new Resource(type, productFacetID);
			await _assetedResources.SaveAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Remove Asset from Resource"), OrderCloudIntegrationsAuth]
		[HttpDelete, Route("assets/{assetID}/assignments")]
		public async Task DeleteAssetAssignment(string productFacetID, string assetID)
		{
			var resource = new Resource(type, productFacetID);
			await _assetedResources.DeleteAssignment(resource, assetID, VerifiedUserContext);
		}

		[DocName("Reorder Asset Assignment"), OrderCloudIntegrationsAuth]
		[HttpPost, Route("assets/{assetID}/assignments/moveto/{listOrderWithinType}")]
		public async Task MoveAssetAssignment(string productFacetID, string assetID, int listOrderWithinType)
		{
			var resource = new Resource(type, productFacetID);
			await _assetedResources.MoveAssignment(resource, assetID, listOrderWithinType, VerifiedUserContext);
		}

		[DocName("Get Documents Assigned to Resource"), OrderCloudIntegrationsAuth]
		[HttpGet, Route("schemas/{schemaID}/documents")]
		public async Task<ListPage<Document>> ListDocuments(string productFacetID, string schemaID, ListArgs<DocumentAssignment> args)
		{
			var resource = new Resource(type, productFacetID);
			return await _documentAssignments.ListDocuments(schemaID, resource, args, VerifiedUserContext);
		}
	}
}
