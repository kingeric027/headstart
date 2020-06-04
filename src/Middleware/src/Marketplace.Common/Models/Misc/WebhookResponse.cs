using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
    public class WebhookResponse<T> : WebhookResponse
    {
        public string Message { get; set; }
		public T Body { get; set; }

		public WebhookResponse(T body)
		{
			Body = body;
			Message = "Unspecified error in webhook";
		}
	}
}


