using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Exceptions
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
