namespace Marketplace.Common.Services.FreightPop.Models
{
	public class FreightPopResponse<TData>
	{
		public int Code { get; set; }
		public string Message { get; set; }
		public TData Data { get; set; }
	}
}
