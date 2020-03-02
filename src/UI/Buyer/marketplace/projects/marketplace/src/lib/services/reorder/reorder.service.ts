import { Injectable } from '@angular/core';
import { OcMeService, LineItem, Inventory, PriceSchedule, OcLineItemService } from '@ordercloud/angular-sdk';
import { partition as _partition } from 'lodash';
import { listAll } from '../../functions/listAll';
import { OrderReorderResponse, MarketplaceProduct } from '../../shopper-context';

@Injectable({
  providedIn: 'root',
})
export class ReorderHelperService {
  constructor(private ocLineItemService: OcLineItemService, private meService: OcMeService) {}

  public async validateReorder(orderID: string): Promise<OrderReorderResponse> {
    if (!orderID) throw new Error('Needs Order ID');
    const lineItems = (await listAll(this.ocLineItemService, this.ocLineItemService.List, 'outgoing', orderID)).Items;
    const products = await this.ListProducts(lineItems);
    const [ValidLi, InvalidLi] = _partition(lineItems, item => this.isLineItemValid(item, products));
    return { ValidLi, InvalidLi };
  }

  private async ListProducts(items: LineItem[]): Promise<MarketplaceProduct[]> {
    const productIds = items.map(item => item.ProductID);
    // TODO - what if the url is too long?
    return (await this.meService.ListProducts({ filters: { ID: productIds.join('|') } }).toPromise()).Items;
  }

  private isLineItemValid(item: LineItem, products: MarketplaceProduct[]): boolean {
    const product = products.find(prod => prod.ID === item.ProductID);
    return product && !this.quantityInvalid(item.Quantity, product);
  }

  private quantityInvalid(qty: number, product: MarketplaceProduct): boolean {
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
