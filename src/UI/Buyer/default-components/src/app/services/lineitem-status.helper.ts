import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';

export function NumberCanReturn(lineItem: MarketplaceLineItem): number {
  return lineItem.xp.StatusByQuantity['Complete'];
}

export function NumberCanCancel(lineItem: MarketplaceLineItem): number {
  return lineItem.xp.StatusByQuantity['Submitted'] + lineItem.xp.StatusByQuantity['Backordered'];
}

export function CanReturn(lineItem: MarketplaceLineItem): boolean {
  return !!NumberCanReturn(lineItem);
}

export function CanCancel(lineItem: MarketplaceLineItem): boolean {
  return !!NumberCanCancel(lineItem);
}

export function NumberCanCancelOrReturn(lineItem: MarketplaceLineItem, action: string): number {
  if (action === 'return') {
    return NumberCanReturn(lineItem);
  } else {
    return NumberCanCancel(lineItem);
  }
}

export function CanReturnOrCancel(lineItem: MarketplaceLineItem, action: string): boolean {
  return !!NumberCanCancelOrReturn(lineItem, action);
}

export function CanReturnOrder(lineItems: MarketplaceLineItem[]): boolean {
  return lineItems.some(li => CanReturn(li));
}

export function CanCancelOrder(lineItems: MarketplaceLineItem[]): boolean {
  return lineItems.some(li => CanCancel(li));
}
