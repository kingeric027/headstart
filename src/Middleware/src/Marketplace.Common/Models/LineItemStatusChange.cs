using Marketplace.Models.Extended;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Models.Models.Marketplace
{
	[SwaggerModel]
	public class LineItemStatusChange
	{
		public List<string> LineItemIDs { get; set; }
		public LineItemStatus LineItemStatus { get; set; }
	}
}
