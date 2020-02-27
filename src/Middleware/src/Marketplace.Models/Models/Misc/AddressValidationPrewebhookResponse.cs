using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
    public class PrewebhookResponseWithError : WebhookResponse
    {
        // in the future consider a more robust error response format
        // this could affect front end displays of errors from prewebhooks
        public string body { get; set; }
    }
}


