using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.cms;
using IDocumentQuery = ordercloud.integrations.cms.IDocumentQuery;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Integration\" represents Chili Template Configurations")]
    [MarketplaceSection.Content(ListOrder = 3)]
    [Route("print/{schemaID}/chilitemplate")]
    public class ChiliTemplateController : BaseController
    {
        private readonly IDocumentQuery _documents;

        public ChiliTemplateController(AppSettings settings, IDocumentQuery schemas) : base(settings)
        {
            _documents = schemas;
        }

        [DocName("Get a Chili Template")]
        [HttpGet, Route("{documentID}"), OrderCloudIntegrationsAuth]
        public async Task<Document> Get(string schemaID, string documentID)
        {
            return await _documents.Get(schemaID, documentID, VerifiedUserContext);
        }

        [DocName("List Chili Template")]
        [HttpGet, Route(""), OrderCloudIntegrationsAuth]
        public async Task<ListPage<Document>> List(string schemaID, ListArgs<Document> args)
        {
            return await _documents.List(schemaID, args, VerifiedUserContext);
        }

		[DocName("Create a Chili Template Configuration")]
        [DocIgnore] // For now, hide from swagger reflection b/c it doesn't handle file uploads well. 
        [HttpPost, Route(""), OrderCloudIntegrationsAuth]
        public async Task<Document> Create(string schemaID, [FromBody] Document document)
        {
            return await _documents.Create(schemaID, document, VerifiedUserContext);
        }

        [DocName("Update a Document")]
        [HttpPut, Route("{documentID}"), OrderCloudIntegrationsAuth]
        public async Task<Document> Update(string schemaID, string documentID, [FromBody] Document document)
        {
            return await _documents.Update(schemaID, documentID, document, VerifiedUserContext);
        }

        [DocName("Delete a Document")]
        [HttpDelete, Route("{documentID}"), OrderCloudIntegrationsAuth]
        public async Task Delete(string schemaID, string documentID)
        {
            await _documents.Delete(schemaID, documentID, VerifiedUserContext);
        }
	}
}
