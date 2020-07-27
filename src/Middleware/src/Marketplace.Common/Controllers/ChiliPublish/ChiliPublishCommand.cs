using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Crud;
using Marketplace.Models;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers.ChiliPublish
{
    public interface IChiliPublishCommand
    {
        Task<ChiliTemplate> Get(string templateID, VerifiedUserContext user);
    }

    public class ChiliPublishCommand : IChiliPublishCommand
    {
        private const string SCHEMA_ID = "chili_schema";
        private readonly AppSettings _settings;
        private OrderCloudClientConfig _config;
        private readonly IDocumentQuery _document;
        private readonly IOrderCloudClient _oc;
        private readonly IMarketplaceProductCommand _product;
        private readonly IAssetedResourceQuery _assets;
        public ChiliPublishCommand(AppSettings settings, IDocumentQuery document, IMarketplaceProductCommand product, IAssetedResourceQuery assets)
        {
            _settings = settings;
            _document = document;
            _product = product;
            _assets = assets;
            _config = new OrderCloudClientConfig
            {
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                AuthUrl = _settings.OrderCloudSettings.AuthUrl,
                ClientId = "6371ED45-3F9E-40E1-9B1C-5E223AAB3134", //_settings.OrderCloudSettings.ClientID,
                ClientSecret = "ulhq0P2DrdvzjBngQhv3DLus15V3VZEGYG0vYuVtBCRrruDNCpQXl11Sfinb", // _settings.OrderCloudSettings.ClientSecret,
                GrantType = GrantType.ClientCredentials,
                Roles = new[]
                {
                    ApiRole.ProductAdmin
                }
            };
        }

        public async Task<ChiliTemplate> Get(string templateID, VerifiedUserContext user)
        {
            var admin_user = await new VerifiedUserContext().Define(_config);
            var d = await _document.List(SCHEMA_ID, new ListArgs<ChiliTemplate>()
            {
                Filters = new List<ListFilter>()
                {
                    new ListFilter()
                    {
                        Name = "SupplierProductID",
                        Values = new List<ListFilterValue>()
                            {new ListFilterValue() {Operator = ListFilterOperator.Equal, Term = "BusinessCards"}}
                    }
                }
            }, admin_user);
            var doc = await _document.Get(SCHEMA_ID, templateID, admin_user);
            var config = doc.Doc.ToObject<ChiliConfig>();
            var product = await _product.Get(templateID, admin_user);
            var templateSpecs = await Throttler.RunAsync(config.Specs, 100, 30, s => _oc.Specs.GetAsync<MarketplaceSpec>(s));
            
            var result = new ChiliTemplate()
            {     
                ChiliTemplateID = doc.InteropID,
                Product = product,
                Specs = templateSpecs.ToList()
            };
            return result;
        }

        private async Task<List<AssetForDelivery>> GetProductAttachments(string productID, VerifiedUserContext user)
        {
            var assets = await _assets.ListAssets(new Resource(ResourceType.Products, productID), user);
            var attachments = assets.Where(a => a.Type == AssetType.Attachment).ToList();
            return attachments;
        }

        private async Task<List<AssetForDelivery>> GetProductImages(string productID, VerifiedUserContext user)
        {
            var assets = await _assets.ListAssets(new Resource(ResourceType.Products, productID), user);
            var images = assets.Where(a => a.Type == AssetType.Image).ToList();
            return images;
        }
    }
}
