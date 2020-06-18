import { Injectable } from '@angular/core';
import { Me, Inventory, PriceSchedule } from 'ordercloud-javascript-sdk';
import { partition as _partition } from 'lodash';
import { OrderReorderResponse, MarketplaceMeProduct } from '../../shopper-context';
import { MarketplaceLineItem } from 'marketplace-javascript-sdk';

@Injectable({
  providedIn: 'root',
})
export class ReorderHelperService {
  constructor() {}

  public async validateReorder(orderID: string, lineItems: MarketplaceLineItem[]): Promise<OrderReorderResponse> {
    // instead of moving all of this logic to the middleware to support orders not
    // submitted by the current user we are adding line items as a paramter

    if (!orderID) throw new Error('Needs Order ID');
    const products = await this.ListProducts(lineItems);
    const [ValidLi, InvalidLi] = _partition(lineItems, item => this.isLineItemValid(item, products));
    return { ValidLi, InvalidLi };
  }

  private async ListProducts(items: MarketplaceLineItem[]): Promise<MarketplaceMeProduct[]> {
    const productIds = items.map(item => item.ProductID);
    // TODO - what if the url is too long?
    return (await Me.ListProducts({ filters: { ID: productIds.join('|') } })).Items;
  }

  private isLineItemValid(item: MarketplaceLineItem, products: MarketplaceMeProduct[]): boolean {
    const product = products.find(prod => prod.ID === item.ProductID);
    return product && !this.quantityInvalid(item.Quantity, product);
  }

  private quantityInvalid(qty: number, product: MarketplaceMeProduct): boolean {
    return (
      this.inventoryTooLow(qty, product.Inventory) || this.restrictedQuantitiesInvalidate(qty, product.PriceSchedule)
    );
  }

  private inventoryTooLow(qty: number, inventory: Inventory): boolean {
    return inventory && inventory.Enabled && !inventory.OrderCanExceed && qty > inventory.QuantityAvailable;
  }

  private restrictedQuantitiesInvalidate(qty: number, schedule: PriceSchedule): boolean {
    return schedule.RestrictedQuantity && !schedule.PriceBreaks.some(pb => pb.Quantity === qty);
  }
}
