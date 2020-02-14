using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Services.Zoho.Resources
{
    public interface IZohoContactResource
    {
        Task<ZohoListContactList> ListAsync(params ZohoFilter[] filters);
        Task<TZohoContactList> ListAsync<TZohoContactList>(params ZohoFilter[] filters) where TZohoContactList : ZohoListContactList;
        Task<ZohoContact> GetAsync(string id);
        Task<TZohoContact> GetAsync<TZohoContact>(string id) where TZohoContact : ZohoContact;
        Task<ZohoContact> SaveAsync(ZohoContact contact);
        Task<TZohoContact> SaveAsync<TZohoContact>(TZohoContact contact) where TZohoContact : ZohoContact;
        Task<ZohoContact> CreateAsync(ZohoContact contact);
        Task<TZohoContact> CreateAsync<TZohoContact>(TZohoContact contact) where TZohoContact : ZohoContact;
        Task DeleteAsync(string id);
    }

    public class ZohoContactResource : ZohoResource, IZohoContactResource
    {
        internal ZohoContactResource(ZohoClient client) : base(client) { }

        public Task<ZohoListContactList> ListAsync(params ZohoFilter[] filters) => ListAsync<ZohoListContactList>(filters);
        public Task<TZohoContactList> ListAsync<TZohoContactList>(params ZohoFilter[] filters) where TZohoContactList : ZohoListContactList
        {
            return Request("contacts")
                .SetQueryParams(filters?.Select(f => new KeyValuePair<string, object>(f.Key, f.Value)))
                .GetJsonAsync<TZohoContactList>();
        }

        public Task<ZohoContact> GetAsync(string id) => GetAsync<ZohoContact>(id);
        
        public Task<TZohoContact> GetAsync<TZohoContact>(string id) where TZohoContact : ZohoContact =>
            Request("contacts", id).GetJsonAsync<TZohoContact>();
        
        public Task<ZohoContact> SaveAsync(ZohoContact contact) => SaveAsync<ZohoContact>(contact);

        public async Task<TZohoContact> SaveAsync<TZohoContact>(TZohoContact contact) where TZohoContact : ZohoContact
        {
            var response = await Request("contacts", 1) // contact.contact_id)
                .SetQueryParam("JSONString", JsonConvert.SerializeObject(contact))
                .PutAsync(null);
                return JObject
                    .Parse(await response.Content.ReadAsStringAsync())
                    .SelectToken("contact").ToObject<TZohoContact>();
        }
            
        public Task<ZohoContact> CreateAsync(ZohoContact contact) => SaveAsync<ZohoContact>(contact);

        public async Task<TZohoContact> CreateAsync<TZohoContact>(TZohoContact contact) where TZohoContact : ZohoContact
        {
            var response = await Request("contacts")
                .SetQueryParam("JSONString", JsonConvert.SerializeObject(contact, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore}))
                .PostJsonAsync(null);
                return JObject
                    .Parse(await response.Content.ReadAsStringAsync())
                    .SelectToken("contact").ToObject<TZohoContact>();
        }

        public Task DeleteAsync(string id) => Request("contacts", id).DeleteAsync();
    }
}
