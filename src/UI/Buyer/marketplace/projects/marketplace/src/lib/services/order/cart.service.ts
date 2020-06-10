import { Injectable } from '@angular/core';
import { ListLineItem, OcOrderService, OcLineItemService, OcMeService, OcTokenService } from '@ordercloud/angular-sdk';
import { Subject } from 'rxjs';
import { OrderStateService } from './order-state.service';
import { isUndefined as _isUndefined } from 'lodash';
import { listAll } from '../../functions/listAll';
import { HttpClient } from '@angular/common/http';
import { MarketplaceLineItem, MarketplaceOrder, MarketplaceSDK } from 'marketplace-javascript-sdk';
import { AppConfig } from '../../shopper-context';

export interface ICart {
  onAdd: Subject<MarketplaceLineItem>;
  get(): ListLineItem;
  add(lineItem: MarketplaceLineItem): Promise<MarketplaceLineItem>;
  remove(lineItemID: string): Promise<void>;
  setQuantity(lineItem: MarketplaceLineItem): Promise<MarketplaceLineItem>;
  addMany(lineItem: MarketplaceLineItem[]): Promise<MarketplaceLineItem[]>;
  empty(): Promise<void>;
  onChange(callback: (lineItems: ListLineItem) => void): void;
  moveOrderToCart(orderID: string): Promise<void>;
}

@Injectable({
  providedIn: 'root',
})
export class CartService implements ICart {
  public onAdd = new Subject<MarketplaceLineItem>(); // need to make available as observable
  public onChange = this.state.onLineItemsChange.bind(this.state);
  private initializingOrder = false;

  constructor(
    private ocOrderService: OcOrderService,
    private ocLineItemService: OcLineItemService,
    private ocMeService: OcMeService,
    private state: OrderStateService,
    private http: HttpClient,
    private ocTokenService: OcTokenService,
    private appSettings: AppConfig
  ) {}

  get(): ListLineItem {
    return this.lineItems;
  }

  // TODO - get rid of the progress spinner for all Cart functions. Just makes it look slower.
  async add(lineItem: MarketplaceLineItem): Promise<MarketplaceLineItem> {
    // order is well defined, line item can be added
    if (!_isUndefined(this.order.DateCreated)) {
      return await this.upsertLineItem(lineItem);
    }
    if (!this.initializingOrder) {
      this.initializingOrder = true;
      await this.state.reset();
      this.initializingOrder = false;
      return await this.upsertLineItem(lineItem);
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

  async setQuantity(lineItem: MarketplaceLineItem): Promise<MarketplaceLineItem> {
    try {
      return await this.add(lineItem);
    } finally {
      this.state.reset();
    }
  }

  async addMany(lineItem: MarketplaceLineItem[]): Promise<MarketplaceLineItem[]> {
    const req = lineItem.map(li => this.add(li));
    return Promise.all(req);
  }

  async moveOrderToCart(orderID: string): Promise<void> {
    /* this process is to move a order into the cart which was previously marked for
     * changes by an approver. We are making the xp as IsResubmitting, then resetting the cart
     * however so that the normal unsubmitted orders (orders which were not previously declined)
     * do not repopulate in the cart after the resubmit we are deleting all of these
     * unsubmitted orders */

    const orderToUpdate = (await this.ocOrderService
      .Patch('Outgoing', orderID, { xp: { IsResubmitting: true } })
      .toPromise()) as MarketplaceOrder;

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

  private async upsertLineItem(lineItem: MarketplaceLineItem): Promise<MarketplaceLineItem> {
    this.onAdd.next(lineItem);
    try {
      return await MarketplaceSDK.Orders.UpsertLineItem(this.order?.ID, lineItem);
    } finally {
      await this.state.reset();
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
