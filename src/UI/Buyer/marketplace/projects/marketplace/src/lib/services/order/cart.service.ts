import { Injectable } from '@angular/core';
import { Orders, LineItems, Me } from 'ordercloud-javascript-sdk';
import { Subject } from 'rxjs';
import { OrderStateService } from './order-state.service';
import { isUndefined as _isUndefined } from 'lodash';
import { listAll } from '../../functions/listAll';
import { MarketplaceLineItem, MarketplaceOrder, MarketplaceSDK, ListPage } from 'marketplace-javascript-sdk';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  public onAdd = new Subject<MarketplaceLineItem>(); // need to make available as observable
  public onChange = this.state.onLineItemsChange.bind(this.state);
  private initializingOrder = false;

  constructor(private state: OrderStateService) {}

  get(): ListPage<MarketplaceLineItem> {
    return this.lineItems;
  }

  // TODO - get rid of the progress spinner for all Cart functions. Just makes it look slower.
  async add(lineItem: MarketplaceLineItem): Promise<MarketplaceLineItem> {
    // order is well defined, line item can be added
    this.onAdd.next(lineItem);
    if (!_isUndefined(this.order.DateCreated)) {
      const lineItems = this.state.lineItems.Items;
      const selectedItemQuantity = lineItems.find(li => li.ProductID === lineItem.ProductID)?.Quantity;
      if (selectedItemQuantity) {
        lineItem.Quantity += selectedItemQuantity;
      }
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
      await LineItems.Delete('Outgoing', this.order.ID, lineItemID);
    } finally {
      this.state.reset();
    }
  }

  async setQuantity(lineItem: MarketplaceLineItem): Promise<MarketplaceLineItem> {
    try {
      return await this.upsertLineItem(lineItem);
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

    const orderToUpdate = (await Orders.Patch('Outgoing', orderID, {
      xp: { IsResubmitting: true },
    })) as MarketplaceOrder;

    const currentUnsubmittedOrders = await Me.ListOrders({
      sortBy: '!DateCreated',
      filters: { DateDeclined: '!*', status: 'Unsubmitted', 'xp.OrderType': 'Standard' },
    });

    const deleteOrderRequests = currentUnsubmittedOrders.Items.map(c => Orders.Delete('Outgoing', c.ID));
    await Promise.all(deleteOrderRequests);

    // cannot use this.state.reset because the order index isn't ready immediately after the patch of IsResubmitting
    this.state.order = orderToUpdate;
    this.state.lineItems = await listAll(LineItems, LineItems.List, 'Outgoing', this.order.ID);
  }

  async empty(): Promise<void> {
    const ID = this.order.ID;
    this.lineItems = this.state.DefaultLineItems;
    Object.assign(this.order, this.calculateOrder());
    try {
      await Orders.Delete('Outgoing', ID);
    } finally {
      this.state.reset();
    }
  }

  private async upsertLineItem(lineItem: MarketplaceLineItem): Promise<MarketplaceLineItem> {
    try {
      return await MarketplaceSDK.Orders.UpsertLineItem(this.order?.ID, lineItem);
    } finally {
      await this.state.reset();
    }
  }

  private calculateOrder(): MarketplaceOrder {
    const LineItemCount = this.lineItems.Items.length;
    this.lineItems.Items.forEach((li: any) => {
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

  private get lineItems(): ListPage<MarketplaceLineItem> {
    return this.state.lineItems;
  }

  private set lineItems(value: ListPage<MarketplaceLineItem>) {
    this.state.lineItems = value;
  }
}
