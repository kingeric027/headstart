using System;
using System.Threading.Tasks.Dataflow;

namespace Marketplace.Helpers.PipeWrench
{
	public static class DataflowExtensions
	{
		public static int InputBufferSize(this IDataflowBlock block) {
			var prop = block.GetType().GetProperty("InputCount") ?? block.GetType().GetProperty("Count");
			return (int)(prop?.GetValue(block) ?? 0);
		}

		public static int OutputBufferSize(this IDataflowBlock block) {
			var prop = block.GetType().GetProperty("OutputCount") ?? block.GetType().GetProperty("Count");
			return (int)(prop?.GetValue(block) ?? 0);
		}
	}
}
