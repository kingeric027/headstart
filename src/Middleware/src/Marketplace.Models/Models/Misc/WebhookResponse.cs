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

	public class AddressValidationWebhookResponse : WebhookResponse<AddressValidation> {
		public AddressValidationWebhookResponse(AddressValidation validation) : base(validation)
		{
			proceed = validation.IsRawAddressValid;
			Body = validation;
			Message = "Address not found. Did you mean one of these addresses?";
		}
	}
}


