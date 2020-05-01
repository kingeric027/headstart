using Avalara.AvaTax.RestClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Integrations.Avalara
{
	public class AvalaraConfig
	{
		public AvaTaxEnvironment Env { get; set; }
		public int AccountID { get; set; }
		public string LicenseKey { get; set; }
		public string CompanyCode { get; set; }
	}
}
