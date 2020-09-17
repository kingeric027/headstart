using Marketplace.Common.Queries;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IProductUpdateCommand
    {
        Task SendAllProductUpdateEmails();
        Task SendProductUpdateEmail(string supplierID);
    }

    public class ProductUpdateCommand : IProductUpdateCommand
    {
        private readonly ProductUpdateQuery _productUpdate;
        private readonly IOrderCloudClient _oc;
        public ProductUpdateCommand(IOrderCloudClient oc, ProductUpdateQuery productUpdate)
        {
            _productUpdate = productUpdate;
            _oc = oc;
        }

        public async Task SendAllProductUpdateEmails()
        {
            var allSuppliers = new List<MarketplaceSupplier>();
            var suppliersFirstPage = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(pageSize: 100);
            allSuppliers.Concat(suppliersFirstPage.Items);
            var i = 2;
            while (i <= suppliersFirstPage.Meta.TotalPages)
            {
                var newPage = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(pageSize: 100, page: i);
                allSuppliers.Concat(newPage.Items);
                i++;
            }
            //await Throttler.RunAsync(allSuppliers, 100, 5, (supplier) => )
        }

        public async Task SendProductUpdateEmail(string supplierID)
        {
            var now = new DateTime();
            var updatedProducts = await _productUpdate.List(supplierID, now);
            //  now we need to just send an email
        }
    }


}
