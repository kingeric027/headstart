import {
  MarketplaceOrder,
  MarketplaceLineItem,
  OrderPromotion,
} from '@ordercloud/headstart-sdk'
import { ShipEstimate, LineItem } from 'ordercloud-javascript-sdk'
import { concatAll } from 'rxjs/operators'

export interface OrderSummaryMeta {
  StandardLineItems: MarketplaceLineItem[]
  POLineItems: MarketplaceLineItem[]
  StandardLineItemCount: number
  POLineItemCount: number

  ShouldHideShippingAndText: boolean
  ShippingAndTaxOverrideText: string

  // with no purchase order these are displayed as the whole order
  CreditCardDisplaySubtotal: number
  ShippingCost: number
  TaxCost: number
  CreditCardTotal: number
  DiscountTotal: number

  POSubtotal: number
  POShippingCost: number
  POTotal: number
  OrderTotal: number
}

const getPurchaseOrderLineItems = (
  lineItems: MarketplaceLineItem[]
): MarketplaceLineItem[] => {
  return lineItems.filter(
    (li) => li.Product.xp?.ProductType === 'PurchaseOrder'
  )
}

const getStandardLineItems = (
  lineItems: MarketplaceLineItem[]
): MarketplaceLineItem[] => {
  return lineItems.filter(
    (li) => !(li.Product.xp?.ProductType === 'PurchaseOrder')
  )
}

const getOverrideText = (checkoutPanel: string): string => {
  /* if there is override text for shipping and tax
   * we show that and calculate the order total differently */
  switch (checkoutPanel) {
    case 'cart':
      return 'Calculated during checkout'
    case 'shippingAddress':
    case 'shippingSelection':
      return 'Pending selections'
    default:
      return ''
  }
}

const getCreditCardTotal = (
  subTotal: number,
  shippingCost: number,
  taxCost: number,
  shouldHideShippingAndText: boolean,
  discountTotal: number
): number => {
  if (shouldHideShippingAndText) {
    return subTotal - discountTotal
  } else {
    return subTotal + shippingCost + taxCost - discountTotal
  }
}

/* eslint-disable */
export const getOrderSummaryMeta = (
  order: MarketplaceOrder,
  orderPromos: OrderPromotion[],
  lineItems: MarketplaceLineItem[],
  shipEstimates: ShipEstimate[],
  checkoutPanel: string,
): OrderSummaryMeta => {
  const StandardLineItems = getStandardLineItems(lineItems);
  const POLineItems = getPurchaseOrderLineItems(lineItems);

  const ShippingAndTaxOverrideText = getOverrideText(checkoutPanel);
  const shouldHideShippingAndText = !!ShippingAndTaxOverrideText;

  const CreditCardDisplaySubtotal = StandardLineItems.reduce((accumulator, li) => (li.Quantity * li.UnitPrice) + accumulator, 0);
  const DiscountTotal = orderPromos.reduce((accumulator, promo) => (promo.Amount) + accumulator, 0);

  const POSubtotal = POLineItems.reduce((accumulator, li) => (li.Quantity * li.UnitPrice) + accumulator, 0);

  const POShippingCost = getPOShippingCost(shipEstimates, POLineItems);
  const ShippingCost = order.ShippingCost - POShippingCost;

  const POTotal = POSubtotal + POShippingCost;

  const CreditCardTotal = getCreditCardTotal(CreditCardDisplaySubtotal, ShippingCost, order.TaxCost, shouldHideShippingAndText, DiscountTotal);
  const OrderTotal = POTotal + CreditCardTotal;

  return {
    StandardLineItemCount: StandardLineItems.length,
    StandardLineItems,
    POLineItemCount: POLineItems.length,
    POLineItems,
    ShippingAndTaxOverrideText,
    ShouldHideShippingAndText: shouldHideShippingAndText,
    CreditCardDisplaySubtotal,
    POShippingCost,
    ShippingCost: ShippingCost,
    TaxCost: order.TaxCost,
    POSubtotal,
    POTotal,
    CreditCardTotal,
    DiscountTotal,
    OrderTotal
  };
}

/* eslint-enable */

const getPOShippingCost = (
  shipEstimates: ShipEstimate[],
  POlineItems: MarketplaceLineItem[]
): number => {
  if (!shipEstimates) {
    // the error is in orderworksheet.ShipEstimateResponse.UnhandledErrorBody
    throw new Error('There was an error while retrieving shipping estimates')
  }
  const POShipEstimates = shipEstimates.filter((shipEstimate) => {
    return shipEstimate.ShipEstimateItems.some((item) =>
      POlineItems.some((li) => li.ID === item.LineItemID)
    )
  })

  return POShipEstimates.reduce((acc, shipEstimate) => {
    const selectedMethod = shipEstimate.ShipMethods.find(
      (method) => method.ID === shipEstimate.SelectedShipMethodID
    )
    return (selectedMethod?.Cost || 0) + acc
  }, 0)
}
