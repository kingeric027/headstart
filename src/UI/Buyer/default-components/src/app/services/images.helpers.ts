import { map as _map, without as _without, uniqBy as _uniq } from 'lodash';
import { ocAppConfig } from '../config/app.config';
import { MarketplaceMeProduct } from 'marketplace';
import { MarketplaceSDK, Asset } from '../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';

// TODO - change when cms project is more stable
export const getImageUrls = async (product: MarketplaceMeProduct): Promise<string[]> => {
  // let images = ((product as any)?.xp?.Images) || [];
  // images = _uniq(images, (img: any) => img.URL);
  // const images = await MarketplaceSDK.ProductContents.ListAssets(product.ID);
  const images = { Items: [] };
  let urls: string[] = images.Items.map(img => {
    return img.Url;
  });
  urls = _without(urls, undefined);
  if (urls.length === 0) urls.push('http://placehold.it/300x300');
  return urls;
};

export const getPrimaryImageUrl = (product: MarketplaceMeProduct): string => {
  return `${ocAppConfig.middlewareUrl}/products/${product.ID}/image`;
  // return getImageUrls(product)[0];
};
