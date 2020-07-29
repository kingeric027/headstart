using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Common.Services.Zoho;
using Marketplace.Common.Services.Zoho.Mappers;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using ErrorCodes = Marketplace.Models.ErrorCodes;

namespace Marketplace.Common.Commands.Zoho
{
    public interface IZohoCommand
    {
        Task<ZohoSalesOrder> CreateSalesOrder(MarketplaceOrderWorksheet orderWorksheet);
        Task<List<ZohoPurchaseOrder>> CreatePurchaseOrder(ZohoSalesOrder z_order, List<MarketplaceOrder> orders);
        Task<ZohoOrganizationList> ListOrganizations();
    }

    public class ZohoCommand : IZohoCommand
    {
        private readonly IZohoClient _zoho;
        private readonly IOrderCloudClient _oc;

        public ZohoCommand(ZohoClientConfig zoho_config, OrderCloudClientConfig oc_config)
        {
            _zoho = new ZohoClient(zoho_config);
            _oc = new OrderCloudClient(oc_config);
        }
        public ZohoCommand(AppSettings settings)
        {
            _zoho = new ZohoClient(new ZohoClientConfig()
            {
                ApiUrl = "https://books.zoho.com/api/v3",
                AccessToken = settings.ZohoSettings.AccessToken,
                ClientId = settings.ZohoSettings.ClientId,
                ClientSecret = settings.ZohoSettings.ClientSecret,
                OrganizationID = settings.ZohoSettings.OrgID
            });
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                AuthUrl = settings.OrderCloudSettings.ApiUrl,
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                ClientId = settings.OrderCloudSettings.ClientID,
                ClientSecret = settings.OrderCloudSettings.ClientSecret,
                GrantType = GrantType.ClientCredentials,
                Roles = new[] { ApiRole.FullAccess }
            });
            _zoho.AuthenticateAsync();
        }

        public async Task<ZohoOrganizationList> ListOrganizations()
        {
            await _zoho.AuthenticateAsync();
            var results = await _zoho.Organizations.ListAsync();
            return results;
        }

        public async Task<List<ZohoPurchaseOrder>> CreatePurchaseOrder(ZohoSalesOrder z_order, List<MarketplaceOrder> orders)
        {
            try
            {
                var results = new List<ZohoPurchaseOrder>();
                foreach (var order in orders)
                {
                    var delivery_address = z_order.shipping_address; //TODO: this is not good enough. Might even need to go back to SaleOrder and split out by delivery address
                    var supplier = await _oc.Suppliers.GetAsync(order.ToCompanyID);
                    // TODO: accomodate possibility of more than 100 line items
                    var lineitems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, order.ID, pageSize: 100);

                    // Step 1: Create contact (customer) in Zoho
                    var contact = await CreateOrUpdateVendor(order);

                    // Step 2: Create or update Items from LineItems/Products on Order
                    var items = await CreateOrUpdateLineItems(lineitems.Items);

                    // Step 3: Create item for shipments
                    //items.AddRange(await ApplyShipping(order)); do we need shipping here?
                    var purchase_order = await _zoho.PurchaseOrders.CreateAsync(
                        ZohoPurchaseOrderMapper.Map(z_order, order, items, lineitems, delivery_address, contact));
                    results.Add(purchase_order);
                }

                return results;
            }
            catch (Exception ex)
            {
                throw new OrderCloudIntegrationException(ErrorCodes.All["ZohoIntegrationError"], ex.Message);
            }
        }

        public async Task<ZohoSalesOrder> CreateSalesOrder(MarketplaceOrderWorksheet orderWorksheet)
        {
            // consider more robust process for ensuring authentication
            // this relies on this method being called before other methods in this command
            // which might always be true
            await _zoho.AuthenticateAsync();
            try
            {
                // Step 1: Create contact (customer) in Zoho
                var contact = await CreateOrUpdateContact(orderWorksheet.Order);

                // Step 2: Create or update Items from LineItems/Products on Order
                var items = await CreateOrUpdateLineItems(orderWorksheet.LineItems);

                // Step 3: Create item for shipments
                items.AddRange(await ApplyShipping(orderWorksheet));

                // Step 4: create sales order with all objects from above
                var salesOrder = await _zoho.SalesOrders.CreateAsync(ZohoSalesOrderMapper.Map(orderWorksheet.Order, items.ToList(), contact, orderWorksheet.LineItems));

                return salesOrder;
            }
            //TODO: evaluate if more specific throw type would be better for handling in command
            catch (Exception ex)
            {
                throw new OrderCloudIntegrationException(ErrorCodes.All["ZohoIntegrationError"], ex.Message);
            }
        }

        private async Task<List<ZohoLineItem>> CreateOrUpdateLineItems(IList<MarketplaceLineItem> lineitems)
        {
            // TODO: accomodate possibility of more than 100 line items
            var products = await Throttler.RunAsync(lineitems.Select(item => item.ProductID).Distinct().ToList(), 100, 5,
                s => _oc.Products.GetAsync<MarketplaceProduct>(s));

            var zItems = await Throttler.RunAsync(products.ToList(), 100, 5, product => _zoho.Items.ListAsync(new ZohoFilter()
            {
                Key = "sku",
                Value = product.ID
            }));
            var z_items = new Dictionary<string, ZohoLineItem>();
            foreach (var list in zItems)
                list.Items.ForEach(item => z_items.Add(item.sku, item));

            var items = await Throttler.RunAsync(lineitems.ToList(), 100, 5, async lineItem =>
            {
                var marketplaceProduct = lineItem.Product.Reserialize<MarketplaceProduct>();
                var z_item = z_items.FirstOrDefault(z => z.Key == marketplaceProduct.ID);
                if (z_item.Key != null)
                    return await _zoho.Items.SaveAsync(
                        ZohoLineItemMapper.Map(z_item.Value, lineitems.First(i => i.ProductID == marketplaceProduct.ID), marketplaceProduct));
                return await _zoho.Items.CreateAsync(ZohoLineItemMapper.Map(lineitems.First(i => i.ProductID == marketplaceProduct.ID), marketplaceProduct));
            });
            return items.ToList();
        }

        private async Task<List<ZohoLineItem>> ApplyShipping(MarketplaceOrderWorksheet orderWorksheet) {
            //// Step 4: shipping must be added as lineitems on the order
            var z_shipping = await _zoho.Items.ListAsync(new ZohoFilter() { Key = "sku", Value = "shipping"});
            if (z_shipping.Items.Count != 0) return ZohoLineItemMapper.Map(orderWorksheet, z_shipping.Items.FirstOrDefault());
            // doesn't exist so we need to create it. shouldn't happen very often
            var new_shipping = await _zoho.Items.CreateAsync(new ZohoLineItem()
            {
                item_type = "sales_and_purchases",
                name = $"Shipping Charge",
                description = $"Shipping Charge",
                sku = "shipping",
                quantity = 1
            });
            return ZohoLineItemMapper.Map(orderWorksheet, new_shipping);
        }

        private async Task<ZohoContact> CreateOrUpdateVendor(Order order)
        {
            var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(order.ToCompanyID);
            var addresses = await _oc.SupplierAddresses.ListAsync<MarketplaceAddressSupplier>(order.ToCompanyID);
            var users = await _oc.SupplierUsers.ListAsync(order.ToCompanyID);
            var currencies = await _zoho.Currencies.ListAsync();
            var vendor = await _zoho.Contacts.ListAsync(new ZohoFilter() { Key = "contact_name", Value = supplier.Name });
            if (vendor.Items.Any())
            {
                return await _zoho.Contacts.SaveAsync<ZohoContact>(
                    ZohoContactMapper.Map(
                        vendor.Items.FirstOrDefault(),
                        supplier,
                        addresses.Items.FirstOrDefault(),
                        users.Items.FirstOrDefault(),
                        currencies.Items.FirstOrDefault(c => c.currency_code == "USD")));
            }
            else
            {
                return await _zoho.Contacts.CreateAsync<ZohoContact>(
                    ZohoContactMapper.Map(
                        supplier,
                        addresses.Items.FirstOrDefault(),
                        users.Items.FirstOrDefault(),
                        currencies.Items.FirstOrDefault(c => c.currency_code == "USD")));
            }
        }

        private async Task<ZohoContact> CreateOrUpdateContact(Order order)
        {
            var ocBuyer = await _oc.Buyers.GetAsync<MarketplaceBuyer>(order.FromCompanyID);
            var buyerAddress = await _oc.Addresses.GetAsync<MarketplaceAddressBuyer>(order.FromCompanyID, order.BillingAddressID);
            var buyerUserGroup = await _oc.UserGroups.GetAsync<MarketplaceLocationUserGroup>(order.FromCompanyID, order.BillingAddressID);
            var location = new MarketplaceBuyerLocation
            {
                Address = buyerAddress,
                UserGroup = buyerUserGroup
            };
            
            // TODO: MODEL update ~ eventually add a filter to get the primary contact user
            var ocUsers = await _oc.Users.ListAsync<MarketplaceUser>(ocBuyer.ID);
            var currencies = await _zoho.Currencies.ListAsync();

            // TODO: MODEL update ~ right now we don't have actual groups set up for locations, so this isn't accurate or complete
            var zContact = await _zoho.Contacts.ListAsync(new ZohoFilter() { Key = "contact_name", Value = ocBuyer.Name });
            if (zContact.Items.Any())
            {
               return await _zoho.Contacts.SaveAsync<ZohoContact>(
                    ZohoContactMapper.Map(
                        zContact.Items.FirstOrDefault(),
                        ocBuyer,
                        ocUsers.Items.FirstOrDefault(),
                        currencies.Items.FirstOrDefault(c => c.currency_code == (location.UserGroup.xp.Currency != null ? location.UserGroup.xp.Currency.ToString() : "USD")),
                        location));
            }
            else
            {
                return await _zoho.Contacts.CreateAsync<ZohoContact>(
                    ZohoContactMapper.Map(
                        ocBuyer,
                        ocUsers.Items.FirstOrDefault(),
                        currencies.Items.FirstOrDefault(c => c.currency_code == (location.UserGroup.xp.Currency != null ? location.UserGroup.xp.Currency.ToString() : "USD")),
                        location));
            }
        }
    }
}
