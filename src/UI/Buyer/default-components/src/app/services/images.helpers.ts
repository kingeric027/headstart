import { ocAppConfig } from '../config/app.config';
import { MarketplaceMeProduct } from 'marketplace';

export const getPrimaryImageUrl = (product: MarketplaceMeProduct): string => {
  return `${ocAppConfig.middlewareUrl}/products/${product.ID}/image`;
};
