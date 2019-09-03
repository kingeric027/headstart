// angular
import { Injectable, Inject } from '@angular/core';

// third party
import { OcMeService, Order, OcOrderService, ListLineItem, LineItem, OcLineItemService } from '@ordercloud/angular-sdk';
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';
import { Subject, BehaviorSubject } from 'rxjs';
import { CurrentUserService } from '../current-user/current-user.service';
import { listAll } from '@app-buyer/shared/functions/listAll';

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService {
  public addToCartSubject: Subject<LineItem> = new Subject<LineItem>();
  public orderSubject: BehaviorSubject<Order> = new BehaviorSubject<Order>(null);
  public lineItemSubject: BehaviorSubject<ListLineItem> = new BehaviorSubject<ListLineItem>({
    Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
    Items: [],
  });

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

  get lineItems(): ListLineItem {
    return this.lineItemSubject.value;
  }

  set lineItems(value: ListLineItem) {
    this.lineItemSubject.next(value);
  }

  async reset(): Promise<void> {
    let order: Order;
    const orders = await this.ocMeService.ListOrders({ sortBy: '!DateCreated', filters: { status: 'Unsubmitted' } }).toPromise();
    if (orders.Items.length) {
      order = orders.Items[0];
    }
    if (!order && this.appConfig.anonymousShoppingEnabled) {
      order = <Order>{ ID: this.currentUser.getOrderIDFromToken() };
    }
    if (!order) {
      order = await this.ocOrderService.Create('outgoing', {}).toPromise();
    }
    this.orderSubject.next(order);
    this.setCurrentCart(order);
  }

  private async setCurrentCart(order: Order): Promise<void> {
    let items: ListLineItem = {
      Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
      Items: [],
    };
    if (order.DateCreated) {
      items = await listAll(this.ocLineItemsService, 'outgoing', order.ID);
    }
    this.lineItemSubject.next(items);
  }
}
