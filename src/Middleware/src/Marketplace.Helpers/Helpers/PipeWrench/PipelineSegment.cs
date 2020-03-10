using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Polly;

namespace Marketplace.Helpers.PipeWrench
{
	public static class PipelineSegment
	{
		private static Policy _retyPolicy = Policy
			.Handle<Exception>()
			.WaitAndRetryAsync(1, i => TimeSpan.FromSeconds(2));

		public static PipelineSegment<TInput> FromAction<TInput>(string name, PipelineDiagnostics diagnostics, Func<string, object, Exception, Task> handleError, Func<TInput, Task> action, Func<Func<TInput, Task>, ITargetBlock<TInput>> buildBlock) {
			return new PipelineSegment<TInput> {
				Name = name,
				Block = buildBlock(x => ExecAsync(name, diagnostics, handleError, x, async x2 => {
					await action(x2);
					return 0;
				}))
			};
		}

		public static PipelineSegment<TInput, TOutput> FromFunc<TInput, TIntermediate, TOutput>(string name, PipelineDiagnostics diagnostics, Func<string, object, Exception, Task> handleError, Func<TInput, Task<TIntermediate>> func, Func<Func<TInput, Task<TIntermediate>>, IPropagatorBlock<TInput, TOutput>> buildBlock) {
			return new PipelineSegment<TInput, TOutput> {
				Name = name,
				Block = buildBlock(x => ExecAsync(name, diagnostics, handleError, x, func))
			};
		}

		private static async Task<TOutput> ExecAsync<TInput, TOutput>(string name, PipelineDiagnostics diagnostics, Func<string, object, Exception, Task> handleError, TInput message, Func<TInput, Task<TOutput>> innerFunc) {
			if (message == null)
				return default(TOutput);

			var start = diagnostics.ReportWorkerStarted(name);
			try {
				return await _retyPolicy.ExecuteAsync(() => innerFunc(message));
			}
			catch (Exception ex) {
				if (handleError != null)
					await handleError(name, message, ex);
				return default(TOutput);
			}
			finally {
				diagnostics.ReportWorkerFinished(name, start);
			}
		}
	}

	public interface IPipelineSegment
	{
		string Name { get; set; }
		IDataflowBlock Block { get; }
		IPipelineSegment Next { get; }
	}

	public interface IInputSegment<TInput>
	{
		ITargetBlock<TInput> Block { get; }
	}

	public interface IOutputSegment<TOutput>
	{
		ISourceBlock<TOutput> Block { get; }
		IInputSegment<TOutput> Next { get; }
		void LinkTo(IInputSegment<TOutput> next);
	}

	public class PipelineSegment<TInput> : IPipelineSegment, IInputSegment<TInput>
	{
		public string Name { get; set; }
		public ITargetBlock<TInput> Block { get; set; }

		IDataflowBlock IPipelineSegment.Block => Block;
		IPipelineSegment IPipelineSegment.Next => null;
	}

	public class PipelineSegment<TInput, TOutput> : IPipelineSegment, IInputSegment<TInput>, IOutputSegment<TOutput>
	{
		public string Name { get; set; }
		public IInputSegment<TOutput> Next { get; set; }
		public IPropagatorBlock<TInput, TOutput> Block { get; set; }

		public void LinkTo(IInputSegment<TOutput> next) {
			Block.LinkTo(next.Block, Pipeline.DefaultLinkOptions);
			Next = next;
		}

		IDataflowBlock IPipelineSegment.Block => Block;
		IPipelineSegment IPipelineSegment.Next => Next as IPipelineSegment;

		ISourceBlock<TOutput> IOutputSegment<TOutput>.Block => Block;
		IInputSegment<TOutput> IOutputSegment<TOutput>.Next => Next;

		ITargetBlock<TInput> IInputSegment<TInput>.Block => Block;
	}
}
