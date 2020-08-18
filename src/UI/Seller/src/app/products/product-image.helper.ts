import { Product, MeUser } from '@ordercloud/angular-sdk';
import { environment } from 'src/environments/environment';
import { MarketplaceProduct } from '@ordercloud/headstart-sdk';
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
