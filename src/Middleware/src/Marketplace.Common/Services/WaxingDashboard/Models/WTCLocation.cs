using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.WaxingDashboard.Models
{
	public class WTCStudio
	{
		public string locationNumber { get; set; }
		public string locationName { get; set; }
		public string address1 { get; set; }
		public string address2 { get; set; }
		public string city { get; set; }
		public string state { get; set; }
		public string country { get; set; }
		public string postCode { get; set; }
		public string email { get; set; }
		public string phoneNumber { get; set; }
		public DateTime? openingDate { get; set; }
		public string primaryContactName { get; set; }
		public string legalEntity { get; set; }
		public string status { get; set; } // Active, Inactive
		public string openStatus { get; set; } // Open, Coming Soon, Now Booking, Unknown
	}
}
