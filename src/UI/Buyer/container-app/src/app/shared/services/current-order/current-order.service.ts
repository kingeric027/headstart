// angular
import { Injectable, Inject } from '@angular/core';

// third party
import { OcMeService, Order, OcOrderService, ListLineItem, OcLineItemService } from '@ordercloud/angular-sdk';
import { applicationConfiguration } from 'src/app/config/app.config';
import { BehaviorSubject } from 'rxjs';
import { CurrentUserService } from '../current-user/current-user.service';
import { listAll } from 'src/app/shared/functions/listAll';
import { ICurrentOrder, AppConfig } from 'src/app/ocm-default-components/shopper-context';

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService implements ICurrentOrder {
  private readonly DefaultLineItems: ListLineItem = {
    Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
    Items: [],
  };

  private orderSubject: BehaviorSubject<Order> = new BehaviorSubject<Order>(null);
  private lineItemSubject: BehaviorSubject<ListLineItem> = new BehaviorSubject<ListLineItem>(this.DefaultLineItems);

  constructor(
    private ocLineItemsService: OcLineItemService,
    private ocOrderService: OcOrderService,
    private ocMeService: OcMeService,
    private currentUser: CurrentUserService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  get order(): Order {
    return this.orderSubject.value;
  }

  set order(value: Order) {
    this.orderSubject.next(value);
  }

  onOrderChange(callback: (order: Order) => void) {
    this.orderSubject.subscribe(callback);
  }

  get lineItems(): ListLineItem {
    return this.lineItemSubject.value;
  }

  set lineItems(value: ListLineItem) {
    this.lineItemSubject.next(value);
  }

  onLineItemsChange(callback: (lineItems: ListLineItem) => void) {
    this.lineItemSubject.subscribe(callback);
  }

  async reset(): Promise<void> {
    const orders = await this.ocMeService.ListOrders({ sortBy: '!DateCreated', filters: { status: 'Unsubmitted' } }).toPromise();
    if (orders.Items.length) {
      this.order = orders.Items[0];
    } else if (this.appConfig.anonymousShoppingEnabled) {
      this.order = <Order>{ ID: this.currentUser.getOrderIDFromToken() };
    } else {
      this.order = await this.ocOrderService.Create('outgoing', {}).toPromise();
    }
    if (this.order.DateCreated) {
      this.lineItems = await listAll(this.ocLineItemsService, 'outgoing', this.order.ID);
    }
  }
}
