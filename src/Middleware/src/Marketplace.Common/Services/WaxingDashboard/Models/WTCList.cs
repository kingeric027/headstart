using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.WaxingDashboard.Models
{
	public class WTCList<T>
	{
		public int pageNumber { get; set; }
		public int pageSize { get; set; }
		public bool hasMore { get; set; }
		public int? total { get; set; }
		public List<T> items { get; set; }
		public int itemsCount { get; set; }
		public string type { get; set; }
		public bool hasError { get; set; }
	}

	// Used when you get a single location
	public class WTCResponse<T> 
	{
		public List<T> items { get; set; }
		public int itemsCount { get; set; }
		public string type { get; set; }
		public bool hasError { get; set; }
	}
}
