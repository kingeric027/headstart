import { get as _get, map as _map, without as _without, uniqBy as _uniq, } from 'lodash';
import { ocAppConfig } from '../config/app.config';
import { Product } from 'marketplace';

export const getImageUrls = (product: Product): string[] => {
  let images = (product.xp && product.xp.Images) || [];
  images = _uniq(images, ((img: any) => img.Url));
  let urls: string[] = _map(images, img => {
    return img.Url.replace('{u}', ocAppConfig.cmsUrl);
  });
  urls = _without(urls, undefined);
  if (urls.length === 0) urls.push('http://placehold.it/300x300');
  return urls; 
};

export const getPrimaryImageUrl = (product: Product): string => {
  return getImageUrls(product)[0];
};
