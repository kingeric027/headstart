import { ocAppConfig } from '../config/app.config';
import { MarketplaceMeProduct, CurrentUser } from 'marketplace';
import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';

export const getPrimaryImageUrl = (product: MarketplaceMeProduct, user: CurrentUser): string => {
  return `${ocAppConfig.middlewareUrl}/assets/${user.Seller.ID}/products/${product.ID}/thumbnail?size=M`;
};

export const getPrimaryLineItemImage = (
  lineItemID: string,
  lineItems: MarketplaceLineItem[],
  user: CurrentUser
): string => {
  const li = lineItems.find(item => item.ID === lineItemID);
  return getPrimaryImageUrl(li.Product, user);
};
