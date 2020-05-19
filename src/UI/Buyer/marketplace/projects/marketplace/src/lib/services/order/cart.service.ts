import { Injectable } from '@angular/core';
import { LineItem, ListLineItem, OcOrderService, OcLineItemService, OcMeService } from '@ordercloud/angular-sdk';
import { Subject } from 'rxjs';
import { OrderStateService } from './order-state.service';
import { isUndefined as _isUndefined } from 'lodash';
import { MarketplaceOrder } from '../../shopper-context';
import { listAll } from '../../functions/listAll';

export interface ICart {
  onAdd: Subject<LineItem>;
  get(): ListLineItem;
  add(lineItem: LineItem): Promise<LineItem>;
  remove(lineItemID: string): Promise<void>;
  setQuantity(lineItemID: string, newQuantity: number): Promise<LineItem>;
  addMany(lineItem: LineItem[]): Promise<LineItem[]>;
  empty(): Promise<void>;
  onChange(callback: (lineItems: ListLineItem) => void): void;
  moveOrderToCart(orderID: string): Promise<void>;
}

@Injectable({
  providedIn: 'root',
})
export class CartService implements ICart {
  public onAdd = new Subject<LineItem>(); // need to make available as observable
  public onChange = this.state.onLineItemsChange.bind(this.state);
  private initializingOrder = false;

  constructor(
    private ocOrderService: OcOrderService,
    private ocLineItemService: OcLineItemService,
    private ocMeService: OcMeService,
    private state: OrderStateService
  ) { }

  get(): ListLineItem {
    return this.lineItems;
  }

  // TODO - get rid of the progress spinner for all Cart functions. Just makes it look slower.
  async add(lineItem: LineItem): Promise<LineItem> {
    // order is well defined, line item can be added
    if (!_isUndefined(this.order.DateCreated)) {
      return this.createLineItem(lineItem);
    }
    if (!this.initializingOrder) {
      this.initializingOrder = true;
      this.state.reset();
      this.initializingOrder = false;
      return this.createLineItem(lineItem);
    }
  }

  async remove(lineItemID: string): Promise<void> {
    this.lineItems.Items = this.lineItems.Items.filter(li => li.ID !== lineItemID);
    Object.assign(this.state.order, this.calculateOrder());
    try {
      await this.ocLineItemService.Delete('outgoing', this.order.ID, lineItemID).toPromise();
    } finally {
      this.state.reset();
    }
  }

  async setQuantity(lineItemID: string, newQuantity: number): Promise<LineItem> {
    try {
      return await this.patchLineItem(lineItemID, { Quantity: newQuantity });
    } finally {
      this.state.reset();
    }
  }

  async addMany(lineItem: LineItem[]): Promise<LineItem[]> {
    const req = lineItem.map(li => this.add(li));
    return Promise.all(req);
  }

  async moveOrderToCart(orderID: string): Promise<void> {
    /* this process is to move a order into the cart which was previously marked for
     * changes by an approver. We are making the xp as IsResubmitting, then resetting the cart
     * however so that the normal unsubmitted orders (orders which were not previously declined)
     * do not repopulate in the cart after the resubmit we are deleting all of these
     * unsubmitted orders */

    const orderToUpdate = await this.ocOrderService
      .Patch('Outgoing', orderID, { xp: { IsResubmitting: true } })
      .toPromise();

    const currentUnsubmittedOrders = await this.ocMeService
      .ListOrders({
        sortBy: '!DateCreated',
        filters: { DateDeclined: '!*', status: 'Unsubmitted', 'xp.OrderType': 'Standard' },
      })
      .toPromise();

    const deleteOrderRequests = currentUnsubmittedOrders.Items.map(c =>
      this.ocOrderService.Delete('Outgoing', c.ID).toPromise()
    );
    await Promise.all(deleteOrderRequests);

    // cannot use this.state.reset because the order index isn't ready immediately after the patch of IsResubmitting
    this.state.order = orderToUpdate;
    this.state.lineItems = await listAll(
      this.ocLineItemService,
      this.ocLineItemService.List,
      'outgoing',
      this.order.ID
    );
  }

  async empty(): Promise<void> {
    const ID = this.order.ID;
    this.lineItems = this.state.DefaultLineItems;
    Object.assign(this.order, this.calculateOrder());
    try {
      await this.ocOrderService.Delete('outgoing', ID).toPromise();
    } finally {
      this.state.reset();
    }
  }

  private async patchLineItem(lineItemID: string, patch: LineItem): Promise<LineItem> {
    const existingLI = this.lineItems.Items.find(li => li.ID === lineItemID);
    Object.assign(existingLI, patch);
    Object.assign(this.order, this.calculateOrder());
    return await this.ocLineItemService.Patch('outgoing', this.order.ID, lineItemID, patch).toPromise();
  }

  private async createLineItem(lineItem: LineItem): Promise<LineItem> {
    // if line item exists simply update quantity, else create
    const existingLI = this.lineItems.Items.find(li => this.LineItemsMatch(li, lineItem));

    this.onAdd.next(lineItem);
    try {
      if (existingLI) {
        lineItem = await this.setQuantity(existingLI.ID, lineItem.Quantity + existingLI.Quantity);
      } else {
        this.lineItems.Items.push(lineItem);
        Object.assign(this.order, this.calculateOrder());
        lineItem = await this.ocLineItemService.Create('outgoing', this.order.ID, lineItem).toPromise();
      }
      return lineItem;
    } finally {
      this.state.reset();
    }
  }

  private calculateOrder(): MarketplaceOrder {
    const LineItemCount = this.lineItems.Items.length;
    this.lineItems.Items.forEach(li => {
      li.LineTotal = li.Quantity * li.UnitPrice;
      if (isNaN(li.LineTotal)) li.LineTotal = undefined;
    });
    const Subtotal = this.lineItems.Items.reduce((sum, li) => sum + li.LineTotal, 0);
    const Total = Subtotal + this.order.TaxCost + this.order.ShippingCost;
    return { LineItemCount, Total, Subtotal };
  }

  // product ID and specs must be the same
  private LineItemsMatch(li1: LineItem, li2: LineItem): boolean {
    if (li1.ProductID !== li2.ProductID) return false;
    for (const spec1 of li1.Specs) {
      const spec2 = li2.Specs.find(s => s.SpecID === spec1.SpecID);
      if (spec1.Value !== spec2.Value) return false;
    }
    return true;
  }

  private get order(): MarketplaceOrder {
    return this.state.order;
  }

  private set order(value: MarketplaceOrder) {
    this.state.order = value;
  }

  private get lineItems(): ListLineItem {
    return this.state.lineItems;
  }

  private set lineItems(value: ListLineItem) {
    this.state.lineItems = value;
  }
}
