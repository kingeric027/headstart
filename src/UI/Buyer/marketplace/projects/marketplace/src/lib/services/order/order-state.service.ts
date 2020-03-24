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
      BuyerLocationID: '',
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
    const orders = await this.ocMeService
      .ListOrders({ sortBy: '!DateCreated', filters: { status: 'Unsubmitted' } })
      .toPromise();
    if (orders.Items.length) {
      this.order = orders.Items[0];
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
