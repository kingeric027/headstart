using Marketplace.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Models.Models.Misc
{
	public class TaxCertificate
	{
		public string ID { get; set; }
		[Required]
		public DateTimeOffset SignedDate { get; set; }
		[Required]
		public DateTimeOffset ExpirationDate { get; set; }
		[ApiReadOnly]
		public bool Expired => ExpirationDate > DateTimeOffset.Now;
		[Required]
		public string State { get; set; }
		[Required]
		public string Base64UrlEncodedPDF { get; set; }
		public string ExemptionNumber { get; set; }
	}
}
