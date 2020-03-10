using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Marketplace.Helpers.PipeWrench
{
	public class PipelineDiagnostics
	{
		private readonly IEnumerable<IPipelineSegment> _segments;
		private readonly ConcurrentDictionary<string, int> _workers = new ConcurrentDictionary<string, int>();
		private Timer _timer;
		private readonly TimeSpan _sampleInterval;
		private bool _enabled;

		public ConcurrentDictionary<string, IList<long>> WorkerCounts { get; } = new ConcurrentDictionary<string, IList<long>>();
		public ConcurrentDictionary<string, IList<long>> Durations { get; } = new ConcurrentDictionary<string, IList<long>>();
		public ConcurrentDictionary<string, IList<long>> InputCounts { get; } = new ConcurrentDictionary<string, IList<long>>();
		public ConcurrentDictionary<string, IList<long>> OutputCounts { get; } = new ConcurrentDictionary<string, IList<long>>();

		public PipelineDiagnostics(IEnumerable<IPipelineSegment> segments, TimeSpan sampleInterval) {
			_segments = segments;
			_sampleInterval = sampleInterval;
		}

		public void Start() {
			_enabled = true;
			_timer = new Timer(_ => GatherStats(), null, _sampleInterval, _sampleInterval);
		}

		private void GatherStats() {
			//Console.WriteLine($"Gathering stats for {_blocks.Count} blocks");
			foreach (var segment in _segments) {
				ReportWorkerCount(segment.Name, _workers.TryGetValue(segment.Name, out var count) ? count : 0);
				ReportInputCount(segment.Name, segment.Block.InputBufferSize());
				ReportOutputCount(segment.Name, segment.Block.OutputBufferSize());
			}
		}

		internal DateTime ReportWorkerStarted(string segment) {
			_workers.AddOrUpdate(segment, 1, (_, count) => count++);
			return DateTime.UtcNow;
		}

		internal void ReportWorkerFinished(string segment, DateTime startTime) {
			ReportDuration(segment, (long)(DateTime.UtcNow - startTime).TotalMilliseconds);
			_workers.AddOrUpdate(segment, 0, (_, count) => count--);
		}

		private void ReportDuration(string segment, long ms) => ReportStat(Durations, segment, ms);
		private void ReportWorkerCount(string segment, long count) => ReportStat(WorkerCounts, segment, count);
		private void ReportInputCount(string segment, long count) => ReportStat(InputCounts, segment, count);
		private void ReportOutputCount(string segment, long count) => ReportStat(OutputCounts, segment, count);

		private void ReportStat(ConcurrentDictionary<string, IList<long>> stats, string segment, long value) {
			if (!_enabled) return;

			stats.AddOrUpdate(segment, new List<long> { value }, (_, list) => {
				// TODO: saw an index out of bounds error here once, I think from list.Add. check thread safety
				list.Add(value);
				return list;
			});
		}

		public string Dump() {
			var sb = new StringBuilder();
			DumpStat(sb, "Average number of workers:", WorkerCounts);
			DumpStat(sb, "Average durations (ms):", Durations);
			DumpStat(sb, "Average input buffer size:", InputCounts);
			DumpStat(sb, "Average output buffer size:", OutputCounts);
			return sb.ToString();
		}

		private void DumpStat(StringBuilder sb, string label, IDictionary<string, IList<long>> stats) {
			sb.AppendLine().AppendLine(label);
			var averages =
				from d in stats
				let avg = d.Value.Average()
				orderby avg descending
				select new { segment = d.Key, avg };

			foreach (var stat in averages)
				sb.AppendLine($"   {stat.segment}: {stat.avg}");
		}
	}
}
