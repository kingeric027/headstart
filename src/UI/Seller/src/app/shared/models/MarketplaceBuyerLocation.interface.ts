import { UserGroup, BuyerAddress } from '@ordercloud/angular-sdk';

export interface MarketplaceBuyerLocation {
  UserGroup: UserGroup<MarketplaceUserGroupXp>;
  Address: BuyerAddress<MarketplaceAddressXp>;
}

interface MarketplaceUserGroupXp {
  Type: string;
}

interface MarketplaceAddressXp {
  Email: string;
}
