using Marketplace.Common.Services.SmartyStreets.models;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
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


