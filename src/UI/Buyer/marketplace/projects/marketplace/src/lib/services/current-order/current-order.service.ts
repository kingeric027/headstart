// angular
import { Injectable } from '@angular/core';

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
import { Subject } from 'rxjs';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';
import { MarketplaceOrder, ProposedShipmentSelection, CreditCardToken, ListProposedShipment } from '../../shopper-context';
import { MiddlewareApiService } from '../middleware-api/middleware-api.service';
import { OrderStateService } from './order-state.service';

export interface ICurrentOrder {
  addToCartSubject: Subject<LineItem>;
  get(): MarketplaceOrder;
  onOrderChange(callback: (order: MarketplaceOrder) => void): void;
  onLineItemsChange(callback: (lineItems: ListLineItem) => void): void;
  
  submit(): Promise<void>;
  
  addComment(comment: string): Promise<MarketplaceOrder>;
  
  listPayments(): Promise<ListPayment>;
  createPayment(payment: Payment): Promise<Payment>;
  createSavedCCPayment(card: BuyerCreditCard): Promise<Payment>;
  createOneTimeCCPayment(card: CreditCardToken): Promise<Payment>;

  setBillingAddress(address: Address): Promise<MarketplaceOrder>;
  setShippingAddress(address: Address): Promise<MarketplaceOrder>;
  setBillingAddressByID(addressID: string): Promise<MarketplaceOrder>;
  setShippingAddressByID(addressID: string): Promise<MarketplaceOrder>;

  getProposedShipments(): Promise<ListProposedShipment>;
  selectShippingRate(selection: ProposedShipmentSelection): Promise<MarketplaceOrder>;
  calculateTax(): Promise<MarketplaceOrder>;
  authOnlyOnetimeCreditCard(card: CreditCardToken, cvv: string): Promise<Payment>;
  authOnlySavedCreditCard(cardID: string, cvv: string): Promise<Payment>;

}

@Injectable({
  providedIn: 'root',
})
export class CurrentOrderService implements ICurrentOrder {

  constructor(
    private ocOrderService: OcOrderService,
    private ocPaymentService: OcPaymentService,
    private paymentHelper: PaymentHelperService,
    private middlewareApi: MiddlewareApiService,
    private state: OrderStateService  
  ) {}

  get(): MarketplaceOrder {
    return this.order;
  }

  async submit(): Promise<void> {
    await this.ocOrderService.Submit('outgoing', this.order.ID).toPromise();
    await this.state.reset();
  }

  async addComment(comment: string): Promise<MarketplaceOrder> {
    return await this.patch({Comments: comment });
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

  // Integration Methods
  async getProposedShipments(): Promise<ListProposedShipment> {
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

  private get order(): MarketplaceOrder {
    return this.state.order;
}

private set order(value: MarketplaceOrder) {
  this.state.order = value;
}

  // Private Methods
  private async deleteExistingPayments(): Promise<any[]> {
    const payments = await this.ocPaymentService.List('outgoing', this.order.ID).toPromise();
    const deleteAll = payments.Items.map((payment) => this.ocPaymentService.Delete('outgoing', this.order.ID, payment.ID).toPromise());
    return Promise.all(deleteAll);
  }

  private async patch(order: MarketplaceOrder): Promise<MarketplaceOrder> {
    return (this.order = await this.ocOrderService.Patch('outgoing', this.order.ID, order).toPromise());
  }
}
