import { ocAppConfig } from '../config/app.config';
import { MarketplaceMeProduct } from 'marketplace';
import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';

export const getPrimaryImageUrl = (product: MarketplaceMeProduct): string => {
  return `${ocAppConfig.middlewareUrl}/assets/products/${product.ID}/thumbnail?size=M`;
};

export const getPrimaryLineItemImage = (lineItemID: string, lineItems: MarketplaceLineItem[]): string => {
  const li = lineItems.find(item => item.ID === lineItemID);
  return getPrimaryImageUrl(li.Product);
};
