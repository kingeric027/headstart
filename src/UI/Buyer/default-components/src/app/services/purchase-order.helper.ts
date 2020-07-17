import { MarketplaceOrder, MarketplaceLineItem, OrderPromotion } from 'marketplace-javascript-sdk';

export interface OrderSummaryMeta {
  StandardLineItems: MarketplaceLineItem[];
  POLineItems: MarketplaceLineItem[];
  StandardLineItemCount: number;
  POLineItemCount: number;

  ShouldHideShippingAndText: boolean;
  ShippingAndTaxOverrideText: string;

  // with no purchase order these are displayed as the whole order
  CreditCardDisplaySubtotal: number;
  ShippingCost: number;
  TaxCost: number;
  CreditCardTotal: number;
  DiscountTotal: number;

  POTotal: number;
  OrderTotal: number;
}

const getPurchaseOrderLineItems = (lineItems: MarketplaceLineItem[]): MarketplaceLineItem[] => {
  return lineItems.filter(li => li.Product.xp?.ProductType ===  'PurchaseOrder');
};

const getStandardLineItems = (lineItems: MarketplaceLineItem[]): MarketplaceLineItem[] => {
  return lineItems.filter(li => !(li.Product.xp?.ProductType ===  'PurchaseOrder'));
};

const getOverrideText = (checkoutPanel: string): string => {
  /* if there is override text for shipping and tax 
  * we show that and calculate the order total differently */
  switch(checkoutPanel) {
    case 'cart':
      return 'Calculated during checkout';
    case 'shippingAddress':
    case 'shippingSelection':
      return 'Pending selections';
    default:
        return '';
  }
}

const getCreditCardTotal = (subTotal: number, shippingCost: number, taxCost: number, shouldHideShippingAndText: boolean): number => {
  if(shouldHideShippingAndText) {
    return subTotal;
  } else {
    return subTotal + shippingCost + taxCost;
  }
}

const getDiscountTotal = (orderPromos: OrderPromotion[]): number => {
  let discountTotal = 0;
  if (orderPromos?.length) {
    orderPromos.map(p => discountTotal = discountTotal + p.Amount)
  }
  return discountTotal;
}

export const getOrderSummaryMeta = (
  order: MarketplaceOrder, 
  orderPromos: OrderPromotion[],
  lineItems: MarketplaceLineItem[], 
  checkoutPanel: string, 
): OrderSummaryMeta => {
  const StandardLineItems = getStandardLineItems(lineItems);
  const POLineItems = getPurchaseOrderLineItems(lineItems);

  const ShippingAndTaxOverrideText = getOverrideText(checkoutPanel);

  const shouldHideShippingAndText = !!ShippingAndTaxOverrideText;

  const CreditCardDisplaySubtotal = StandardLineItems.reduce((accumulator, li) => (li.Quantity * li.UnitPrice) + accumulator, 0);
  const CreditCardTotal = getCreditCardTotal(CreditCardDisplaySubtotal, order.ShippingCost, order.TaxCost, shouldHideShippingAndText);
  // const DiscountTotal = getDiscountTotal(orderPromos)

  const POTotal = POLineItems.reduce((accumulator, li) => (li.Quantity * li.UnitPrice) + accumulator, 0);
  const DiscountTotal = orderPromos?.reduce((accumulator, promo) => (promo.Amount) + accumulator, 0);
  const OrderTotal = (POTotal + CreditCardTotal) - DiscountTotal;

  return {
    StandardLineItemCount: StandardLineItems.length, 
    StandardLineItems,
    POLineItemCount: POLineItems.length,
    POLineItems, 
    ShippingAndTaxOverrideText, 
    ShouldHideShippingAndText: shouldHideShippingAndText, 
    CreditCardDisplaySubtotal, 
    ShippingCost: order.ShippingCost, 
    TaxCost: order.TaxCost, 
    POTotal, 
    CreditCardTotal, 
    DiscountTotal,
    OrderTotal 
  };
}




