using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Crud
{
    public interface IMarketplaceProductCommand
    {
        Task<SuperMarketplaceProduct> Get(string id, VerifiedUserContext user);
        Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user);
        Task<SuperMarketplaceProduct> Post(SuperMarketplaceProduct product, VerifiedUserContext user);
        Task<SuperMarketplaceProduct> Put(string id, SuperMarketplaceProduct product, VerifiedUserContext user);
        Task Delete(string id, VerifiedUserContext user);

    }

    public class MarketplaceProductCommand : IMarketplaceProductCommand
    {
        private readonly IOrderCloudClient _oc;
        public MarketplaceProductCommand(AppSettings settings)
        {
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                AuthUrl = settings.OrderCloudSettings.AuthUrl,
            });
        }

        public async Task<SuperMarketplaceProduct> Get(string id, VerifiedUserContext user)
        {
            var _product =  await _oc.Products.GetAsync<MarketplaceProduct>(id, user.AccessToken);
            var _priceSchedule = await _oc.PriceSchedules.GetAsync<PriceSchedule>(_product.DefaultPriceScheduleID, user.AccessToken);
            return new SuperMarketplaceProduct
            {
                Product = _product,
                PriceSchedule = _priceSchedule
            };
        }

        public async Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args, VerifiedUserContext user)
        {
            var _productsList = await _oc.Products.ListAsync<MarketplaceProduct>(filters: args.ToFilterString(),
                accessToken: user.AccessToken);
            var _superProductsList = new List<SuperMarketplaceProduct> { };
            foreach (var product in _productsList.Items)
            {
                var priceSchedule = await _oc.PriceSchedules.GetAsync(product.DefaultPriceScheduleID, user.AccessToken);
                _superProductsList.Add(new SuperMarketplaceProduct
                {
                    Product = product,
                    PriceSchedule = priceSchedule
                });
            }
            return new ListPage<SuperMarketplaceProduct>
            {
                Meta = _productsList.Meta,
                Items = _superProductsList
            };
        }

        public async Task<SuperMarketplaceProduct> Post(SuperMarketplaceProduct superProduct, VerifiedUserContext user)
        {
            var _priceSchedule = await _oc.PriceSchedules.CreateAsync<PriceSchedule>(superProduct.PriceSchedule, user.AccessToken);
            superProduct.Product.DefaultPriceScheduleID = _priceSchedule.ID;
            var _product = await _oc.Products.CreateAsync<MarketplaceProduct>(superProduct.Product, user.AccessToken);
            return new SuperMarketplaceProduct
            {
                Product = _product,
                PriceSchedule = _priceSchedule
            };
        }

        public async Task<SuperMarketplaceProduct> Put(string id, SuperMarketplaceProduct superProduct, VerifiedUserContext user)
        {
            var _updatedPriceSchedule = await _oc.PriceSchedules.SaveAsync<PriceSchedule>(superProduct.PriceSchedule.ID, superProduct.PriceSchedule, user.AccessToken);
            var _updatedProduct = await _oc.Products.SaveAsync<MarketplaceProduct>(superProduct.Product.ID, superProduct.Product, user.AccessToken);
            return new SuperMarketplaceProduct
            {
                Product = _updatedProduct,
                PriceSchedule = _updatedPriceSchedule
            };
        }

        public async Task Delete(string id, VerifiedUserContext user)
        {
            await _oc.Products.DeleteAsync(id, user.AccessToken);
        }
    }
}
