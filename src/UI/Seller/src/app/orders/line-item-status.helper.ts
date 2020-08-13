import { MarketplaceLineItem, HeadStartSDK } from '@ordercloud/headstart-sdk';
import { LineItemStatus } from '@app-seller/shared/models/order-status.interface';

const validPreviousStates = {
  Submitted: [],
  Complete: [LineItemStatus.Submitted, LineItemStatus.Backordered],
  ReturnRequested: [LineItemStatus.Complete],
  Returned: [LineItemStatus.ReturnRequested, LineItemStatus.Complete],
  Backordered: [LineItemStatus.Submitted],
  CancelRequested: [LineItemStatus.Submitted, LineItemStatus.Backordered],
  Canceled: [LineItemStatus.CancelRequested, LineItemStatus.Backordered, LineItemStatus.Submitted],
};

export function NumberCanChangeTo(lineItemStatus: LineItemStatus, lineItem: MarketplaceLineItem): number {
  return Object.entries(lineItem.xp.StatusByQuantity as any)
    .filter(([status, quantity]) => validPreviousStates[lineItemStatus].includes(status))
    .reduce((acc, [status, quantity]) => (quantity as number) + acc, 0);
}
export function CanChangeTo(lineItemStatus: LineItemStatus, lineItem: MarketplaceLineItem): boolean {
  return !!NumberCanChangeTo(lineItemStatus, lineItem);
}

export function CanChangeLineItemsOnOrderTo(lineItemStatus: LineItemStatus, lineItems: MarketplaceLineItem[]): boolean {
  return lineItems.some(li => CanChangeTo(lineItemStatus, li));
}
