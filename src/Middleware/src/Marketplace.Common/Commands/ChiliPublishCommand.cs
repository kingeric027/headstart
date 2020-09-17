using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut.Attributes;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Queries;
using Marketplace.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    #region Models
    [SwaggerModel]
    public class ChiliTemplate
    {
        public SuperMarketplaceProduct Product { get; set; }
        public List<ChiliSpec> TemplateSpecs { get; set; } = new List<ChiliSpec>();
        public string ChiliTemplateID { get; set; }
    }

    [SwaggerModel]
    public class ChiliConfig : ICosmosObject
    {
        public string SupplierProductID { get; set; }
        public string ChiliTemplateID { get; set; }
        public string ChiliTemplateName { get; set; }
        public List<string> Specs { get; set; }
        [JsonProperty(PropertyName = "ID")]
        public string id { get; set; }
        public DateTimeOffset timeStamp { get; set; }
        [CosmosPartitionKey, ApiIgnore]
        public string OwnerClientID { get; set; }
        public string BuyerID { get; set; }
        public string CatalogID { get; set; }
    }

    [SwaggerModel]
    public class ChiliSpec : Spec<ChiliSpecXp, SpecOption>
    {
        
    }

    [SwaggerModel]
    public class ChiliSpecXp
    {
        [Required]
        public ChiliSpecUI UI { get; set; } = new ChiliSpecUI();
    }

    [SwaggerModel]
    public class ChiliSpecUI
    {
        public ControlType ControlType { get; set; } = ControlType.Text;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ControlType
    {
        Text, DropDown, Checkbox, Range
    }

    [SwaggerModel]
    public class ChiliSpecOption : SpecOption<ChiliSpecOptionXp>
    {
    }

    [SwaggerModel]
    public class ChiliSpecOptionXp
    {
        public string Description { get; set; }
        public string SpecID { get; set; }
    }

    
    #endregion

    public interface IChiliSpecOptionCommand
    {
        Task<ChiliSpecOption> Get(string specID, string specOptionID);
        Task<ListPage<ChiliSpecOption>> List(string specID, ListArgs<ChiliSpecOption> args);
        Task<ChiliSpecOption> Create(string specID, ChiliSpecOption specOption);
        Task<ChiliSpecOption> Update(string specID, string specOptionID, ChiliSpecOption spec);
        Task Delete(string specID, string specOptionID);
    }

    public class ChiliSpecOptionCommand : IChiliSpecOptionCommand
    {
        private readonly AppSettings _settings;
        private readonly OrderCloudClientConfig _config;
        private VerifiedUserContext _context;
        private readonly IOrderCloudClient _oc;

        public ChiliSpecOptionCommand(AppSettings settings)
        {
            _settings = settings;
            _config = new OrderCloudClientConfig
            {
                AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                ClientId = _settings.ChiliPublishSettings.ClientId,
                ClientSecret = _settings.ChiliPublishSettings.ClientSecret,
                GrantType = GrantType.ClientCredentials,
                Roles = new[]
                {
                    ApiRole.ProductAdmin
                }
            };
            _oc = new OrderCloudClient(_config);
        }

        public async Task<ChiliSpecOption> Get(string specID, string specOptionID)
        {
            _context = await new VerifiedUserContext().Define(_config);
            return await _oc.Specs.GetOptionAsync<ChiliSpecOption>(specID, specOptionID, _context.AccessToken);
        }

        public async Task<ListPage<ChiliSpecOption>> List(string specID, ListArgs<ChiliSpecOption> args)
        {
            _context = await new VerifiedUserContext().Define(_config);
            var list = await _oc.Specs.ListOptionsAsync<ChiliSpecOption>(specID,
                page: args.Page,
                pageSize: args.PageSize,
                sortBy: args.SortBy.FirstOrDefault(),
                filters: args.Filters.ToJRaw(),
                search: args.Search,
                searchOn: args.SearchOn,
                accessToken: _context.AccessToken);
            return list;
        }

        public async Task<ChiliSpecOption> Create(string specID, ChiliSpecOption specOption)
        {
            _context = await new VerifiedUserContext().Define(_config);
            var result = await _oc.Specs.CreateOptionAsync<ChiliSpecOption>(specID, specOption, _context.AccessToken);
            return result;
        }

        public async Task<ChiliSpecOption> Update(string specID, string specOptionID, ChiliSpecOption specOption)
        {
            _context = await new VerifiedUserContext().Define(_config);
            return await _oc.Specs.SaveOptionAsync<ChiliSpecOption>(specID, specOptionID, specOption, _context.AccessToken);
        }

        public async Task Delete(string specID, string specOptionID)
        {
            _context = await new VerifiedUserContext().Define(_config);
            await _oc.Specs.DeleteOptionAsync(specID, specOptionID, _context.AccessToken);
        }

    }

    public interface IChiliSpecCommand
    {
        Task<ChiliSpec> Get(string specID);
        Task<ListPage<ChiliSpec>> List(ListArgs<ChiliSpec> args);
        Task<ChiliSpec> Create(ChiliSpec spec);
        Task<ChiliSpec> Update(string specID, ChiliSpec spec);
        Task Delete(string specID);
    }

    public class ChiliSpecCommand : IChiliSpecCommand
    {
        private readonly AppSettings _settings;
        private readonly OrderCloudClientConfig _config;
        private VerifiedUserContext _context;
        private readonly IOrderCloudClient _oc;

        public ChiliSpecCommand(AppSettings settings)
        {
            _settings = settings;
            _config = new OrderCloudClientConfig
            {
                AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                ClientId = _settings.ChiliPublishSettings.ClientId,
                ClientSecret = _settings.ChiliPublishSettings.ClientSecret,
                GrantType = GrantType.ClientCredentials,
                Roles = new[]
                {
                    ApiRole.ProductAdmin
                }
            };
            _oc = new OrderCloudClient(_config);
        }

        public async Task<ChiliSpec> Get(string specID)
        {
            _context = await new VerifiedUserContext().Define(_config);
            return await _oc.Specs.GetAsync<ChiliSpec>(specID, _context.AccessToken);
        }

        public async Task<ListPage<ChiliSpec>> List(ListArgs<ChiliSpec> args)
        {
            _context = await new VerifiedUserContext().Define(_config);
            var list = await _oc.Specs.ListAsync<ChiliSpec>(
                page: args.Page, 
                pageSize: args.PageSize, 
                sortBy: args.SortBy.FirstOrDefault(), 
                filters: args.Filters.ToJRaw(),
                search: args.Search, 
                searchOn: args.SearchOn, 
                accessToken: _context.AccessToken);
            return list;
        }

        public async Task<ChiliSpec> Create(ChiliSpec spec)
        {
            _context = await new VerifiedUserContext().Define(_config);
            var result = await _oc.Specs.CreateAsync<ChiliSpec>(spec, _context.AccessToken);
            return result;
        }

        public async Task<ChiliSpec> Update(string specID, ChiliSpec spec)
        {
            _context = await new VerifiedUserContext().Define(_config);
            return await _oc.Specs.SaveAsync<ChiliSpec>(specID, spec, _context.AccessToken);
        }

        public async Task Delete(string specID)
        {
            _context = await new VerifiedUserContext().Define(_config);
            await _oc.Specs.DeleteAsync(specID, _context.AccessToken);
        }
    }

    public interface IChiliConfigCommand
    {
        Task<ChiliConfig> Get(string configID);
        Task<ListPage<ChiliConfig>> List(ListArgs<ChiliConfig> args);
        Task<ChiliConfig> Save(ChiliConfig config);
        Task Delete(string configID);
    }

    public class ChiliConfigCommand : IChiliConfigCommand
    {
        private readonly AppSettings _settings;
        private readonly ChiliPublishConfigQuery _query;
        private readonly OrderCloudClientConfig _config;
        private VerifiedUserContext _context;

        public ChiliConfigCommand(AppSettings settings, ChiliPublishConfigQuery query)
        {
            _settings = settings;
            _query = query;
            _config = new OrderCloudClientConfig
            {
                AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                ClientId = _settings.ChiliPublishSettings.ClientId,
                ClientSecret = _settings.ChiliPublishSettings.ClientSecret,
                GrantType = GrantType.ClientCredentials,
                Roles = new[]
                {
                    ApiRole.ProductAdmin
                }
            };
        }

        public async Task<ChiliConfig> Get(string configID)
        {
            _context = await new VerifiedUserContext().Define(_config);
            return await _query.Get(configID, _context.ClientID);
        }

        public async Task<ListPage<ChiliConfig>> List(ListArgs<ChiliConfig> args)
        {
            _context = await new VerifiedUserContext().Define(_config);
            return await _query.List(args, _context.ClientID);
        }

        public async Task<ChiliConfig> Save(ChiliConfig config)
        {
            _context = await new VerifiedUserContext().Define(_config);
            config.OwnerClientID = _context.ClientID;
            return await _query.Save(config, _context.ClientID);
        }

        public async Task Delete(string configID)
        {
            _context = await new VerifiedUserContext().Define(_config);
            await _query.Delete(configID, _context.ClientID);
        }
    }

    public interface IChiliTemplateCommand
    {
        Task<ChiliTemplate> Get(string templateID, VerifiedUserContext user);
    }

    public class ChiliTemplateCommand : IChiliTemplateCommand
    {
        private readonly AppSettings _settings;
        private readonly OrderCloudClientConfig _config;
        private readonly IOrderCloudClient _oc;
        private readonly IMarketplaceProductCommand _product;
        private readonly ChiliPublishConfigQuery _query;
        private VerifiedUserContext _context;

        public ChiliTemplateCommand(AppSettings settings, IMarketplaceProductCommand product, ChiliPublishConfigQuery query)
        {
            _settings = settings;
            _product = product;
            _query = query;
            _config = new OrderCloudClientConfig
            {
                AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                ClientId = _settings.ChiliPublishSettings.ClientId,
                ClientSecret = _settings.ChiliPublishSettings.ClientSecret,
                GrantType = GrantType.ClientCredentials,
                Roles = new[]
                {
                    ApiRole.ProductAdmin,
                    ApiRole.ProductReader,
                    ApiRole.PriceScheduleReader,
                    ApiRole.PriceScheduleAdmin
                }
            };
            _oc = new OrderCloudClient(_config);
        }


        public async Task<ChiliTemplate> Get(string templateID, VerifiedUserContext user)
        {
            _context = await new VerifiedUserContext().Define(_config);
            var template = await _query.Get(templateID, _context.ClientID);
            var product = await _product.Get(template.SupplierProductID, _context);
            var templateSpecs = await Throttler.RunAsync(template.Specs, 100, 30, s => _oc.Specs.GetAsync<ChiliSpec>(s, _context.AccessToken));
            
            var result = new ChiliTemplate()
            {     
                ChiliTemplateID = template.ChiliTemplateID,
                Product = product,
                TemplateSpecs = templateSpecs.ToList()
            };
            return result;
        }
    }
}
