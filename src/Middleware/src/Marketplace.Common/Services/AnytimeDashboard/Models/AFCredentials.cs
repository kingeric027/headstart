using System;
using System.Collections.Generic;
using System.Text;

namespace Headstart.Common.Services.AnytimeDashboard.Models
{
	// This is basically equivallent to the response from /me in OC.
	// I need to use it for SSO to get a username from the bearer token. 
	// So basically the only field I use right now is username
	public class AFCredentials
	{
		public string username { get; set; }
		public int authId { get; set; }
		public string authGuid { get; set; }
		public CredentialsUserType userType { get; set; }
		public IEnumerable<CredentialsRole> roles { get; set; }
		public IEnumerable<CredentialsClub> clubs { get; set; }
	}

	public class CredentialsUserType
	{
		public int userTypeId { get; set; }
		public string name { get; set; }
	}

	public class CredentialsRole
	{
		public string id { get; set; }
		public string name { get; set; }
		public int applicationId { get; set; }
		public string category { get; set; }
	}

	public class CredentialsClub
	{
		public string guid { get; set; }
		public string name { get; set; }
		public string afNumber { get; set; }
		public string billingNumber { get; set; }
		public int franchiseDbId { get; set; }
	}
}
