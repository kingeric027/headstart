using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Common.Services;
using Marketplace.Models;
using Marketplace.Models.Extended;
using OrderCloud.SDK;

namespace Marketplace.Tests.Mocks
{
    public static class Products
    {
        public static MarketplaceProduct ProductWithOneImage()
        {
            return new MarketplaceProduct()
            {
                xp = new ProductXp()
                {
                    Images = new List<ProductImage>()
                    {
                        new ProductImage()
                        {
                            URL = "mock-image-url-0"
                        }
                    }
                }
            };
        }

        public static MarketplaceProduct PatchResponse()
        {
            var product = new MarketplaceProduct();
            product.ID = "patched-product";
            return product;
        }
    }
}
