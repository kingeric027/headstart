using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Exceptions
{
	public class MissingProductDimensionsError
	{
		public MissingProductDimensionsError(IEnumerable<string> ids)
		{
			ProductIDsRequiringAttention = ids;
		}

		public IEnumerable<string> ProductIDsRequiringAttention;
	}
}
