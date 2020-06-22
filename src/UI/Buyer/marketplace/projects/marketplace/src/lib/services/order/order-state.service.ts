import { Injectable } from '@angular/core';
import { ListPage, MarketplaceLineItem, MarketplaceOrder } from 'marketplace-javascript-sdk';
import { LineItems, Me, Order, Orders } from 'ordercloud-javascript-sdk';
import { BehaviorSubject } from 'rxjs';
import { listAll } from '../../functions/listAll';
import { AppConfig, ClaimStatus, ShippingStatus } from '../../shopper-context';
import { CurrentUserService } from '../current-user/current-user.service';
import { TokenHelperService } from '../token-helper/token-helper.service';

@Injectable({
  providedIn: 'root',
})
export class OrderStateService {
  public readonly DefaultLineItems: ListPage<MarketplaceLineItem> = {
    Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
    Items: [],
  };
  private readonly DefaultOrder: MarketplaceOrder = {
    xp: {
      AvalaraTaxTransactionCode: '',
      OrderType: 'Standard',
      QuoteOrderInfo: null,
      Currency: 'USD', // Default value, overriden in reset() when app loads
      OrderReturnInfo: {
        HasReturn: false,
      },
      ClaimStatus: ClaimStatus.NoClaim,
      ShippingStatus: ShippingStatus.Processing
    },
  };
  private orderSubject = new BehaviorSubject<MarketplaceOrder>(this.DefaultOrder);
  private lineItemSubject = new BehaviorSubject<ListPage<MarketplaceLineItem>>(this.DefaultLineItems);

  constructor(
    private tokenHelper: TokenHelperService,
    private currentUserService: CurrentUserService,
    private appConfig: AppConfig
  ) {}

  get order(): MarketplaceOrder {
    return this.orderSubject.value;
  }

  set order(value: MarketplaceOrder) {
    this.orderSubject.next(value);
  }

  get lineItems(): ListPage<MarketplaceLineItem> {
    return this.lineItemSubject.value;
  }

  set lineItems(value: ListPage<MarketplaceLineItem>) {
    this.lineItemSubject.next(value);
  }

  onOrderChange(callback: (order: MarketplaceOrder) => void): void {
    this.orderSubject.subscribe(callback);
  }

  onLineItemsChange(callback: (lineItems: ListPage<MarketplaceLineItem>) => void): void {
    this.lineItemSubject.subscribe(callback);
  }

  async reset(): Promise<void> {
    /* when an order is declined it will appear as an unsubmitted order with
     * a date declined, we need to remove these from the normal order
     * query so that it does not immediately affect a users cart by getting
     * mixed into the normal unsubmitted orders
     * however we also need to know when a user marks an order for
     * resbumit which we are designating with xp.IsResubmitting
     */
    const [ordersForResubmit, ordersNeverSubmitted] = await Promise.all([
      this.getOrdersForResubmit(),
      this.getOrdersNeverSubmitted(),
    ]);
    if (ordersForResubmit.Items.length) {
      this.order = ordersForResubmit.Items[0];
    } else if (ordersNeverSubmitted.Items.length) {
      this.order = ordersNeverSubmitted.Items[0];
    } else if (this.appConfig.anonymousShoppingEnabled) {
      this.order = { ID: this.tokenHelper.getAnonymousOrderID() };
    } else {
      this.DefaultOrder.xp.Currency = this.currentUserService.get().Currency;
      this.order = (await Orders.Create('Outgoing', this.DefaultOrder as Order)) as MarketplaceOrder;
    }
    if (this.order.DateCreated) {
      this.lineItems = await listAll(LineItems, LineItems.List, 'outgoing', this.order.ID);
    }
  }

  private async getOrdersForResubmit(): Promise<ListPage<MarketplaceOrder>> {
    const orders = await Me.ListOrders({
      sortBy: '!DateCreated',
      filters: { DateDeclined: '*', status: 'Unsubmitted', 'xp.IsResubmitting': 'True' },
    });
    return orders;
  }

  private async getOrdersNeverSubmitted(): Promise<ListPage<MarketplaceOrder>> {
    const orders = await Me.ListOrders({
      sortBy: '!DateCreated',
      filters: { DateDeclined: '!*', status: 'Unsubmitted', 'xp.OrderType': 'Standard' },
    });
    return orders;
  }
}
