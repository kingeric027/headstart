import { Injectable } from '@angular/core';
import { OcLineItemService, ListLineItem, LineItem, OcOrderService } from '@ordercloud/angular-sdk';
import { flatMap } from 'rxjs/operators';
import { isUndefined as _isUndefined, flatMap as _flatMap, get as _get, isEqual as _isEqual, omitBy as _omitBy } from 'lodash';
import { CurrentOrderService } from '../current-order/current-order.service';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private initializingOrder = false;

  constructor(
    private currentOrder: CurrentOrderService,
    private ocLineItemService: OcLineItemService,
    private ocOrderService: OcOrderService
  ) {}

  addSpecsToProductName(lineItems: ListLineItem): ListLineItem {
    const lis: ListLineItem = JSON.parse(JSON.stringify(lineItems));
    lis.Items = lis.Items.map((lineItem) => {
      if (lineItem.Specs.length === 0) return lineItem;
      const list = lineItem.Specs.map((spec) => spec.Value).join(', ');
      lineItem.Product.Name = `${lineItem.Product.Name} (${list})`;
      return lineItem;
    });
    return lis;
  }

  async removeItem(lineItemID: string): Promise<void> {
    await this.ocLineItemService.Delete('outgoing', this.currentOrder.order.ID, lineItemID).toPromise();
    this.currentOrder.reset();
  }

  async updateQuantity(lineItemID: string, newQuantity: number): Promise<LineItem> {
    const li = await this.ocLineItemService
      .Patch('outgoing', this.currentOrder.order.ID, lineItemID, {
        Quantity: newQuantity,
      })
      .toPromise();
    this.currentOrder.reset();
    return li;
  }

  async addManyToCart(lineItem: LineItem[]): Promise<LineItem[]> {
    const req = lineItem.map((li) => this.addToCart(li));
    return Promise.all(req);
  }

  async addToCart(lineItem: LineItem): Promise<LineItem> {
    // order is well defined, line item can be added
    if (!_isUndefined(this.currentOrder.order.DateCreated)) {
      return this.createLineItem(lineItem);
    }
    // this is the first line item call - initialize order first
    if (!this.initializingOrder) {
      this.initializingOrder = true;
      const newOrder = await this.ocOrderService.Create('outgoing', {}).toPromise();
      this.initializingOrder = false;
      this.currentOrder.order = newOrder;
      return this.createLineItem(lineItem);
    }
    // initializing order - wait until its done
    return this.currentOrder.orderSubject
      .pipe(
        flatMap((newOrder) => {
          if (newOrder.ID) {
            return this.createLineItem(lineItem);
          }
        })
      )
      .toPromise();
  }

  private async createLineItem(newLI: LineItem): Promise<LineItem> {
    // if line item exists simply update quantity, else create
    const existingLI = this.currentOrder.lineItems.Items.find((li) => this.LineItemsMatch(li, newLI));

    newLI.Quantity += _get(existingLI, 'Quantity', 0);
    const request = existingLI
      ? this.ocLineItemService.Patch('outgoing', this.currentOrder.order.ID, existingLI.ID, newLI).toPromise()
      : this.ocLineItemService.Create('outgoing', this.currentOrder.order.ID, newLI).toPromise();
    const lineitem = await request;
    this.currentOrder.addToCartSubject.next(newLI);
    this.currentOrder.reset();
    return lineitem;
  }

  // product ID and specs must be the same
  private LineItemsMatch(li1: LineItem, li2: LineItem): boolean {
    if (li1.ProductID !== li2.ProductID) return false;
    for (let i = 0; i < li1.Specs.length; i++) {
      const spec1 = li1.Specs[i];
      const spec2 = li2.Specs.find((s) => s.SpecID === spec1.SpecID);
      if (spec1.Value !== spec2.Value) return false;
    }
    return true;
  }
}
