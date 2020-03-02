import { map as _map, without as _without, uniqBy as _uniq } from 'lodash';
import { ocAppConfig } from '../config/app.config';
import { MarketplaceProduct } from 'marketplace';

export const getImageUrls = (product: MarketplaceProduct): string[] => {
  let images = (product?.xp?.Images) || [];
  images = _uniq(images, (img: any) => img.URL);
  let urls: string[] = _map(images, img => {
    if (!img.URL) return;
    return img.URL.replace('{u}', ocAppConfig.cmsUrl);
  });
  urls = _without(urls, undefined);
  if (urls.length === 0) urls.push('http://placehold.it/300x300');
  return urls;
};

export const getPrimaryImageUrl = (product: MarketplaceProduct): string => {
  return getImageUrls(product)[0];
};
