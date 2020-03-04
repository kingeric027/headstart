using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceBuyerLocation
    {
        public MarketplaceBuyerUserGroup UserGroup { get; set; }
        public MarketplaceBuyerAddress Address { get; set; }
    }
    public class MarketplaceBuyerUserGroup : UserGroup<MarketplaceUserGroupXp>, IMarketplaceObject
    {

    }
    public class MarketplaceBuyerAddress : Address<MarketplaceBuyerAddressXp>, IMarketplaceObject
    {
        
    }
    public class MarketplaceUserGroupXp
    {
        public string Type { get; set; }
    }
    public class MarketplaceBuyerAddressXp
    {
        public string Email { get; set; }
    }
}
