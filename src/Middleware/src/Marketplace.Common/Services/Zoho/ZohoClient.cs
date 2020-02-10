using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Common.Services.Zoho.Service;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using ZohoOrganization = Marketplace.Common.Services.Zoho.Models.ZohoOrganization;


namespace Marketplace.Common.Services.Zoho
{
    public interface IZohoClient
    {
        ZohoClient Init(string token, string id);
        Task<ZohoOrganization> Ping(string id);
        Task<ZohoSalesorder> CreateSalesorder(ZohoSalesorder order);
        Task<ZohoLineItem> GetOrCreateLineItem(ZohoLineItem item);
        Task<ZohoContact> GetOrCreateContact(ZohoContact zohoContact);
        Task<ZohoContactPerson> GetOrCreateContactPerson(ZohoContact zohoContact, ZohoContactPerson person);
        Task<ZohoSalesorder> GetSalesOrder(string id);
        Task<ZohoItemList> SearchLineItem(string search);
        Task<dynamic> RequestToken(string code);
        Task<dynamic> Auth();
    }

    public class ZohoClient : IZohoClient
    {
        private readonly ZohoBooks _zoho;
        private readonly AppSettings _settings;

        public ZohoClient(AppSettings settings)
        {
            _settings = settings;
            _zoho = new ZohoBooks();
        }

        public ZohoClient Init(string token, string id)
        {
            _zoho.initialize(token, id);
            return this;
        }

        public async Task<dynamic> RequestToken(string code)
        {
            var request = await "https://accounts.zoho.com/oauth/v2/token"
                .SetQueryParam("code", code)
                .SetQueryParam("client_id", "1000.9BTEOO38B36J41SW6XGHA027P6IU3H")
                .SetQueryParam("client_secret", "43623d92d1c14dc145691ca9b60e35f40ce40c8ea0")
                .SetQueryParam("grant_type", "authorization_code")
                .SetQueryParam("redirect_uri", "https://a110fb7f.ngrok.io/zoho/token")
                .GetJsonAsync();
            return request;
        }

        public async Task<dynamic> Auth()
        {
            try
            {
                var request = await "https://accounts.zoho.com/oauth/v2/auth"
                    .SetQueryParam("scope", "ZohoBooks.fullaccess.all")
                    .SetQueryParam("client_id", "1000.9BTEOO38B36J41SW6XGHA027P6IU3H")
                    .SetQueryParam("response_type", "code")
                    .SetQueryParam("redirect_uri", "https://a110fb7f.ngrok.io/zoho/token")
                    .SetQueryParam("access_type", "offline")
                    .SetQueryParam("prompt", "none")
                    .GetAsync();
                return request;
            }
            catch (FlurlHttpException ex)
            {
                throw new ApiErrorException(new ErrorCode(ex.Call.Response.StatusCode.ToString(), 500, ex.Message), ex);
            }
        }

        public async Task<string> GetToken(string email, string password)
        {
            return await "https://accounts.zoho.com/apiauthtoken/create".SetQueryParam("EMAIL_ID", email)
                .SetQueryParam("PASSWORD", password).GetStringAsync();
        }

        public async Task<ZohoOrganization> Ping(string orgId)
        {
            var org = _zoho.GetOrganizationsApi().Get(orgId);
            return await Task.FromResult(org);
        }

        public async Task<ZohoSalesorder> CreateSalesorder(ZohoSalesorder order)
        {
            try
            {
                var o = _zoho.GetSalesordersApi().Create(order, null);
                return await Task.FromResult(o);
            }
            catch (Exception ex)
            {
                throw new ZohoException(ex.Message, ex);
            }
        }

        public async Task<ZohoContactPerson> GetOrCreateContactPerson(ZohoContact zohoContact, ZohoContactPerson person)
        {
            try
            {
                var zPerson = _zoho.GetContactsApi().GetContactPersons(zohoContact.contact_id, new Dictionary<object, object>
                {
                    { "email", person.email }
                }).First();
                return await Task.FromResult(_zoho.GetContactsApi().UpdateContactperson(zPerson.contact_person_id, person));
            }
            catch (Exception)
            {
                return await Task.FromResult(_zoho.GetContactsApi().CreateContactPerson(person));
            }
        }

        public async Task<ZohoItemList> SearchLineItem(string search)
        {
            var list = _zoho.GetItemsApi().GetItems(new Dictionary<object, object>
            {
                { "name_contains", search }
            });
            return list;
        }

        public async Task<ZohoLineItem> GetOrCreateLineItem(ZohoLineItem item)
        {
            try
            {
                var lineItem = _zoho.GetItemsApi().GetItems(new Dictionary<object, object>
                {
                    {"sku", item.sku}
                });
                if (!lineItem.Any())
                {
                    var zItem = _zoho.GetItemsApi().Create(item);
                    zItem.quantity = item.quantity;
                    zItem.rate = item.rate;
                    return await Task.FromResult(zItem);
                }
                try
                {
                    item.item_id = lineItem.First().item_id;
                    var update = _zoho.GetItemsApi().Update(lineItem.First().item_id, item);
                    _zoho.GetItemsApi().MarkAsActive(update.item_id);
                    update.quantity = item.quantity;
                    update.rate = item.rate;
                    return await Task.FromResult(update);
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(item);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.EndsWith("already exists.")) return item;
                throw new ZohoException(ex.Message, ex);
            }
        }

        public async Task<ZohoContact> GetOrCreateContact(ZohoContact zohoContact)
        {
            try
            {
                var zContact = _zoho.GetContactsApi().GetContacts(new Dictionary<object, object>
                {
                    {"contact_name", zohoContact.contact_name}
                });
                if (zContact.Any())
                    return await Task.FromResult(zContact.First());
                try
                {
                    return await Task.FromResult(_zoho.GetContactsApi().Create(zohoContact));
                }
                catch (Exception ex)
                {
                    throw new ZohoException(ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                throw new ZohoException(ex.Message, ex);
            }
        }

        public async Task<ZohoSalesorder> GetSalesOrder(string id)
        {
            var order = _zoho.GetSalesordersApi().Get(id, null);
            return await Task.FromResult(order);
        }

        //    public async Task<ZohoPurchaseOrder> CreatePurchaseOrder(cXML cXmlOrder, Order<StorefrontOrderXp, UserXp, AddressXp> order,
        //        List<ZohoLineItem<StorefrontLineItemXp, ProductXp, AddressXp, AddressXp>> lineitems, Buyer<BuyerXp> buyer,
        //        ZohoUser<UserXp> user)
        //    {
        //        //var grouped_lineitems = lineitems
        //        //    .GroupBy(li => li.xp.Product.EndCustomerOrderID == cXML && li.xp.Product.BuyerID == buyer.ID).Select(li => li)
        //        //    .ToList();
        //        //var ocSalesOrder =
        //        //    await _oc.Orders.ListAsync<Order<StorefrontOrderXp, UserXp, AddressXp>>(OrderDirection.Incoming,
        //        //        opts => opts.AddFilter(p => p.xp.IDs.StorefrontSalesOrderID == cXmlOrder.));
        //        var contacts = _zoho.GetContactsApi().GetContacts(new Dictionary<object, object>());
        //        var expenses = _zoho.GetChartOfAccountsApi().GetChartOfAcounts(null);
        //        var id = expenses.FirstOrDefault(e => e.account_name == "netfx_expense");
        //        var so = _zoho.GetSalesordersApi().GetSalesorders(new Dictionary<object, object>()
        //        {
        //            {"salesorder_id", order.xp.IDs.ZohoID}
        //        });

        //        var z_lineitems = new List<ZohoLineItem>();
        //        lineitems.ForEach(item =>
        //        {
        //            z_lineitems.Add(this.GetOrCreateLineItem(ZohoMapper.MapOrderCloudLineItemToZohoLineItem(item, "sales_and_purchases")));
        //        });

        //        var po = _zoho.GetPurchaseordersApi().Create(new ZohoPurchaseOrder()
        //        {
        //            vendor_id = contacts.First(c => c.contact_type == "vendor").contact_id,
        //            salesorder_id = so.FirstOrDefault().salesorder_id,
        //            line_items = z_lineitems,
        //        }, null, lineitems.Where(i => i.xp.Product.URL != null).Select(i => i.xp.Product.URL).ToArray()); 
        //        return await Task.FromResult(po);
        //    }

        
    }
}

