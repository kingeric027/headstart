using System.Collections.Generic;
using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
    public class PreWebhookError<T> : WebhookResponse
    {
        public string Message { get; set; }
		public T Body { get; set; }
	}

	public class AddressValidation
	{
		public List<Address> SuggestedValidAddresses { get; set; }
	}

	public class AddressValidationPreWebhookError : PreWebhookError<AddressValidation> {
		public AddressValidationPreWebhookError(List<Address> suggestedAddresses)
		{
			proceed = false;
			Message = "Address not found. Did you mean one of these addresses?";
			Body = new AddressValidation() { SuggestedValidAddresses = suggestedAddresses };
		}
	}
}


