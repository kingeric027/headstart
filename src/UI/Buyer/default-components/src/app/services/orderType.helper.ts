import { OrderType, Order } from 'marketplace';

export const isQuoteOrder = (order?: Order): boolean => {
  return order.xp.OrderType === OrderType.Quote;
};
