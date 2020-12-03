using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class AssetContainerMapper
	{
		public static AssetContainer MapTo(AssetContainerDO dataObject, Customer customer)
		{
			return new AssetContainer()
			{
				id = dataObject.id,
				SupplierID = dataObject.SupplierID,
				BuyerID = dataObject.BuyerID,
				Customer = customer
			};
		}
	}
}
