using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.Zoho;
using Marketplace.Common.Services.Zoho.Mappers;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Extensions;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using ErrorCodes = Marketplace.Models.ErrorCodes;

namespace Marketplace.Common.Commands.Zoho
{
    public interface IZohoCommand
    {
        Task<ZohoSalesOrder> CreateSalesOrder(MarketplaceOrder order);
        Task<List<ZohoPurchaseOrder>> CreatePurchaseOrder(ZohoSalesOrder z_order, OrderSplitResult orders);
    }

    public class ZohoCommand : IZohoCommand
    {
        private readonly IZohoClient _zoho;
        private readonly IOrderCloudClient _oc;

        public ZohoCommand(AppSettings settings)
        {
            _zoho = new ZohoClient(new ZohoClientConfig()
            {
                ApiUrl = "https://books.zoho.com/api/v3",
                AuthToken = settings.ZohoSettings.AuthToken,
                OrganizationID = settings.ZohoSettings.OrgID
            });
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                AuthUrl = settings.OrderCloudSettings.AuthUrl,
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                ClientId = settings.OrderCloudSettings.ClientID,
                ClientSecret = settings.OrderCloudSettings.ClientSecret,
                GrantType = GrantType.ClientCredentials,
                Roles = new[] { ApiRole.FullAccess }
            });
        }

        public async Task<List<ZohoPurchaseOrder>> CreatePurchaseOrder(ZohoSalesOrder z_order, OrderSplitResult orders)
        {
            try
            {
                var results = new List<ZohoPurchaseOrder>();
                foreach (var order in orders.OutgoingOrders)
                {
                    var delivery_address = z_order.shipping_address; //TODO: this is not good enough. Might even need to go back to SaleOrder and split out by delivery address
                    var supplier = await _oc.Suppliers.GetAsync(order.ToCompanyID);
                    // TODO: accomodate possibility of more than 100 line items
                    var lineitems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, order.ID, pageSize: 100);

                    // Step 1: Create contact (customer) in Zoho
                    var contact = await CreateOrUpdateVendor(order);

                    // Step 2: Create or update Items from LineItems/Products on Order
                    var items = await CreateOrUpdateLineItems(lineitems);

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
                throw new ApiErrorException(ErrorCodes.All["ZohoIntegrationError"], ex.Message);
            }
        }

        public async Task<ZohoSalesOrder> CreateSalesOrder(MarketplaceOrder order)
        {
            try
            {
                // TODO: accomodate possibility of more than 100 line items
                var lineitems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Incoming, order.ID, pageSize: 100);

                // Step 1: Create contact (customer) in Zoho
                var contact = await CreateOrUpdateContact(order);

                // Step 2: Create or update Items from LineItems/Products on Order
                var items = await CreateOrUpdateLineItems(lineitems);

                // Step 3: Create item for shipments
                items.AddRange(await ApplyShipping(order));

                // Step 4: create sales order with all objects from above
                var salesOrder =
                    await _zoho.SalesOrders.CreateAsync(ZohoSalesOrderMapper.Map(order, items.ToList(), contact,
                        lineitems));

                return salesOrder;
            }
            //TODO: evaluate if more specific throw type would be better for handling in command
            catch (Exception ex)
            {
                throw new ApiErrorException(ErrorCodes.All["ZohoIntegrationError"], ex.Message);
            }
        }

        private async Task<List<ZohoLineItem>> CreateOrUpdateLineItems(ListPage<MarketplaceLineItem> lineitems)
		{
			// TODO: accomodate possibility of more than 100 line items
			var products = await Throttler.RunAsync(lineitems.Items.Select(item => item.ProductID).ToList(), 100, 5,
                s => _oc.Products.GetAsync<MarketplaceProduct>(s));

            var zItems = await Throttler.RunAsync(products.ToList(), 100, 5, product => _zoho.Items.ListAsync(new ZohoFilter()
            {
                Key = "sku",
                Value = product.ID
            }));
            var z_items = new Dictionary<string, ZohoLineItem>();
            foreach (var list in zItems)
                list.Items.ForEach(item => z_items.Add(item.sku, item));

            var items = await Throttler.RunAsync(products.Select(p => p).ToList(), 100, 5, async product =>
            {
                var z_item = z_items.FirstOrDefault(z => z.Key == product.ID);
                if (z_item.Key != null)
                    return await _zoho.Items.SaveAsync(
                        ZohoLineItemMapper.Map(z_item.Value, lineitems.Items.First(i => i.ProductID == product.ID), product));
                return await _zoho.Items.CreateAsync(ZohoLineItemMapper.Map(lineitems.Items.First(i => i.ProductID == product.ID), product));
            });
            return items.ToList();
        }

        private async Task<List<ZohoLineItem>> ApplyShipping(MarketplaceOrder order) {
            //// Step 4: shipping must be added as lineitems on the order
            var z_shipping = await _zoho.Items.ListAsync(new ZohoFilter() { Key = "sku", Value = "shipping"});
            if (z_shipping.Items.Count != 0) return ZohoLineItemMapper.Map(order, z_shipping.Items.FirstOrDefault());
            // doesn't exist so we need to create it. shouldn't happen very often
            var new_shipping = await _zoho.Items.CreateAsync(new ZohoLineItem()
            {
                item_type = "sales_and_purchases",
                name = $"Shipping Charge",
                description = $"Shipping Charge",
                sku = "shipping",
                quantity = 1
            });
            return ZohoLineItemMapper.Map(order, new_shipping);
        }

        private async Task<ZohoContact> CreateOrUpdateVendor(Order order)
        {
            var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(order.ToCompanyID);
            var addresses = await _oc.SupplierAddresses.ListAsync<MarketplaceAddress>(order.ToCompanyID);
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
            // as this routine evolves around model updates to avoid so much listing abstract this out into a routine for reuse
            var ocBuyer = await _oc.Buyers.GetAsync<MarketplaceBuyer>(order.FromCompanyID);
            // TODO: MODEL update ~ eventually add a filter to get the primary contact user
            var ocUsers = await _oc.Users.ListAsync<MarketplaceUser>(ocBuyer.ID);

            // TODO: MODEL update ~ eventually we'll add a filter for the user group type that represents the location
            var ocGroupAssignments = await Throttler.RunAsync(ocUsers.Items, 100, 5, marketplaceUser => _oc.UserGroups.ListUserAssignmentsAsync(ocBuyer.ID, userID: marketplaceUser.ID, pageSize: 100));
            var ocGroupAssignmentList = new List<UserGroupAssignment>();
            foreach (var ocGroupAssignment in ocGroupAssignments)
                ocGroupAssignmentList.AddRange(ocGroupAssignment.Items);

            var ocGroups = await Throttler.RunAsync(ocGroupAssignmentList, 100, 5,
                assignment => _oc.UserGroups.GetAsync<MarketplaceUserGroup>(ocBuyer.ID, assignment.UserGroupID));
            // TODO: MODEL update ~ make this use the Party property for currency
            var addresses = await _oc.Addresses.ListAsync<MarketplaceAddress>(ocBuyer.ID);
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
                        ocGroups.FirstOrDefault(),
                        currencies.Items.FirstOrDefault(c => c.currency_code == "USD"),
                        addresses.Items.FirstOrDefault()));
            }
            else
            {
                return await _zoho.Contacts.CreateAsync<ZohoContact>(
                    ZohoContactMapper.Map(
                        ocBuyer,
                        ocUsers.Items.FirstOrDefault(),
                        ocGroups.FirstOrDefault(),
                        currencies.Items.FirstOrDefault(c => c.currency_code == "USD"),
                        addresses.Items.FirstOrDefault()));
            }
        }
    }
    //public class ZohoCommand : IZohoCommand
    //{
    //    private readonly IZohoClient _zoho;
    //    private readonly IOrderCloudClient _oc;

    //    public ZohoCommand(AppSettings settings, IZohoClient zoho, IOrderCloudClient oc)
    //    {
    //        _zoho = zoho.Init(settings.ZohoSettings.AuthToken, settings.ZohoSettings.OrgID);
    //        _oc = new OrderCloudClient(new OrderCloudClientConfig()
    //        {
    //            AuthUrl = settings.OrderCloudSettings.AuthUrl,
    //            ApiUrl = settings.OrderCloudSettings.ApiUrl,
    //            ClientId = settings.OrderCloudSettings.ClientID,
    //            ClientSecret = settings.OrderCloudSettings.ClientSecret,
    //            GrantType = GrantType.ClientCredentials,
    //            Roles = new[] { ApiRole.FullAccess }
    //        });
    //    }

    //    public async Task ReceiveBuyerOrder(MarketplaceOrder order)
    //    {
    //        try
    //        {
    //            // Step 1: update existing contact or create new based on order from company id
    //            var zContact = await this.CreateOrUpdate(order);

    //            //// Step 2: update existing contact person or create new based on order from user id
    //            //var user = await _oc.Users.GetAsync(order.FromCompanyID, order.FromUserID);
    //            //var person = await _zoho.GetOrCreateContactPerson(contact, ZohoMapper.Map(user, contact));

    //            //// Step 3: update existing item (product) or create new based on lineitems on order
    //            //var lineitems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, order.ID, pageSize: 100); // TODO: accomodate possibility of more than 100 line items
    //            //var products = await Throttler.RunAsync(lineitems.Items.Select(item => item.ProductID).ToList(), 100, 5,
    //            //    s => _oc.Products.GetAsync<MarketplaceProduct>(s));
    //            //var items = await Throttler.RunAsync(products.Select(p => p).ToList(), 100, 5,
    //            //    product => _zoho.GetOrCreateLineItem(ZohoMapper.Map(order, lineitems.Items.First(i => i.ProductID == product.ID), product)));

    //            //// Step 4: shipping must be added as lineitems on the order
    //            //var shipments = ZohoMapper.Map(order);
    //            //foreach (var s in shipments) items.Add(s);

    //            //// Step 4: create sales order with all objects from above
    //            //var salesOrder = await _zoho.CreateSalesorder(ZohoMapper.Map(order, contact, items.ToList(), person, ocBuyer));
    //        }
    //        catch (BooksException ex)
    //        {
    //            throw new ZohoException(ex.Message, ex);
    //        }
    //    }
    //}
}
