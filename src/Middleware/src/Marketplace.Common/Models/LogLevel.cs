using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marketplace.Common.Models
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum LogLevel { Progress, Error, Warn, Success }
}
