import { MarketplaceOrder } from '@ordercloud/headstart-sdk'
import { OrderType } from '../shopper-context'

export const isQuoteOrder = (order?: MarketplaceOrder): boolean => {
  return order.xp.OrderType === OrderType.Quote
}
