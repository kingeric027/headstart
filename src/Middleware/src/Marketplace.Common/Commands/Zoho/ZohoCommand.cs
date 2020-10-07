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
        //Task<List<ZohoPurchaseOrder>> CreateShippingPurchaseOrder(ZohoSalesOrder z_order, MarketplaceOrderWorksheet updatedMarketplaceOrderWorksheet);
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

        //public async Task<List<ZohoPurchaseOrder>> CreateShippingPurchaseOrder(ZohoSalesOrder z_order, MarketplaceOrderWorksheet updatedMarketplaceOrderWorksheet)
        //{
        //    // special request by SMG for creating PO of shipments
        //    foreach (var item in updatedMarketplaceOrderWorksheet.LineItems)
        //    {
        //        if (item.sku == "41000")
        //        {
        //            var vendor = await _zoho.Contacts.ListAsync(new ZohoFilter() { Key = "contact_name", Value = "SMG Shipping" });
        //            var z_shipping = await _zoho.Items.ListAsync(new ZohoFilter() { Key = "sku", Value = "41000" });
        //            var shipping_order = await _zoho.PurchaseOrders.CreateAsync(new ZohoPurchaseOrder()
        //            {
        //                line_items = new List<ZohoLineItem>()
        //                {
        //                    new ZohoLineItem()
        //                    {
        //                        account_id = item.purchase_account_id,
        //                        item_id = item.item_id,
        //                        description = item.description,
        //                        rate = item.rate,
        //                        quantity = 1
        //                    }
        //                },
        //                salesorder_id = z_order.salesorder_id,
        //                reference_number = z_order.reference_number,
        //            })
        //        }
        //    }
        //}

        public async Task<List<ZohoPurchaseOrder>> CreatePurchaseOrder(ZohoSalesOrder z_order, List<MarketplaceOrder> orders)
        {
            try
            {
                var results = new List<ZohoPurchaseOrder>();
                foreach (var order in orders)
                {
                    var delivery_address =
                        z_order
                            .shipping_address; //TODO: this is not good enough. Might even need to go back to SaleOrder and split out by delivery address
                    var supplier = await _oc.Suppliers.GetAsync(order.ToCompanyID);
                    // TODO: accomodate possibility of more than 100 line items
                    var lineitems =
                        await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, order.ID,
                            pageSize: 100);

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
                var salesOrder = await CreateSalesOrder(orderWorksheet, items, contact);

                return salesOrder;
            }
            //TODO: evaluate if more specific throw type would be better for handling in command
            catch (Exception ex)
            {
                throw new OrderCloudIntegrationException(ErrorCodes.All["ZohoIntegrationError"], ex.Message);
            }
        }

        private async Task<ZohoSalesOrder> CreateSalesOrder(MarketplaceOrderWorksheet orderWorksheet, IEnumerable<ZohoLineItem> items, ZohoContact contact)
        {
            // promotions aren't part of the order worksheet, so we have to get them from OC
            var promotions = await _oc.Orders.ListPromotionsAsync(OrderDirection.Incoming, orderWorksheet.Order.ID);
            var zOrder = await _zoho.SalesOrders.ListAsync(new ZohoFilter() { Key = "reference_number", Value = orderWorksheet.Order.ID });
            if (zOrder.Items.Any())
                return await _zoho.SalesOrders.SaveAsync(ZohoSalesOrderMapper.Map(zOrder.Items.FirstOrDefault(), orderWorksheet.Order, items.ToList(), contact, orderWorksheet.LineItems, promotions.Items));
            return await _zoho.SalesOrders.CreateAsync(ZohoSalesOrderMapper.Map(orderWorksheet.Order, items.ToList(), contact, orderWorksheet.LineItems, promotions.Items));
        } 

        private async Task<List<ZohoLineItem>> CreateOrUpdateLineItems(IList<MarketplaceLineItem> lineitems)
        {
            // TODO: accomodate possibility of more than 100 line items
            // Overview: variants will be saved in Zoho as the Item. If the variant is null save the Product as the Item

            // gather IDs either at the product or variant level to search Zoho for existing Items 
            var itemIds = lineitems.Select(item => item.Variant == null ? item.Product.ID : item.Variant.ID);

            var zItems = await Throttler.RunAsync(itemIds, 100, 5, id => _zoho.Items.ListAsync(new ZohoFilter()
            {
                Key = "sku",
                Value = id
            }));
            // the search api returns a list always. if no item was found the list will be empty
            // so we want to get found items into a pared down list
            var z_items = new Dictionary<string, ZohoLineItem>();
            foreach (var list in zItems)
                list.Items.ForEach(item => z_items.Add(item.sku, item));

            var items = await Throttler.RunAsync(lineitems.ToList(), 100, 5, async lineItem =>
            {
                var z_item = z_items.FirstOrDefault(z => lineItem.Variant != null ? z.Key == lineItem.Variant.ID : z.Key == lineItem.Product.ID);
                if (z_item.Key != null)
                    return await _zoho.Items.SaveAsync(ZohoLineItemMapper.Map(z_item.Value, lineitems.First(i => i.ProductID == lineItem.Product.ID), lineItem.Product, lineItem.Variant));
                return await _zoho.Items.CreateAsync(ZohoLineItemMapper.Map(lineitems.First(i => i.ProductID == lineItem.Product.ID), lineItem.Product, lineItem.Variant));
            });
            return items.ToList();
        }

        private async Task<List<ZohoLineItem>> ApplyShipping(MarketplaceOrderWorksheet orderWorksheet) {
            //// Step 4: shipping must be added as lineitems on the order
            var z_shipping = await _zoho.Items.ListAsync(new ZohoFilter() { Key = "sku", Value = "41000"});
            if (z_shipping.Items.Count != 0) return ZohoLineItemMapper.Map(orderWorksheet, z_shipping.Items.FirstOrDefault());
            // doesn't exist so we need to create it. shouldn't happen very often
            var new_shipping = await _zoho.Items.CreateAsync(new ZohoLineItem()
            {
                item_type = "sales_and_purchases",
                name = $"Shipping Charge",
                description = $"Shipping Charge",
                sku = "41000",
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
            var ocUsers = await _oc.Users.ListAsync<MarketplaceUser>(ocBuyer.ID, buyerUserGroup.ID);
            var location = new MarketplaceBuyerLocation
            {
                Address = buyerAddress,
                UserGroup = buyerUserGroup
            };
            
            // TODO: MODEL update ~ eventually add a filter to get the primary contact user
            var currencies = await _zoho.Currencies.ListAsync();

            // TODO: MODEL update ~ right now we don't have actual groups set up for locations, so this isn't accurate or complete
            var zContactList = await _zoho.Contacts.ListAsync(new ZohoFilter() { Key = "contact_name", Value = $"{location.Address?.AddressName} - {location.Address?.xp.LocationID}"});
            var zContact = await _zoho.Contacts.GetAsync(zContactList.Items.FirstOrDefault()?.contact_id);
            if (zContact != null)
            {
                try
                {
                    var map = ZohoContactMapper.Map(
                        zContact.Item,
                        ocBuyer,
                        ocUsers.Items,
                        currencies.Items.FirstOrDefault(c =>
                            c.currency_code == (location.UserGroup.xp.Currency != null
                                ? location.UserGroup.xp.Currency.ToString()
                                : "USD")),
                        location);
                    return await _zoho.Contacts.SaveAsync<ZohoContact>(map);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            var contact = ZohoContactMapper.Map(
                ocBuyer,
                ocUsers.Items,
                currencies.Items.FirstOrDefault(c =>
                    c.currency_code == (location.UserGroup.xp.Currency != null
                        ? location.UserGroup.xp.Currency.ToString()
                        : "USD")),
                location);
            return await _zoho.Contacts.CreateAsync<ZohoContact>(contact);
        }
    }
}
