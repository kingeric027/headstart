using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum LogLevel { Progress, Error, Warn, Success }
}
