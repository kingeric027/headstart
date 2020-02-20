using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services
{
	public interface ISmartyStreetsService
	{
		List<Address> GetSuggestedAddresses(Address address);
	}

	public class SmartyStreetsService : ISmartyStreetsService
	{
		public  List<Address> GetSuggestedAddresses(Address address)
		{
			var mock = new List<Address> {
				new Address() {
					Street1 = "110 N St",
					Street2 = "#300",
					City = "Minneapolis",
					State = "MN",
					Zip = "55403",
					Country = "US"
				},
				new Address() {
					Street1 = "110 N St",
					Street2 = "#400",
					City = "Minneapolis",
					State = "MN",
					Zip = "55403",
					Country = "US"
				},
				new Address() {
					Street1 = "110 N St",
					Street2 = "#500",
					City = "Minneapolis",
					State = "MN",
					Zip = "55403",
					Country = "US"
				}
			};
			return mock;
		}
	}
}
