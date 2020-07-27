using System.Collections.Generic;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Common.Controllers.ChiliPublish;
using Marketplace.Models;
using ordercloud.integrations.cms;
using IDocumentQuery = ordercloud.integrations.cms.IDocumentQuery;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Integration\" represents a Chili Template Product")]
    [MarketplaceSection.Content(ListOrder = 3)]
    [Route("print/chilitemplate")]
    public class ChiliTemplateController : BaseController
    {
        private readonly IChiliPublishCommand _command;
        private readonly AppSettings _settings;
        public ChiliTemplateController(AppSettings settings, IChiliPublishCommand command) : base(settings)
        {
            _command = command;
            _settings = settings;
        }

        [DocName("Get a Chili Template")]
        [HttpGet, Route("{templateID}"), OrderCloudIntegrationsAuth]
        public async Task<ChiliTemplate> Get(string templateID)
        {
            var result = await _command.Get(templateID, this.VerifiedUserContext);
            return result;
        }
    }

    public class ChiliTemplate
    {
        public SuperMarketplaceProduct Product { get; set; }
        public List<MarketplaceSpec> Specs { get; set; } = new List<MarketplaceSpec>();
        public string ChiliTemplateID { get; set; }
    }

    public class ChiliConfig
    {
        public string SupplierProductID { get; set; }
        public string ChiliTemplateID { get; set; }
        public List<string> Specs { get; set; }
    } 

    [DocComments("\"Integration\" represents Chili Template Configurations")]
    [MarketplaceSection.Content(ListOrder = 3)]
    [Route("print/{schemaID}/chiliassignment")]
    public class ChiliConfigController : BaseController
    {
        private readonly IDocumentQuery _documents;

        public ChiliConfigController(AppSettings settings, IDocumentQuery schemas) : base(settings)
        {
            _documents = schemas;
        }

        [DocName("Get a Chili Assignment")]
        [HttpGet, Route("{documentID}"), OrderCloudIntegrationsAuth]
        public async Task<Document> Get(string schemaID, string documentID)
        {
            return await _documents.Get(schemaID, documentID, VerifiedUserContext);
        }

        [DocName("List Chili Assignment")]
        [HttpGet, Route(""), OrderCloudIntegrationsAuth]
        public async Task<ListPage<Document>> List(string schemaID, ListArgs<Document> args)
        {
            return await _documents.List(schemaID, args, VerifiedUserContext);
        }

		[DocName("Create a Chili Assignment")]
        [DocIgnore] // For now, hide from swagger reflection b/c it doesn't handle file uploads well. 
        [HttpPost, Route(""), OrderCloudIntegrationsAuth]
        public async Task<Document> Create(string schemaID, [FromBody] Document document)
        {
            return await _documents.Create(schemaID, document, VerifiedUserContext);
        }

        [DocName("Update a Chili Assignment")]
        [HttpPut, Route("{documentID}"), OrderCloudIntegrationsAuth]
        public async Task<Document> Update(string schemaID, string documentID, [FromBody] Document document)
        {
            return await _documents.Update(schemaID, documentID, document, VerifiedUserContext);
        }

        [DocName("Delete a Chili Assignment")]
        [HttpDelete, Route("{documentID}"), OrderCloudIntegrationsAuth]
        public async Task Delete(string schemaID, string documentID)
        {
            await _documents.Delete(schemaID, documentID, VerifiedUserContext);
        }
	}
}
