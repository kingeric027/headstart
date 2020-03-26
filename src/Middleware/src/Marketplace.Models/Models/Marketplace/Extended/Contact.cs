
using Marketplace.Helpers.Attributes;

namespace Marketplace.Models.Marketplace.Extended
{
    [SwaggerModel]
    public class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
