import { BuyerProduct } from '@ordercloud/angular-sdk';
import { get as _get } from 'lodash';
import { QuantityLimits } from '../models/quantity-limits';

export function BuildQtyLimits(product: BuyerProduct): QuantityLimits {
  const inventory = getInventory(product);
  const minPerOrder = minQty(product);
  const maxPerOrder = maxQty(product);
  const restrictedQuantities = getRestrictedQuantities(product);
  return { inventory, minPerOrder, maxPerOrder, restrictedQuantities };
}

function getRestrictedQuantities(product: BuyerProduct): number[] {
  const isRestricted = _get(product, 'PriceSchedule.RestrictedQuantity', false);
  if (!isRestricted) return [];
  return product.PriceSchedule.PriceBreaks.map((b) => b.Quantity);
}

function minQty(product: BuyerProduct): number {
  if (product.PriceSchedule && product.PriceSchedule.MinQuantity) {
    return product.PriceSchedule.MinQuantity;
  }
  return 1;
}

function maxQty(product: BuyerProduct): number {
  if (product.PriceSchedule && product.PriceSchedule.MaxQuantity != null) {
    return product.PriceSchedule.MaxQuantity;
  }
  return Infinity;
}

function getInventory(product: BuyerProduct): number {
  if (product.Inventory && product.Inventory.Enabled && !product.Inventory.OrderCanExceed && product.Inventory.QuantityAvailable != null) {
    return product.Inventory.QuantityAvailable;
  }
  return Infinity;
}
