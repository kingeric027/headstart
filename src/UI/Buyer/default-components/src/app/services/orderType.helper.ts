import { OrderType } from 'marketplace';
import { MarketplaceOrder } from 'marketplace-javascript-sdk';

export const isQuoteOrder = (order?: MarketplaceOrder): boolean => {
  return order.xp.OrderType === OrderType.Quote;
};
