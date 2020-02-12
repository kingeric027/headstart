// angular
import { Injectable, Inject } from '@angular/core';

// third party
import {
  OcMeService,
  OcOrderService,
  ListLineItem,
  OcLineItemService,
  Address,
  ListPayment,
  Payment,
  OcPaymentService,
  LineItem,
  BuyerCreditCard,
} from '@ordercloud/angular-sdk';
import { BehaviorSubject, Subject } from 'rxjs';
import { isUndefined as _isUndefined, get as _get } from 'lodash';
import { TokenHelperService } from '../token-helper/token-helper.service';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';
import { listAll } from '../../functions/listAll';
import { AppConfig, MarketplaceOrder, ProposedShipment, ProposedShipmentSelection, CreditCardToken } from '../../shopper-context';
import { MiddlewareApiService } from '../middleware-api/middleware-api.service';

export interface ICurrentOrder {
  addToCartSubject: Subject<LineItem>;
  get(): MarketplaceOrder;
  patch(order: MarketplaceOrder): Promise<MarketplaceOrder>;
  getLineItems(): ListLineItem;
  submit(): Promise<void>;

  addToCart(lineItem: LineItem): Promise<LineItem>;
  addManyToCart(lineItem: LineItem[]): Promise<LineItem[]>;
  setQuantityInCart(lineItemID: string, newQuantity: number): Promise<LineItem>;
  removeFromCart(lineItemID: string): Promise<void>;
  emptyCart(): Promise<void>;

  listPayments(): Promise<ListPayment>;
  createPayment(payment: Payment): Promise<Payment>;
  createSavedCCPayment(card: BuyerCreditCard): Promise<Payment>;
  createOneTimeCCPayment(card: CreditCardToken): Promise<Payment>;

  setBillingAddress(address: Address): Promise<MarketplaceOrder>;
  setShippingAddress(address: Address): Promise<MarketplaceOrder>;
  setBillingAddressByID(addressID: string): Promise<MarketplaceOrder>;
  setShippingAddressByID(addressID: string): Promise<MarketplaceOrder>;

  getProposedShipments(): Promise<ProposedShipment[]>;
  selectShippingRate(selection: ProposedShipmentSelection): Promise<MarketplaceOrder>;
  calculateTax(): Promise<MarketplaceOrder>;
  authOnlyOnetimeCreditCard(card: CreditCardToken, cvv: string): Promise<Payment>;
  authOnlySavedCreditCard(cardID: string, cvv: string): Promise<Payment>;

  onOrderChange(callback: (order: MarketplaceOrder) => void): void;
  onLineItemsChange(callback: (lineItems: ListLineItem) => void): void;
}

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService implements ICurrentOrder {
  private readonly DefaultOrder: MarketplaceOrder = { xp: { AvalaraTaxTransactionCode: '', ProposedShipmentSelections: [] }};
  private readonly DefaultLineItems: ListLineItem = {
    Meta: { Page: 1, PageSize: 25, TotalCount: 0, TotalPages: 1 },
    Items: [],
  };
  private initializingOrder = false;
  public addToCartSubject = new Subject<LineItem>(); // need to make available as observable
  private orderSubject = new BehaviorSubject<MarketplaceOrder>(null);
  private lineItemSubject = new BehaviorSubject<ListLineItem>(this.DefaultLineItems);

  constructor(
    private ocOrderService: OcOrderService,
    private ocLineItemService: OcLineItemService,
    private ocMeService: OcMeService,
    private tokenHelper: TokenHelperService,
    private ocPaymentService: OcPaymentService,
    private paymentHelper: PaymentHelperService,
    private middlewareApi: MiddlewareApiService,
    private appConfig: AppConfig
  ) {}

  onOrderChange(callback: (order: MarketplaceOrder) => void) {
    this.orderSubject.subscribe(callback);
  }

  get(): MarketplaceOrder {
    return this.order;
  }

  async patch(order: MarketplaceOrder): Promise<MarketplaceOrder> {
    return (this.order = await this.ocOrderService.Patch('outgoing', this.order.ID, order).toPromise());
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

  async setBillingAddress(address: Address): Promise<MarketplaceOrder> {
    return (this.order = await this.ocOrderService.SetBillingAddress('outgoing', this.order.ID, address).toPromise());
  }

  async setShippingAddress(address: Address): Promise<MarketplaceOrder> {
    return (this.order = await this.ocOrderService.SetShippingAddress('outgoing', this.order.ID, address).toPromise());
  }

  async setBillingAddressByID(addressID: string): Promise<MarketplaceOrder> {
    try {
      return await this.patch({ BillingAddressID: addressID });
    } catch (ex) {
      if (ex.error.Errors[0].ErrorCode === 'NotFound') {
        throw Error('You no longer have access to this saved address. Please enter or select a different one.');
      }
    }
  }

  async setShippingAddressByID(addressID: string): Promise<MarketplaceOrder> {
    try {
      return await this.patch({ ShippingAddressID: addressID });
    } catch (ex) {
      if (ex.error.Errors[0].ErrorCode === 'NotFound') {
        throw Error('You no longer have access to this saved address. Please enter or select a different one.');
      }
    }
  }

  async listPayments(): Promise<ListPayment> {
    return await this.paymentHelper.ListPaymentsOnOrder(this.order.ID);
  }

  async createSavedCCPayment(card: BuyerCreditCard): Promise<Payment> {
    return await this.createPayment({
      Amount : this.order.Total,
      DateCreated: new Date().toDateString(),
      Accepted: false,
      Type: 'CreditCard',
      CreditCardID: card.ID,
      xp: {
        partialAccountNumber: card.PartialAccountNumber,
        cardType: card.CardType
      }
    });
  }

  async createOneTimeCCPayment(card: CreditCardToken): Promise<Payment> {
    return await this.createPayment({
      Amount : this.order.Total,
      DateCreated: new Date().toDateString(),
      Accepted: false,
      Type: 'CreditCard',
      CreditCardID: null,
      xp: {
        // This slice() is sooo crucial. Otherwise we would be storing creditcard numbers in xp.
        // Which would be really really bad.
        partialAccountNumber: card.AccountNumber.slice(-4),
        cardType: card.CardType
      }
    });
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
      this.order = { ID: this.tokenHelper.getAnonymousOrderID() };
    } else {
      this.order = await this.ocOrderService.Create('outgoing', this.DefaultOrder).toPromise();
    }
    if (this.order.DateCreated) {
      this.lineItems = await listAll(this.ocLineItemService, this.ocLineItemService.List, 'outgoing', this.order.ID);
    }
  }

  // Cart Methods

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
      return await this.patchLineItem(lineItemID, { Quantity: newQuantity });
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

  // Integration Methods

  async getProposedShipments(): Promise<ProposedShipment[]> {
    return await this.middlewareApi.getProposedShipments(this.order.ID);
  }

  async selectShippingRate(selection: ProposedShipmentSelection): Promise<MarketplaceOrder> {
    return (this.order = await this.middlewareApi.selectShippingRate(this.order.ID, selection));
  }

  async calculateTax(): Promise<MarketplaceOrder> {
    return (this.order = await this.middlewareApi.calculateTax(this.order.ID));
  }

  async authOnlyOnetimeCreditCard(card: CreditCardToken, cvv: string): Promise<Payment> {
    return await this.middlewareApi.authOnlyCreditCard(this.order.ID, card, cvv);
  }

  async authOnlySavedCreditCard(cardID: string, cvv: string): Promise<Payment> {
    return await this.middlewareApi.authOnlySavedCreditCard(this.order.ID, cardID, cvv);
  }


  // Private Methods

  private get order(): MarketplaceOrder {
    return this.orderSubject.value;
  }

  private set order(value: MarketplaceOrder) {
    this.orderSubject.next(value);
  }

  private get lineItems(): ListLineItem {
    return this.lineItemSubject.value;
  }

  private set lineItems(value: ListLineItem) {
    this.lineItemSubject.next(value);
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

  private calculateOrder(): MarketplaceOrder {
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
    for (const spec1 of  li1.Specs) {
      const spec2 = li2.Specs.find((s) => s.SpecID === spec1.SpecID);
      if (spec1.Value !== spec2.Value) return false;
    }
    return true;
  }
}
