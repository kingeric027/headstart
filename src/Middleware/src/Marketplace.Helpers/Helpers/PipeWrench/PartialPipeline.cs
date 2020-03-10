using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Marketplace.Helpers.PipeWrench
{
	/// <summary>
	/// Represents a pipeline under construction. A pipeline is not "complete" until a terminating segment is added,
	/// i.e. one with no output.
	/// </summary>
	public class PartialPipeline<TInput, TOutput> : PipelineBase<TInput>
	{
		public IOutputSegment<TOutput> LastSegment { get; }

		public PartialPipeline() : this(null, null) { }

		private PartialPipeline(PipelineBase<TInput> from, IOutputSegment<TOutput> lastSegment) : base(from, lastSegment as IPipelineSegment) {
			LastSegment = lastSegment;
		}

		public PartialPipeline<TInput, TOutput> OnError(Func<string, object, Exception, Task> handle) {
			HandleError = handle;
			return this;
		}

		// link to ActionBlock
		public Pipeline<TInput> LinkTo(string name, Action<TOutput> action, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded) {
			return LinkTo(name, x => {
				action(x);
				return Task.CompletedTask;
			}, maxDegreeOfParallelism, boundedCapacity);
		}

		// link to ActionBlock async
		public Pipeline<TInput> LinkTo(string name, Func<TOutput, Task> action, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded) {
			var opts = new ExecutionDataflowBlockOptions { BoundedCapacity = boundedCapacity, MaxDegreeOfParallelism = maxDegreeOfParallelism };
			return LinkTo(PipelineSegment.FromAction(name, Diagnostics, HandleError, action, f => new ActionBlock<TOutput>(f, opts)));
		}

        // link to TransformBlock
        public PartialPipeline<TInput, TNext> LinkTo<TNext>(string name, Func<TOutput, TNext> propagate, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded) {
			return LinkTo(name, x => Task.FromResult(propagate(x)), maxDegreeOfParallelism, boundedCapacity);
		}

		// link to TransformBlock async
		public PartialPipeline<TInput, TNext> LinkTo<TNext>(string name, Func<TOutput, Task<TNext>> propagate, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded) {
			var opts = new ExecutionDataflowBlockOptions { BoundedCapacity = boundedCapacity, MaxDegreeOfParallelism = maxDegreeOfParallelism };
			return LinkTo(PipelineSegment.FromFunc(name, Diagnostics, HandleError, propagate, f => new TransformBlock<TOutput, TNext>(f, opts)));
		}

		// link to TransformManyBlock
		public PartialPipeline<TInput, TNext> LinkToMany<TNext>(string name, Func<TOutput, IEnumerable<TNext>> propagate, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded) {
			return LinkToMany(name, x => Task.FromResult(propagate(x)), maxDegreeOfParallelism, boundedCapacity);
		}

		// link to TransformManyBlock async
		public PartialPipeline<TInput, TNext> LinkToMany<TNext>(string name, Func<TOutput, Task<IEnumerable<TNext>>> propagate, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded) {
			var opts = new ExecutionDataflowBlockOptions { BoundedCapacity = boundedCapacity, MaxDegreeOfParallelism = maxDegreeOfParallelism };
			return LinkTo(PipelineSegment.FromFunc(name, Diagnostics, HandleError, propagate, f => new TransformManyBlock<TOutput, TNext>(f, opts)));
		}

		// link to BatchBlock
		public PartialPipeline<TInput, IEnumerable<TOutput>> Batch(string name, int batchSize, int boundedCapacity = DataflowBlockOptions.Unbounded) {
			var opts = new GroupingDataflowBlockOptions() { BoundedCapacity = boundedCapacity };
			return LinkTo(new PipelineSegment<TOutput, IEnumerable<TOutput>>()
			{
				Name = "batch",
				Block = new BatchBlock<TOutput>(batchSize, opts)
			});
		}

		public PartialPipeline<TInput, TOutput> Filter(string name, Func<TOutput, bool> condition) {
			return LinkToMany(name, x => condition(x) ? new[] { x } : Enumerable.Empty<TOutput>());
		}

        #region LinkTo with name inferrence
        // link to ActionBlock
        public Pipeline<TInput> LinkTo(Action<TOutput> action, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded)
        {
            return LinkTo(InferName(action), action, maxDegreeOfParallelism, boundedCapacity);
        }

        // link to ActionBlock async

        public Pipeline<TInput> LinkTo(Func<TOutput, Task> action, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded)
        {
            return LinkTo(InferName(action), action, maxDegreeOfParallelism, boundedCapacity);
        }

        // link to TransformBlock
        public PartialPipeline<TInput, TNext> LinkTo<TNext>(Func<TOutput, TNext> propagate, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded)
        {
            return LinkTo(InferName(propagate), x => Task.FromResult(propagate(x)), maxDegreeOfParallelism, boundedCapacity);
        }

        // link to TransformBlock async
        public PartialPipeline<TInput, TNext> LinkTo<TNext>(Func<TOutput, Task<TNext>> propagate, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded) {
			return LinkTo(InferName(propagate), propagate, maxDegreeOfParallelism, boundedCapacity);
		}

        // link to TransformManyBlock
        public PartialPipeline<TInput, TNext> LinkToMany<TNext>(Func<TOutput, IEnumerable<TNext>> propagate, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded)
        {
            return LinkToMany(InferName(propagate), propagate, maxDegreeOfParallelism, boundedCapacity);
        }

        // link to TransformManyBlock async
        public PartialPipeline<TInput, TNext> LinkToMany<TNext>(Func<TOutput, Task<IEnumerable<TNext>>> propagate, int maxDegreeOfParallelism = 1, int boundedCapacity = DataflowBlockOptions.Unbounded)
        {
            return LinkToMany(InferName(propagate), propagate, maxDegreeOfParallelism, boundedCapacity);
        }

        // link to BatchBlock
        public PartialPipeline<TInput, IEnumerable<TOutput>> Batch(int batchSize, int boundedCapacity = DataflowBlockOptions.Unbounded)
        {
	        return Batch("batch", batchSize, boundedCapacity);
        }

		public PartialPipeline<TInput, TOutput> Filter(Func<TOutput, bool> condition) {
			return Filter(InferName(condition), condition);
		}

		private string InferName(Delegate d) {
			var name = d.Method.Name;
			return name;
		}
		#endregion

		private Pipeline<TInput> LinkTo(PipelineSegment<TOutput> segment) {
			LastSegment?.LinkTo(segment);
			return new Pipeline<TInput>(this, segment);
		}

		private PartialPipeline<TInput, TNext> LinkTo<TNext>(PipelineSegment<TOutput, TNext> segment) {
			LastSegment?.LinkTo(segment);
			return new PartialPipeline<TInput, TNext>(this, segment);
		}
	}
}
