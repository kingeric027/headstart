using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Mappers.Zoho;
using Marketplace.Common.Models;
using Marketplace.Common.Services.Zoho;
using Marketplace.Common.Services.Zoho.Exceptions;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Zoho
{
    public interface IZohoCommand {}
    public class ZohoCommand : IZohoCommand
    {
        private readonly IZohoClient _zoho;
        private readonly IOrderCloudClient _oc;
        public ZohoCommand(AppSettings settings, IZohoClient zoho, IOrderCloudClient oc)
        {
            _zoho = zoho;
            _zoho.Init(settings.ZohoSettings.AuthToken, settings.ZohoSettings.OrgID);
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

        public async Task ReceiveBuyerOrder(Order order)
        {
            try
            {
                // Step 1: update existing contact or create new based on order from company id
                var buyer = await _oc.Buyers.GetAsync(order.FromCompanyID);
                var contact = await _zoho.GetOrCreateContact(ZohoMapper.Map(buyer));

                // Step 2: update existing contact person or create new based on order from user id
                var user = await _oc.Users.GetAsync(order.FromCompanyID, order.FromUserID);
                var person = await _zoho.GetOrCreateContactPerson(contact, ZohoMapper.Map(user, contact));

                // Step 3: update existing item (product) or create new based on lineitems on order
                var lineitems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, order.ID, pageSize: 100); // TODO: accomodate possibility of more than 100 line items
                var products = await Throttler.RunAsync(lineitems.Items.Select(item => item.ProductID).ToList(), 100, 5,
                    s => _oc.Products.GetAsync(s));
                var items = await Throttler.RunAsync(products.Select(p => p).ToList(), 100, 5,
                    product => _zoho.GetOrCreateLineItem(ZohoMapper.Map(order, lineitems.Items.First(i => i.ProductID == product.ID), product)));

                // Step 4: shipping must be added as lineitems on the order


                // Step 4: create sales order with all objects from above
                var salesOrder = await _zoho.CreateSalesorder(ZohoMapper.Map(order, contact, items.ToList(), person, buyer));
            }
            catch (BooksException ex)
            {
                throw new ZohoException(ex.Message, ex);
            }
        }
    }
}
