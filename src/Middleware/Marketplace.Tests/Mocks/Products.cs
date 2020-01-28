using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Common.Services;
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
                            Url = "mock-image-url.png"
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
