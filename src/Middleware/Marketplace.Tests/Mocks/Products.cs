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
        public static Product<ProductXp> ProductWithOneImage()
        {
            return new Product<ProductXp>()
            {
                xp = new ProductXp()
                {
                    Images = new List<ProductImage>()
                    {
                        new ProductImage()
                        {
                            URL = "mock-image-url.png"
                        }
                    }
                }
            };
        }

        public static Product PatchResponse()
        {
            var product = new Product();
            product.ID = "patched-product";
            return product;
        }
    }
}
