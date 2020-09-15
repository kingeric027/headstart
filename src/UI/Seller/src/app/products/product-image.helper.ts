import { Product, MeUser, User } from '@ordercloud/angular-sdk';
import { environment } from 'src/environments/environment';
import { MarketplaceProduct, MarketplaceLineItem } from '@ordercloud/headstart-sdk';
import { UserContext } from '@app-seller/config/user-context';

export const IMAGE_HOST_URL = 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product';
export const PLACEHOLDER_URL = 'http://placehold.it/300x300';
export const PRODUCT_IMAGE_PATH_STRATEGY = 'PRODUCT_IMAGE_PATH_STRATEGY';

export function getProductSmallImageUrl(product: MarketplaceProduct, sellerID: string): string {
  return `${environment.middlewareUrl}/assets/${sellerID}/products/${product.ID}/thumbnail?size=s`;
}

export function getProductMediumImageUrl(product: MarketplaceProduct, sellerID: string): string {
  return `${environment.middlewareUrl}/assets/${sellerID}/products/${product.ID}/thumbnail?size=m`;
}


export const getPrimaryLineItemImage = (
  lineItemID: string,
  lineItems: MarketplaceLineItem[],
  sellerID: string
): string => {
  const li = lineItems.find(item => item.ID === lineItemID);
  return getProductMediumImageUrl(li.Product, sellerID);
};