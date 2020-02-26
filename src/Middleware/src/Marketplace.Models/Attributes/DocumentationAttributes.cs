using System;
using Marketplace.Helpers.SwaggerTools;

namespace Marketplace.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MarketplaceSection : DocSection
    {
        public MarketplaceSection()
        {
            
        }
        
        public class OrchestrationAttribute : DocSection { }
        /// <summary>
        /// Use on controllers to indicate that they belong in the Authentication section of the API reference docs.
        /// </summary>
        public class AuthenticationAndAuthorizationAttribute : DocSection { }
        /// <summary>
        /// Use on controllers to indicate that they belong in the Seller section of the API reference docs.
        /// </summary>
        public class SellerAttribute : DocSection { }
        /// <summary>
        /// Use on controllers to indicate that they belong in the Buyers section of the API reference docs.
        /// </summary>
        public class BuyersAttribute : DocSection { }
        /// <summary>
        /// Use on controllers to indicate that they belong in the Suppliers section of the API reference docs.
        /// </summary>
        public class SuppliersAttribute : DocSection { }
        /// <summary>
        /// Use on controllers to indicate that they belong in the Product Catalogs section of the API reference docs.
        /// </summary>
        public class ProductCatalogsAttribute : DocSection { }
        /// <summary>
        /// Use on controllers to indicate that they belong in the Orders section of the API reference docs.
        /// </summary>
        public class OrdersAndFulfillmentAttribute : DocSection { }
        /// <summary>
        /// Use on controllers to indicate that they belong in "me" section of the API reference docs.
        /// </summary>
        public class MeAndMyStuffAttribute : DocSection { }
    }
}
