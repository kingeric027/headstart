namespace Marketplace.Common.Services.DevCenter.Models
{
    public class Organization
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DevCenterUser Owner { get; set; }
        public string Environment { get; set; }
    }
}
