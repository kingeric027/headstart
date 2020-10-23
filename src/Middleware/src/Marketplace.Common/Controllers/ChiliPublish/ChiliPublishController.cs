using System;
using System.Collections.Generic;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using ordercloud.integrations.tecra;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Common.Commands;
using ordercloud.integrations.tecra.Models;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Integration\" represents a Chili Template Spec Option")]
    [MarketplaceSection.Content(ListOrder = 3)]
    [Route("chili/specs/{specID}/options")]
    public class ChiliSpecOptionsController : BaseController
    {
        private readonly IChiliSpecOptionCommand _command;
        private readonly AppSettings _settings;

        public ChiliSpecOptionsController(AppSettings settings, IChiliSpecOptionCommand command) : base(settings)
        {
            _command = command;
            _settings = settings;
        }

        [DocName("Get a Chili Spec Option")]
        [HttpGet, Route("{specOptionID}"), OrderCloudIntegrationsAuth]
        public async Task<ChiliSpecOption> Get(string specID, string specOptionID)
        {
            return await _command.Get(specID, specOptionID);
        }

        [DocName("List Chili Spec Options")]
        [HttpGet, OrderCloudIntegrationsAuth]
        public async Task<ListPage<ChiliSpecOption>> List(string specID, ListArgs<ChiliSpecOption> args)
        {
            return await _command.List(specID, args);
        }

        [DocName("Create a Chili Spec Option")]
        [DocIgnore]
        [HttpPost, Route(""), OrderCloudIntegrationsAuth]
        public async Task<ChiliSpecOption> Create(string specID, [FromBody] ChiliSpecOption specOption)
        {
            return await _command.Create(specID, specOption);
        }

        [DocName("Update a Chili Spec Option")]
        [HttpPut, Route("{specOptionID}"), OrderCloudIntegrationsAuth]
        public async Task<ChiliSpecOption> Update(string specID, string specOptionID, [FromBody] ChiliSpecOption spec)
        {
            return await _command.Update(specID, specOptionID, spec);
        }

        [DocName("Delete a Chili Spec")]
        [HttpDelete, Route("{specOptionID}"), OrderCloudIntegrationsAuth]
        public async Task Delete(string specID, string specOptionID)
        {
            await _command.Delete(specID, specOptionID);
        }
    }

    [DocComments("\"Integration\" represents a Chili Template Spec")]
    [MarketplaceSection.Content(ListOrder = 3)]
    [Route("chili/specs")]
    public class ChiliSpecController : BaseController
    {
        private readonly IChiliSpecCommand _command;
        private readonly AppSettings _settings;
        public ChiliSpecController(AppSettings settings, IChiliSpecCommand command) : base(settings)
        {
            _command = command;
            _settings = settings;
        }

        [DocName("Get a Chili Spec")]
        [HttpGet, Route("{specID}"), OrderCloudIntegrationsAuth]
        public async Task<ChiliSpec> Get(string specID)
        {
            return await _command.Get(specID);
        }

        [DocName("List Chili Specs")]
        [HttpGet, OrderCloudIntegrationsAuth]
        public async Task<ListPage<ChiliSpec>> List(ListArgs<ChiliSpec> args)
        {
            return await _command.List(args);
        }

        [DocName("Create a Chili Spec")]
        [DocIgnore]
        [HttpPost, Route(""), OrderCloudIntegrationsAuth]
        public async Task<ChiliSpec> Create([FromBody] ChiliSpec spec)
        {
            return await _command.Create(spec);
        }

        [DocName("Update a Chili Spec")]
        [HttpPut, Route("{specID}"), OrderCloudIntegrationsAuth]
        public async Task<ChiliSpec> Update(string specID, [FromBody] ChiliSpec spec)
        {
            return await _command.Update(specID, spec);
        }

        [DocName("Delete a Chili Spec")]
        [HttpDelete, Route("{specID}"), OrderCloudIntegrationsAuth]
        public async Task Delete(string specID)
        {
            await _command.Delete(specID);
        }
    }

    [DocComments("\"Integration\" represents a Chili Template Product")]
    [MarketplaceSection.Content(ListOrder = 3)]
    [Route("chili/template")]
    public class ChiliTemplateController : BaseController
    {
        private readonly IChiliTemplateCommand _command;
        private readonly AppSettings _settings;
        public ChiliTemplateController(AppSettings settings, IChiliTemplateCommand command) : base(settings)
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

    [DocComments("\"Integration\" represents Chili Template Configurations")]
    [MarketplaceSection.Content(ListOrder = 3)]
    [Route("chili/config")]
    public class ChiliConfigController : BaseController
    {
        private readonly IChiliConfigCommand _command;

        public ChiliConfigController(AppSettings settings, IChiliConfigCommand command) : base(settings)
        {
            _command = command;
        }

        [DocName("Get a Chili Assignment")]
        [HttpGet, Route("{configID}"), OrderCloudIntegrationsAuth]
        public async Task<ChiliConfig> Get(string configID)
        {
            return await _command.Get(configID);
        }

        [DocName("List Chili Assignment")]
        [HttpGet, Route(""), OrderCloudIntegrationsAuth]
        public async Task<ListPage<ChiliConfig>> List(ListArgs<ChiliConfig> args)
        {
            return await _command.List(args);
        }

		[DocName("Create a Chili Assignment")]
        [DocIgnore]
        [HttpPost, Route(""), OrderCloudIntegrationsAuth]
        public async Task<ChiliConfig> Create([FromBody] ChiliConfig config)
        {
            return await _command.Save(config);
        }

        [DocName("Update a Chili Assignment")]
        [HttpPut, Route(""), OrderCloudIntegrationsAuth]
        public async Task<ChiliConfig> Update([FromBody] ChiliConfig config)
        {
            return await _command.Save(config);
        }

        [DocName("Delete a Chili Assignment")]
        [HttpDelete, Route("{configID}"), OrderCloudIntegrationsAuth]
        public async Task Delete(string configID)
        {
            await _command.Delete(configID);
        }
	}

    [DocComments("\"Integration\" represents an array of Tecra Documents")]
    [MarketplaceSection.Content(ListOrder = 4)]
    [Route("tecra/documents"), OrderCloudIntegrationsAuth]
    public class TecraDocumentController : BaseController
    {
        private readonly IOrderCloudIntegrationsTecraCommand _tecra;
        private readonly AppSettings _settings;
        public TecraDocumentController(AppSettings settings, IOrderCloudIntegrationsTecraCommand tecra) : base(settings)
        {
            _settings = settings;
            _tecra = tecra;
        }

        [DocName("Get Tecra Documents")]
        [HttpGet, Route("")]
        public async Task<IEnumerable<TecraDocument>> Get()
        {
            var result = await _tecra.TecraDocuments();
            return result;
        }

        [DocName("Get Tecra Documents By Folder")]
        [HttpGet, Route("byfolder/{folder}")]
        public async Task<IEnumerable<TecraDocument>> GetByFolder(string folder)
        {
            var result = await _tecra.TecraDocumentsByFolder(folder);
            return result;
        }
    }

    [DocComments("\"Integration\" represents an array of Tecra Specs")]
    [MarketplaceSection.Content(ListOrder = 5)]
    [Route("tecra/specs"), OrderCloudIntegrationsAuth]
    public class TecraSpecsController : BaseController
    {
        private readonly IOrderCloudIntegrationsTecraCommand _tecra;
        private readonly AppSettings _settings;
        public TecraSpecsController(AppSettings settings, IOrderCloudIntegrationsTecraCommand tecra) : base(settings)
        {
            _settings = settings;
            _tecra = tecra;
        }

        [DocName("Get Tecra Specs")]
        [HttpGet, Route("")]
        public async Task<IEnumerable<TecraSpec>> Get(string id)
        {
            var result = await _tecra.TecraSpecs(id);
            return result;
        }
    }

    [DocComments("\"Integration\" represents a URL to a Tecra Frame")]
    [MarketplaceSection.Content(ListOrder = 6)]
    [Route("tecra/frame"), OrderCloudIntegrationsAuth]
    public class TecraFrameController : BaseController
    {
        private readonly IOrderCloudIntegrationsTecraCommand _tecra;
        private readonly AppSettings _settings;
        public TecraFrameController(AppSettings settings, IOrderCloudIntegrationsTecraCommand tecra) : base(settings)
        {
            _settings = settings;
            _tecra = tecra;
        }

        [DocName("Get Tecra Frame")]
        [HttpGet, Route("")]
        public async Task<string> Get(string id)
        {
            var result = await _tecra.TecraFrame(id);
            return result;
        }
    }

    [DocComments("\"Integration\" represents a URL to a Tecra Proof")]
    [MarketplaceSection.Content(ListOrder = 7)]
    [Route("tecra/proof"), OrderCloudIntegrationsAuth]
    public class TecraProofController : BaseController
    {
        private readonly IOrderCloudIntegrationsTecraCommand _tecra;
        private readonly AppSettings _settings;
        public TecraProofController(AppSettings settings, IOrderCloudIntegrationsTecraCommand tecra) : base(settings)
        {
            _settings = settings;
            _tecra = tecra;
        }

        [DocName("Get Tecra Proof")]
        [HttpGet, Route("")]
        public async Task<string> Get(string id)
        {
            var result = await _tecra.TecraProof(id);
            return result;
        }
    }

    [DocComments("\"Integration\" represents a URL to a Tecra PDF")]
    [MarketplaceSection.Content(ListOrder = 8)]
    [Route("tecra/pdf"), OrderCloudIntegrationsAuth]
    public class TecraPDFController : BaseController
    {
        private readonly IOrderCloudIntegrationsTecraCommand _tecra;
        private readonly AppSettings _settings;
        public TecraPDFController(AppSettings settings, IOrderCloudIntegrationsTecraCommand tecra) : base(settings)
        {
            _settings = settings;
            _tecra = tecra;
        }

        [DocName("Get Tecra PDF")]
        [HttpGet, Route("")]
        public async Task<string> Get(string id)
        {
            var result = await _tecra.TecraPDF(id);
            return result;
        }
    }
}
