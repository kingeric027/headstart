using System.ComponentModel.DataAnnotations;
using ordercloud.integrations.openapispec;

namespace Marketplace.Models.Extended
{
    [SwaggerModel]
    public class StaticContent
    {
        [Required]
        public string URL { get; set; }
        [Required]
        public string Title { get; set; }
        public bool Active { get; set; }
        public string Filename { get; set; }
    }
}
