using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marketplace.Models.Orchestration
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum LogLevel { Progress, Error, Warn, Success }
}
