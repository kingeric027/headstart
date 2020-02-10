using System.ComponentModel.DataAnnotations;

namespace Marketplace.Models.Extended
{
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
