import { OrderType, Order } from "marketplace"

export const isQuoteOrder = (order?: Order) => {
    return order.xp.OrderType === OrderType.Quote;
}