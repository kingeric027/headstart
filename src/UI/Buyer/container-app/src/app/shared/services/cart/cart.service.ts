import { Injectable } from '@angular/core';
import { OcLineItemService, LineItem, OcOrderService } from '@ordercloud/angular-sdk';
import { isUndefined as _isUndefined, flatMap as _flatMap, get as _get, isEqual as _isEqual, omitBy as _omitBy } from 'lodash';
import { CurrentOrderService } from '../current-order/current-order.service';
import { Subject } from 'rxjs';
import { ICartActions } from 'shopper-context-interface';

@Injectable({
  providedIn: 'root',
})
export class CartService implements ICartActions {
  private initializingOrder = false;

  public addToCartSubject: Subject<LineItem> = new Subject<LineItem>(); // need to make available as observable

  constructor(
    private currentOrder: CurrentOrderService,
    private ocLineItemService: OcLineItemService,
    private ocOrderService: OcOrderService
  ) {}

  async addToCart(lineItem: LineItem): Promise<LineItem> {
    // order is well defined, line item can be added
    if (!_isUndefined(this.currentOrder.order.DateCreated)) {
      return this.createLineItem(lineItem);
    }
    // TODO - what to do if order is initializing?
    if (!this.initializingOrder) {
      this.initializingOrder = true;
      const newOrder = await this.ocOrderService.Create('outgoing', {}).toPromise();
      this.initializingOrder = false;
      this.currentOrder.order = newOrder;
      return this.createLineItem(lineItem);
    }
  }

  async removeLineItem(lineItemID: string): Promise<void> {
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

  async emptyCart(): Promise<void> {
    await this.ocOrderService.Delete('outgoing', this.currentOrder.order.ID).toPromise();
    await this.currentOrder.reset();
  }

  onAddToCart(callback: (lineItem: LineItem) => void): void {
    this.addToCartSubject.subscribe(callback);
  }

  private async createLineItem(newLI: LineItem): Promise<LineItem> {
    // if line item exists simply update quantity, else create
    const existingLI = this.currentOrder.lineItems.Items.find((li) => this.LineItemsMatch(li, newLI));

    newLI.Quantity += _get(existingLI, 'Quantity', 0);
    const request = existingLI
      ? this.ocLineItemService.Patch('outgoing', this.currentOrder.order.ID, existingLI.ID, newLI).toPromise()
      : this.ocLineItemService.Create('outgoing', this.currentOrder.order.ID, newLI).toPromise();
    const lineitem = await request;
    this.addToCartSubject.next(newLI);
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
