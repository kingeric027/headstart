import { Product } from '@ordercloud/angular-sdk';
import { MarketPlaceProductImage } from '@app-seller/shared/models/MarketPlaceProduct.interface';

export const IMAGE_HOST_URL = 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product';
export const PLACEHOLDER_URL = 'http://placehold.it/300x300';
export const PRODUCT_IMAGE_PATH_STRATEGY = 'PRODUCT_IMAGE_PATH_STRATEGY';

export function getProductMainImageUrlOrPlaceholder(product: Product): string {
  const imgUrls = getProductImageUrls(product);
  return imgUrls.length ? imgUrls[0] : PLACEHOLDER_URL;
}

export function ReplaceHostUrls(product: Product): MarketPlaceProductImage[] {
  const images = (product && product.xp && product.xp.Images) || [];
  return images.map(img => ReplaceHostUrl(img));
}

function getProductImageUrls(product: Product): string[] {
  return ReplaceHostUrls(product)
    .map(image => image.URL)
    .filter(url => url);
}

function ReplaceHostUrl(img: MarketPlaceProductImage): MarketPlaceProductImage {
  return { ...img, URL: img.URL.replace('{u}', IMAGE_HOST_URL) };
}
