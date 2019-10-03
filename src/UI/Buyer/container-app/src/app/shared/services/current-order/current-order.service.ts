// angular
import { Injectable, Inject } from '@angular/core';

// third party
import { OcMeService, Order, OcOrderService, ListLineItem, OcLineItemService, Address, ListPayment } from '@ordercloud/angular-sdk';
import { applicationConfiguration } from 'src/app/config/app.config';
import { BehaviorSubject } from 'rxjs';
import { CurrentUserService } from '../current-user/current-user.service';
import { listAll } from 'src/app/shared/functions/listAll';
import { ICurrentOrder, AppConfig } from 'shopper-context-interface';
import { ToastrService } from 'ngx-toastr';
import { AppPaymentService } from '../app-payment/app-payment.service';

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
    private toastrService: ToastrService,
    private appPaymentService: AppPaymentService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  private get order(): Order {
    return this.orderSubject.value;
  }

  private set order(value: Order) {
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

  get(): Order {
    return this.order;
  }

  async patch(order: Order): Promise<Order> {
    return (this.order = await this.ocOrderService.Patch('outgoing', this.order.ID, order).toPromise());
  }

  async submit(): Promise<void> {
    await this.ocOrderService.Submit('outgoing', this.order.ID).toPromise();
    await this.reset();
  }

  async setBillingAddress(address: Address): Promise<Order> {
    return (this.order = await this.ocOrderService.SetBillingAddress('outgoing', this.order.ID, address).toPromise());
  }

  async setShippingAddress(address: Address): Promise<Order> {
    return (this.order = await this.ocOrderService.SetShippingAddress('outgoing', this.order.ID, address).toPromise());
  }

  async setBillingAddressByID(addressID: string): Promise<Order> {
    try {
      return await this.patch({ BillingAddressID: addressID });
    } catch (ex) {
      if (ex.error.Errors[0].ErrorCode === 'NotFound') {
        this.toastrService.error('You no longer have access to this saved address. Please enter or select a different one.');
      }
    }
  }

  async setShippingAddressByID(addressID: string): Promise<Order> {
    try {
      return await this.patch({ ShippingAddressID: addressID });
    } catch (ex) {
      if (ex.error.Errors[0].ErrorCode === 'NotFound') {
        this.toastrService.error('You no longer have access to this saved address. Please enter or select a different one.');
      }
    }
  }

  async listPayments(): Promise<ListPayment> {
    return await this.appPaymentService.ListPaymentsOnOrder(this.order.ID);
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
