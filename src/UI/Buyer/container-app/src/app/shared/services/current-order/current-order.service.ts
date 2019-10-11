// angular
import { Injectable, Inject } from '@angular/core';

// third party
import {
  OcMeService,
  Order,
  OcOrderService,
  ListLineItem,
  OcLineItemService,
  Address,
  ListPayment,
  Payment,
  OcPaymentService,
  LineItem,
} from '@ordercloud/angular-sdk';
import { applicationConfiguration } from 'src/app/config/app.config';
import { BehaviorSubject, Subject } from 'rxjs';
import { listAll } from 'src/app/shared/functions/listAll';
import { ICurrentOrder, AppConfig } from 'shopper-context-interface';
import { ToastrService } from 'ngx-toastr';
import { isUndefined as _isUndefined, get as _get } from 'lodash';
import { TokenHelperService } from '../token-helper/token-helper.service';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService implements ICurrentOrder {
  private readonly DefaultLineItems: ListLineItem = {
    Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
    Items: [],
  };
  private initializingOrder = false;
  public addToCartSubject: Subject<LineItem> = new Subject<LineItem>(); // need to make available as observable
  private orderSubject: BehaviorSubject<Order> = new BehaviorSubject<Order>(null);
  private lineItemSubject: BehaviorSubject<ListLineItem> = new BehaviorSubject<ListLineItem>(this.DefaultLineItems);

  constructor(
    private ocOrderService: OcOrderService,
    private ocLineItemService: OcLineItemService,
    private ocMeService: OcMeService,
    private tokenHelper: TokenHelperService,
    private ocPaymentService: OcPaymentService,
    private toastrService: ToastrService,
    private paymentHelper: PaymentHelperService,
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

  get(): Order {
    return this.order;
  }

  async patch(order: Order): Promise<Order> {
    return (this.order = await this.ocOrderService.Patch('outgoing', this.order.ID, order).toPromise());
  }

  private get lineItems(): ListLineItem {
    return this.lineItemSubject.value;
  }

  private set lineItems(value: ListLineItem) {
    this.lineItemSubject.next(value);
  }

  onLineItemsChange(callback: (lineItems: ListLineItem) => void) {
    this.lineItemSubject.subscribe(callback);
  }

  getLineItems(): ListLineItem {
    return this.lineItems;
  }

  async patchLineItem(lineItemID: string, patch: LineItem): Promise<LineItem> {
    const existingLI = this.lineItems.Items.find((li) => li.ID === lineItemID);
    Object.assign(existingLI, patch);
    Object.assign(this.order, this.calculateOrder());
    return await this.ocLineItemService.Patch('outgoing', this.order.ID, lineItemID, patch).toPromise();
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
    return await this.paymentHelper.ListPaymentsOnOrder(this.order.ID);
  }

  async createPayment(payment: Payment): Promise<Payment> {
    await this.deleteExistingPayments(); // TODO - is this still needed? There used to be an OC bug with multiple payments on an order.
    return await this.ocPaymentService.Create('outgoing', this.order.ID, payment).toPromise();
  }

  private async deleteExistingPayments(): Promise<any[]> {
    const payments = await this.ocPaymentService.List('outgoing', this.order.ID).toPromise();
    const deleteAll = payments.Items.map((payment) => this.ocPaymentService.Delete('outgoing', this.order.ID, payment.ID).toPromise());
    return Promise.all(deleteAll);
  }

  async reset(): Promise<void> {
    const orders = await this.ocMeService.ListOrders({ sortBy: '!DateCreated', filters: { status: 'Unsubmitted' } }).toPromise();
    if (orders.Items.length) {
      this.order = orders.Items[0];
    } else if (this.appConfig.anonymousShoppingEnabled) {
      this.order = <Order>{ ID: this.tokenHelper.getAnonmousOrderID() };
    } else {
      this.order = await this.ocOrderService.Create('outgoing', {}).toPromise();
    }
    if (this.order.DateCreated) {
      this.lineItems = await listAll(this.ocLineItemService, 'outgoing', this.order.ID);
    }
  }

  // TODO - get rid of the progress spinner for all Cart functions. Just makes it look slower.
  async addToCart(lineItem: LineItem): Promise<LineItem> {
    // order is well defined, line item can be added
    if (!_isUndefined(this.order.DateCreated)) {
      return this.createLineItem(lineItem);
    }
    if (!this.initializingOrder) {
      this.initializingOrder = true;
      this.reset();
      this.initializingOrder = false;
      return this.createLineItem(lineItem);
    }
  }

  async removeFromCart(lineItemID: string): Promise<void> {
    this.lineItems.Items = this.lineItems.Items.filter((li) => li.ID !== lineItemID);
    Object.assign(this.order, this.calculateOrder());
    try {
      await this.ocLineItemService.Delete('outgoing', this.order.ID, lineItemID).toPromise();
    } finally {
      this.reset();
    }
  }

  async setQuantityInCart(lineItemID: string, newQuantity: number): Promise<LineItem> {
    try {
      const li = await this.patchLineItem(lineItemID, { Quantity: newQuantity });
      return li;
    } finally {
      this.reset();
    }
  }

  async addManyToCart(lineItem: LineItem[]): Promise<LineItem[]> {
    const req = lineItem.map((li) => this.addToCart(li));
    return Promise.all(req);
  }

  async emptyCart(): Promise<void> {
    const ID = this.order.ID;
    this.lineItems = this.DefaultLineItems;
    Object.assign(this.order, this.calculateOrder());
    try {
      await this.ocOrderService.Delete('outgoing', ID).toPromise();
    } finally {
      this.reset();
    }
  }

  private async createLineItem(lineItem: LineItem): Promise<LineItem> {
    // if line item exists simply update quantity, else create
    const existingLI = this.lineItems.Items.find((li) => this.LineItemsMatch(li, lineItem));

    this.addToCartSubject.next(lineItem);
    try {
      if (existingLI) {
        lineItem = await this.setQuantityInCart(existingLI.ID, lineItem.Quantity + existingLI.Quantity);
      } else {
        this.lineItems.Items.push(lineItem);
        Object.assign(this.order, this.calculateOrder());
        lineItem = await this.ocLineItemService.Create('outgoing', this.order.ID, lineItem).toPromise();
      }
      return lineItem;
    } finally {
      this.reset();
    }
  }

  private calculateOrder(): Order {
    const LineItemCount = this.lineItems.Items.length;
    this.lineItems.Items.forEach((li) => {
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
    for (let i = 0; i < li1.Specs.length; i++) {
      const spec1 = li1.Specs[i];
      const spec2 = li2.Specs.find((s) => s.SpecID === spec1.SpecID);
      if (spec1.Value !== spec2.Value) return false;
    }
    return true;
  }
}
