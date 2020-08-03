import { Product } from '@ordercloud/angular-sdk';
import { environment } from 'src/environments/environment';

export const IMAGE_HOST_URL = 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product';
export const PLACEHOLDER_URL = 'http://placehold.it/300x300';
export const PRODUCT_IMAGE_PATH_STRATEGY = 'PRODUCT_IMAGE_PATH_STRATEGY';

export function getProductMainImageUrlOrPlaceholder(product: Product): string {
  return `${environment.middlewareUrl}/assets/resource/primary-image?ResourceID=${product.ID}&ResourceType=Products`;
}
