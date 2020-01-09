import { Product } from '@ordercloud/angular-sdk';

export const IMAGE_HOST_URL = 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product';
export const PLACEHOLDER_URL = 'http://placehold.it/300x300';
export const PRODUCT_IMAGE_PATH_STRATEGY = 'PRODUCT_IMAGE_PATH_STRATEGY';

export function getProductMainImageUrlOrPlaceholder(product: Product) {
  const imgUrls = getProductImageUrls(product);
  return imgUrls.length ? imgUrls[0] : PLACEHOLDER_URL;
}

export function getProductImageUrls(product: Product): string[] {
  const images = (product.xp && product.xp.Images) || [];
  const result = images.map(img => {
    return img.Url.replace('{u}', IMAGE_HOST_URL);
  });

  //remove undefined values
  return result.filter(r => r);
}
