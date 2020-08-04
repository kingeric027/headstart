import { OrderType } from 'marketplace';
import { MarketplaceOrder } from '@ordercloud/headstart-sdk';

export const isQuoteOrder = (order?: MarketplaceOrder): boolean => {
  return order.xp.OrderType === OrderType.Quote;
};