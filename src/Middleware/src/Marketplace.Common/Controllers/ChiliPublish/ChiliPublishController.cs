using System;
using System.Collections.Generic;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Common.Commands;

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
        [HttpPut, Route("{documentID}"), OrderCloudIntegrationsAuth]
        public async Task<ChiliConfig> Update([FromBody] ChiliConfig config)
        {
            return await _command.Save(config);
        }

        [DocName("Delete a Chili Assignment")]
        [HttpDelete, Route("{documentID}"), OrderCloudIntegrationsAuth]
        public async Task Delete(string configID)
        {
            await _command.Delete(configID);
        }
	}
}
