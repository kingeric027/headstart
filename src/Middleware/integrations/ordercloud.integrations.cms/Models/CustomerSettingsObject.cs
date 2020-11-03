using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms.Models
{
	public class CustomerSettingsObject : CosmosObject
	{
		public string SinglePartitionID { get; set; }
		public List<Customer> Customers { get; set; }
	}

	public class Customer
	{
		public string SellerID { get; set; }
		public string Name { get; set; }
		public AssetPartitionStrategy AssetPartitionStrategy { get; set; }
	}

	public enum AssetPartitionStrategy
	{
		PartitionBySellerID = 0,
		PartitionByCompanyID = 1
	}
}
