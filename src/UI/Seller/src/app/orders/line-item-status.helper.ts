import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';
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

function getPreviousQuantitiesForCancelation(
  lineItemStatus: LineItemStatus,
  lineItem: MarketplaceLineItem,
  quantityToCancel: number
): any {
  const previousQuantities = { CancelRequested: 0, Submitted: 0, Backordered: 0 };
  // todo figure out why the typing is potentially off here for dictionaries in sdk
  let Submitted = lineItem.xp.StatusByQuantity['Submitted'] || 0;
  let Backordered = lineItem.xp.StatusByQuantity['Backordered'] || 0;
  let CancelRequested = lineItem.xp.StatusByQuantity['CancelRequested'] || 0;

  while (quantityToCancel > 0) {
    if (Submitted) {
      previousQuantities.Submitted++;
      Submitted--;
      quantityToCancel--;
    } else if (Backordered) {
      previousQuantities.Backordered++;
      Backordered--;
      quantityToCancel--;
    } else if (CancelRequested) {
      previousQuantities.CancelRequested++;
      CancelRequested--;
      quantityToCancel--;
    } else {
      throw new Error('Not enough quantity to support change');
    }
  }
  return previousQuantities;
}

export function GetPreviousQuantityAssumptions(
  lineItemStatus: LineItemStatus,
  lineItem: MarketplaceLineItem,
  quantity: number
): any {
  const numberCanChangeTo = NumberCanChangeTo(lineItemStatus, lineItem);
  if (numberCanChangeTo < quantity) {
    throw new Error('Not enough quantity on the item to support this change');
  }

  if (!(lineItemStatus === LineItemStatus.Backordered || lineItemStatus === LineItemStatus.Canceled)) {
    throw new Error('Assumptions for this change not configured');
  }

  if (lineItemStatus === LineItemStatus.Backordered) {
    return { Submitted: quantity };
  }

  if (lineItemStatus === LineItemStatus.Canceled) {
    return getPreviousQuantitiesForCancelation(lineItemStatus, lineItem, quantity);
  }
}
