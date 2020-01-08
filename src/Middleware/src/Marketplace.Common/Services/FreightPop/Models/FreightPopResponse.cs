using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.FreightPop
{
	public class FreightPopResponse<TData>
	{
		public int Code { get; set; }
		public string Message { get; set; }
		public TData Data { get; set; }
	}
}
