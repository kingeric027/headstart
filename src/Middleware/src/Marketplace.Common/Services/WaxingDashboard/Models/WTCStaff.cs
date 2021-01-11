using System;
using System.Collections.Generic;
using System.Text;

namespace Headstart.Common.Services.WaxingDashboard.Models
{
	public class WTCStaff
	{
		public int id { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string email { get; set; }
		public string userType { get; set; } // Corporate, Owner, Manager, Staff, Studio Coordinator, Cerologist, User
		public string status { get; set; } // Active, Inactive
		public DateTime createdDateTime { get; set; }
		public DateTime updatedDateTime { get; set; }
	}
}
