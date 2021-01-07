import { MarketplaceOrder } from '@ordercloud/headstart-sdk'
import { OrderType } from '../models/order.types'

export const isQuoteOrder = (order?: MarketplaceOrder): boolean => {
  return order.xp.OrderType === OrderType.Quote
}
