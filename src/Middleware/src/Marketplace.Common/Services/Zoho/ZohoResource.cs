using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Services.Zoho
{
    public abstract class ZohoResource
    {
        private readonly ZohoClient _client;
        private readonly string _resource;
        private readonly object[] _segments;

        protected ZohoResource(ZohoClient client, string resource, params object[] segments)
        {
            _client = client;
            _resource = resource;
            _segments = segments;
        }

        private object[] AppendSegments(params object[] segments)
        {
            if (segments.Length <= 0) return _segments;
            var appended = _segments.ToList();
            appended.AddRange(segments);
            return appended.ToArray();
        }

        protected internal IFlurlRequest Get(params object[] segments) => 
            _client.Request(this.AppendSegments(segments));

        protected internal IFlurlRequest Delete(params object[] segments)=> 
            _client.Request(this.AppendSegments(segments));

        protected internal async Task<T> Post<T>(object obj) => 
            await Parse<T>(await _client.Post(obj, _segments).PostAsync(null));

        protected internal async Task<T> Put<T>(object obj, params object[] segments) => 
            await Parse<T>(await _client.Put(obj, this.AppendSegments(segments)).PutAsync(null));

        private async Task<T> Parse<T>(IFlurlResponse res) =>
            JObject.Parse(await res.ResponseMessage.Content.ReadAsStringAsync()).SelectToken(_resource).ToObject<T>();
    }
}
