import { OrderType } from 'marketplace';
import { MarketplaceOrder, MarketplaceLineItem } from '@ordercloud/headstart-sdk';

export const isQuoteOrder = (order?: MarketplaceOrder): boolean => {
  return order.xp.OrderType === OrderType.Quote;
};

export const lineItemHasBeenShipped = (lineItem: MarketplaceLineItem): boolean => {
  return ['Complete', 'Returned', 'ReturnRequested'].includes(lineItem.xp.LineItemStatus);
}
