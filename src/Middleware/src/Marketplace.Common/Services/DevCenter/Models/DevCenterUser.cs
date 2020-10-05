namespace Marketplace.Common.Services.DevCenter.Models
{
    public class DevCenterUser
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public bool CanCreateProductionOrgs { get; set; }
    }
}
