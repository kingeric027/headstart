using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Marketplace.Helpers.PipeWrench
{
	public static class Pipeline
	{
		public static PartialPipeline<TInput, TInput> Create<TInput>() {
			return new PartialPipeline<TInput, TInput>();
		}

		public static DataflowLinkOptions DefaultLinkOptions { get; } = new DataflowLinkOptions { PropagateCompletion = true };
	}

	public abstract class PipelineBase<TInput>
	{
		public IInputSegment<TInput> EntrySegment { get; protected set; }
		public PipelineDiagnostics Diagnostics { get; protected set; }
		public Func<string, object, Exception, Task> HandleError { get; protected set; }

		protected PipelineBase(PipelineBase<TInput> from, IPipelineSegment lastSegment) {
			EntrySegment = from?.EntrySegment ?? lastSegment as IInputSegment<TInput>;
			Diagnostics = from?.Diagnostics ?? new PipelineDiagnostics(Segments, TimeSpan.FromSeconds(1));
			HandleError = from?.HandleError;
		}

		public IEnumerable<IPipelineSegment> Segments {
			get {
				for (var segment = EntrySegment as IPipelineSegment; segment != null; segment = segment.Next)
					yield return segment;
			}
		}
	}

	public class Pipeline<TInput> : PipelineBase<TInput>
	{
		public Pipeline(PipelineBase<TInput> from, IPipelineSegment lastSegment) : base(from, lastSegment) { }

		public async Task FeedAsync(TInput item) {
			await EntrySegment.Block.SendAsync(item);
		}

		public async Task FeedAsync(IEnumerable<TInput> items) {
			foreach (var item in items)
				await EntrySegment.Block.SendAsync(item);
		}

        /// <summary>
        /// Signals for the first block to complete and awaits completion of the final block.
        /// </summary>
        public Task CompleteAsync() {
			EntrySegment.Block.Complete();
			return Task.WhenAll(Segments.Select(segment => segment.Block.Completion));
		}

		public async Task FeedAndCompleteAsync(IEnumerable<TInput> items) {
			await FeedAsync(items);
			await CompleteAsync();
		}

        //public async Task FeedAndCompleteAsync(IEnumerable<IGrouping<string, TInput>> inventoryGrouped)
        //{
        //    var grouped = inventoryGrouped.ToList();
        //    for (var index = 0; index <= grouped.Count() - 1; index++)
        //    {
        //        var item = grouped[index];
        //        await FeedAsync(item.ToList());
        //    }

        //    await CompleteAsync();
        //}
    }
}
