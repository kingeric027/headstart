import { MarketplaceOrder, CreditCardToken, OrderAddressType } from '../../shopper-context';
import {
  ListPayment,
  Payment,
  BuyerCreditCard,
  OcOrderService,
  OcPaymentService,
  Address,
  BuyerAddress,
} from '@ordercloud/angular-sdk';
import { Injectable } from '@angular/core';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';
import { MiddlewareApiService } from '../middleware-api/middleware-api.service';
import { OrderStateService } from './order-state.service';
import { OrderCalculation, ShipmentPreference } from '../ordercloud-sandbox/ordercloud-sandbox.models';
import { OrderCloudSandboxService } from '../ordercloud-sandbox/ordercloud-sandbox.service';

export interface ICheckout {
  submit(): Promise<void>;
  addComment(comment: string): Promise<MarketplaceOrder>;
  listPayments(): Promise<ListPayment>;
  createPayment(payment: Payment): Promise<Payment>;
  createSavedCCPayment(card: BuyerCreditCard): Promise<Payment>;
  createOneTimeCCPayment(card: CreditCardToken): Promise<Payment>;
  setAddressByID(type: OrderAddressType, addressID: string): Promise<MarketplaceOrder>;
  setAddress(type: OrderAddressType, address: BuyerAddress): Promise<MarketplaceOrder>;
  getProposedShipments(): Promise<OrderCalculation>;
  selectShippingRate(selection: ShipmentPreference): Promise<MarketplaceOrder>;
  calculateOrder(): Promise<MarketplaceOrder>;
  authOnlyOnetimeCreditCard(card: CreditCardToken, cvv: string): Promise<Payment>;
  authOnlySavedCreditCard(cardID: string, cvv: string): Promise<Payment>;
}

@Injectable({
  providedIn: 'root',
})
export class CheckoutService implements ICheckout {
  constructor(
    private ocOrderService: OcOrderService,
    private ocPaymentService: OcPaymentService,
    private paymentHelper: PaymentHelperService,
    private middlewareApi: MiddlewareApiService,
    private state: OrderStateService,
    private orderCloudSandBoxService: OrderCloudSandboxService
  ) {}

  async submit(): Promise<void> {
    await this.ocOrderService.Submit('outgoing', this.order.ID).toPromise();
    await this.state.reset();
  }

  async addComment(comment: string): Promise<MarketplaceOrder> {
    return await this.patch({ Comments: comment });
  }

  async setAddress(type: OrderAddressType, address: BuyerAddress): Promise<MarketplaceOrder> {
    if (type === OrderAddressType.Billing) {
      this.order = await this.ocOrderService.SetBillingAddress('outgoing', this.order.ID, address).toPromise();
    } else if (type === OrderAddressType.Shipping) {
      this.order = await this.ocOrderService.SetShippingAddress('outgoing', this.order.ID, address).toPromise();
    }
    return this.order;
  }

  async setAddressByID(type: OrderAddressType, addressID: string): Promise<MarketplaceOrder> {
    const patch = { [`${type.toString()}AddressID`]: addressID };
    try {
      return await this.patch(patch);
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
      Amount: this.order.Total,
      DateCreated: new Date().toDateString(),
      Accepted: false,
      Type: 'CreditCard',
      CreditCardID: card.ID,
      xp: {
        partialAccountNumber: card.PartialAccountNumber,
        cardType: card.CardType,
      },
    });
  }

  async createOneTimeCCPayment(card: CreditCardToken): Promise<Payment> {
    return await this.createPayment({
      Amount: this.order.Total,
      DateCreated: new Date().toDateString(),
      Accepted: false,
      Type: 'CreditCard',
      CreditCardID: null,
      xp: {
        // This slice() is sooo crucial. Otherwise we would be storing creditcard numbers in xp.
        // Which would be really really bad.
        partialAccountNumber: card.AccountNumber.slice(-4),
        cardType: card.CardType,
      },
    });
  }

  async createPayment(payment: Payment): Promise<Payment> {
    await this.deleteExistingPayments(); // TODO - is this still needed? There used to be an OC bug with multiple payments on an order.
    return await this.ocPaymentService.Create('outgoing', this.order.ID, payment).toPromise();
  }

  // Integration Methods
  // order cloud sandbox service methods, to be replaced by updated sdk in the future
  async getProposedShipments(): Promise<OrderCalculation> {
    return await this.orderCloudSandBoxService.calculateShippingOptions(this.order.ID);
  }

  async selectShippingRate(selection: ShipmentPreference): Promise<MarketplaceOrder> {
    const orderCalculation = await this.orderCloudSandBoxService.selectShippingRate(this.order.ID, selection);
    this.order = orderCalculation.Order;
    return this.order;
  }

  async calculateOrder(): Promise<MarketplaceOrder> {
    const orderCalculation = await this.orderCloudSandBoxService.calculateOrder(this.order.ID);
    this.order = await orderCalculation.Order;
    return this.order;
  }

  async authOnlyOnetimeCreditCard(card: CreditCardToken, cvv: string): Promise<Payment> {
    return await this.middlewareApi.authOnlyCreditCard(this.order.ID, card, cvv);
  }

  async authOnlySavedCreditCard(cardID: string, cvv: string): Promise<Payment> {
    return await this.middlewareApi.authOnlySavedCreditCard(this.order.ID, cardID, cvv);
  }

  // Private Methods
  private async deleteExistingPayments(): Promise<any[]> {
    const payments = await this.ocPaymentService.List('outgoing', this.order.ID).toPromise();
    const deleteAll = payments.Items.map(payment =>
      this.ocPaymentService.Delete('outgoing', this.order.ID, payment.ID).toPromise()
    );
    return Promise.all(deleteAll);
  }

  private async patch(order: MarketplaceOrder): Promise<MarketplaceOrder> {
    return (this.order = await this.ocOrderService.Patch('outgoing', this.order.ID, order).toPromise());
  }

  private get order(): MarketplaceOrder {
    return this.state.order;
  }

  private set order(value: MarketplaceOrder) {
    this.state.order = value;
  }
}
