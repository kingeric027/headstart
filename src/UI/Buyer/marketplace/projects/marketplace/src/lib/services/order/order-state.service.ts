import { Injectable } from '@angular/core';
import { MarketplaceOrder, AppConfig, OrderType } from '../../shopper-context';
import { BehaviorSubject } from 'rxjs';
import { ListLineItem, OcOrderService, OcLineItemService, OcMeService } from '@ordercloud/angular-sdk';
import { TokenHelperService } from '../token-helper/token-helper.service';
import { listAll } from '../../functions/listAll';

@Injectable({
  providedIn: 'root',
})
export class OrderStateService {
  public readonly DefaultLineItems: ListLineItem = {
    Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
    Items: [],
  };
  private readonly DefaultOrder: MarketplaceOrder = {
    xp: {
      AvalaraTaxTransactionCode: '',
      OrderType: OrderType.Standard,
      QuoteOrderInfo: null,
    },
  };
  private orderSubject = new BehaviorSubject<MarketplaceOrder>(this.DefaultOrder);
  private lineItemSubject = new BehaviorSubject<ListLineItem>(this.DefaultLineItems);

  constructor(
    private ocOrderService: OcOrderService,
    private ocLineItemService: OcLineItemService,
    private ocMeService: OcMeService,
    private tokenHelper: TokenHelperService,
    private appConfig: AppConfig
  ) {}

  get order(): MarketplaceOrder {
    return this.orderSubject.value;
  }

  set order(value: MarketplaceOrder) {
    this.orderSubject.next(value);
  }

  get lineItems(): ListLineItem {
    return this.lineItemSubject.value;
  }

  set lineItems(value: ListLineItem) {
    this.lineItemSubject.next(value);
  }

  onOrderChange(callback: (order: MarketplaceOrder) => void): void {
    this.orderSubject.subscribe(callback);
  }

  onLineItemsChange(callback: (lineItems: ListLineItem) => void): void {
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

    const orderQueries = [
      // platform change to support quering by `true` may be coming
      this.ocMeService
        .ListOrders({
          sortBy: '!DateCreated',
          filters: { DateDeclined: '*', status: 'Unsubmitted', 'xp.IsResubmitting': 'True' },
        })
        .toPromise(),
      await this.ocMeService
        .ListOrders({
          sortBy: '!DateCreated',
          filters: { DateDeclined: '!*', status: 'Unsubmitted', 'xp.OrderType': 'Standard' },
        })
        .toPromise(),
    ];

    const [resubmittingOrders, normalUnsubmittedOrders] = await Promise.all(orderQueries);

    if (resubmittingOrders.Items.length) {
      this.order = resubmittingOrders.Items[0];
    } else if (normalUnsubmittedOrders.Items.length) {
      this.order = normalUnsubmittedOrders.Items[0];
    } else if (this.appConfig.anonymousShoppingEnabled) {
      this.order = { ID: this.tokenHelper.getAnonymousOrderID() };
    } else {
      this.order = await this.ocOrderService.Create('outgoing', this.DefaultOrder).toPromise();
    }
    if (this.order.DateCreated) {
      this.lineItems = await listAll(this.ocLineItemService, this.ocLineItemService.List, 'outgoing', this.order.ID);
    }
  }
}
