using System.Collections.Generic;

namespace Marketplace.Models.Exceptions
{
	public class MissingShippingSelectionError
	{
		public MissingShippingSelectionError(IEnumerable<string> ids)
		{
			ShipFromAddressIDsRequiringAttention = ids;
		}

		public IEnumerable<string> ShipFromAddressIDsRequiringAttention;
	}
}
