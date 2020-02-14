using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Mappers.Zoho;
using Marketplace.Common.Services.Zoho;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Helpers;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Zoho
{
    public interface IZohoCommand
    {
        Task<ZohoContact> CreateOrUpdate(MarketplaceOrder order);
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

        public async Task<ZohoListContactList> ListAsync(params ZohoFilter[] filters)
        {
            var contact = await _zoho.Contacts.ListAsync(filters);
            return contact;
        }

        public async Task<ZohoContact> GetAsync()
        {
            var contact = await _zoho.Contacts.GetAsync("");
            return contact;
        }

        public async Task<ZohoContact> CreateOrUpdate(MarketplaceOrder order)
        {
            var ocBuyer = await _oc.Buyers.GetAsync<MarketplaceBuyer>(order.FromCompanyID);
            // eventually add a filter to get the primary contact user
            var ocUsers = await _oc.Users.ListAsync<MarketplaceUser>(ocBuyer.ID);
            // eventually we'll add a filter for the user group type that represents the location
            var ocGroupAssignments = await Throttler.RunAsync(ocUsers.Items, 100, 5, marketplaceUser => _oc.UserGroups.ListUserAssignmentsAsync(ocBuyer.ID, userID: marketplaceUser.ID, pageSize: 100));
            var ocGroupAssignmentList = new List<UserGroupAssignment>();
            foreach (var ocGroupAssignment in ocGroupAssignments)
                ocGroupAssignmentList.AddRange(ocGroupAssignment.Items);

            var ocGroups = await Throttler.RunAsync(ocGroupAssignmentList, 100, 5,
                assignment => _oc.UserGroups.GetAsync<MarketplaceUserGroup>(ocBuyer.ID, assignment.UserGroupID));
            // TODO: make this use the Party property for currency
            var currencies = await _zoho.Currencies.ListAsync(); 

            var addresses = await _oc.Addresses.ListAsync<MarketplaceAddress>(ocBuyer.ID);

            // right now we don't have actual groups set up for locations, so this isn't accurate or complete
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

    

    //    //private async Task<ZohoContactPerson> CreateOrUpdate()
    //    //{

    //    //}

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
