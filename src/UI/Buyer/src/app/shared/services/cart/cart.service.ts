import { Injectable } from '@angular/core';
import { OcLineItemService, ListLineItem, LineItem, OcOrderService, Order } from '@ordercloud/angular-sdk';
import { AppStateService } from '@app-buyer/shared/services/app-state/app-state.service';
import { flatMap } from 'rxjs/operators';
import { isUndefined as _isUndefined, flatMap as _flatMap, get as _get, isEqual as _isEqual, omitBy as _omitBy } from 'lodash';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private initializingOrder = false;

  constructor(
    private appStateService: AppStateService,
    private ocLineItemService: OcLineItemService,
    private ocOrderService: OcOrderService
  ) {}

  async listAllItems(orderID: string): Promise<ListLineItem> {
    const options = {
      page: 1,
      pageSize: 100, // The maximum # of records an OC request can return.
    };
    const list = await this.ocLineItemService.List('outgoing', orderID, options).toPromise();
    if (list.Meta.TotalPages <= 1) {
      return list;
    }
    // If more than 100 exist, request all the remaining pages.
    const requests = new Array(list.Meta.TotalPages - 1).map(() => {
      options.page++;
      return this.ocLineItemService.List('outgoing', orderID, options).toPromise();
    });
    const res: ListLineItem[] = await Promise.all(requests);
    const rest = _flatMap(res, (x) => x.Items);
    return { Items: list.Items.concat(rest), Meta: list.Meta };
  }

  buildSpecDisplayList(lineItem: LineItem): string {
    if (lineItem.Specs.length === 0) return '';
    const list = lineItem.Specs.map((spec) => spec.Value).join(', ');
    return `(${list})`;
  }

  async removeItem(lineItemID: string): Promise<void> {
    await this.ocLineItemService.Delete('outgoing', this.currentOrder().ID, lineItemID).toPromise();
    this.updateAppState();
  }

  async updateQuantity(lineItemID: string, newQuantity: number): Promise<LineItem> {
    const li = await this.ocLineItemService
      .Patch('outgoing', this.currentOrder().ID, lineItemID, {
        Quantity: newQuantity,
      })
      .toPromise();
    this.updateAppState();
    return li;
  }

  async addManyToCart(lineItem: LineItem[]): Promise<LineItem[]> {
    const req = lineItem.map((li) => this.addToCart(li));
    return Promise.all(req);
  }

  async addToCart(lineItem: LineItem): Promise<LineItem> {
    // order is well defined, line item can be added
    if (!_isUndefined(this.currentOrder().DateCreated)) {
      return this.createLineItem(lineItem);
    }
    // this is the first line item call - initialize order first
    if (!this.initializingOrder) {
      this.initializingOrder = true;
      const newOrder = await this.ocOrderService.Create('outgoing', {}).toPromise();
      this.initializingOrder = false;
      this.appStateService.orderSubject.next(newOrder);
      return this.createLineItem(lineItem);
    }
    // initializing order - wait until its done
    return this.appStateService.orderSubject
      .pipe(
        flatMap((newOrder) => {
          if (newOrder.ID) {
            return this.createLineItem(lineItem);
          }
        })
      )
      .toPromise();
  }

  private currentOrder(): Order {
    return this.appStateService.orderSubject.value;
  }

  private async createLineItem(newLI: LineItem): Promise<LineItem> {
    const lineItems = this.appStateService.lineItemSubject.value;
    // if line item exists simply update quantity, else create
    const existingLI = lineItems.Items.find((li) => this.LineItemsMatch(li, newLI));

    newLI.Quantity += _get(existingLI, 'Quantity', 0);
    const request = existingLI
      ? this.ocLineItemService.Patch('outgoing', this.currentOrder().ID, existingLI.ID, newLI).toPromise()
      : this.ocLineItemService.Create('outgoing', this.currentOrder().ID, newLI).toPromise();
    const lineitem = await request;
    this.appStateService.addToCartSubject.next(newLI);
    this.updateAppState();
    return lineitem;
  }

  private async updateAppState() {
    const order = await this.ocOrderService.Get('outgoing', this.currentOrder().ID).toPromise();
    const lis = await this.listAllItems(this.currentOrder().ID);
    this.appStateService.orderSubject.next(order);
    this.appStateService.lineItemSubject.next(lis);
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
