using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.library;

namespace Marketplace.Models.Extended
{
    [SwaggerModel]
    public class SpecUI
    {
        public ControlType ControlType { get; set; } = ControlType.Text;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ControlType
    {
        Text, DropDown, Checkbox, Range
    }
}
