import {
  MarketplaceOrder,
  OrderAddressType,
  CreditCardPayment,
  AppConfig,
  MarketplaceBuyerCreditCard,
} from '../../shopper-context';
import { ListPayment, Payment, OcOrderService, OcPaymentService, BuyerAddress } from '@ordercloud/angular-sdk';
import { Injectable } from '@angular/core';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';
import { OrderStateService } from './order-state.service';
import { OrderWorksheet, ShipMethodSelection } from '../ordercloud-sandbox/ordercloud-sandbox.models';
import { OrderCloudSandboxService } from '../ordercloud-sandbox/ordercloud-sandbox.service';
import { MarketplaceSDK, CreditCardToken } from 'marketplace-javascript-sdk';

export interface ICheckout {
  submit(card: CreditCardPayment, marketplaceID: string): Promise<string>;
  addComment(comment: string): Promise<MarketplaceOrder>;
  listPayments(): Promise<ListPayment>;
  createSavedCCPayment(card: MarketplaceBuyerCreditCard): Promise<Payment>;
  createOneTimeCCPayment(card: CreditCardToken): Promise<Payment>;
  setAddress(type: OrderAddressType, address: BuyerAddress): Promise<MarketplaceOrder>;
  setAddressByID(type: OrderAddressType, addressID: string): Promise<MarketplaceOrder>;
  setBuyerLocationByID(buyerLocationID: string): Promise<MarketplaceOrder>;
  estimateShipping(): Promise<OrderWorksheet>;
  selectShipMethod(selection: ShipMethodSelection): Promise<MarketplaceOrder>;
  calculateOrder(): Promise<MarketplaceOrder>;
}

@Injectable({
  providedIn: 'root',
})
export class CheckoutService implements ICheckout {
  constructor(
    private ocOrderService: OcOrderService,
    private ocPaymentService: OcPaymentService,
    private paymentHelper: PaymentHelperService,
    private appSettings: AppConfig,
    private state: OrderStateService,
    private orderCloudSandBoxService: OrderCloudSandboxService,
    private appConfig: AppConfig
  ) {}

  async submit(payment: CreditCardPayment): Promise<string> {
    // TODO - auth call on submit probably needs to be enforced in the middleware, not frontend.
    const ccPayment = {
      OrderId: this.order.ID,
      CreditCardID: payment?.SavedCard.ID,
      CreditCardDetails: payment.NewCard,
      Currency: 'USD',
      CVV: payment.CVV,
      MerchantID: this.appSettings.cardConnectMerchantID,
    };
    await MarketplaceSDK.MePayments.Post(ccPayment); // authorize card
    const orderWithCleanID = await this.ocOrderService
      .Patch('outgoing', this.order.ID, {
        ID: `${this.appConfig.marketplaceID}{orderIncrementor}`,
      })
      .toPromise();
    this.order = orderWithCleanID;
    await this.ocOrderService.Submit('outgoing', orderWithCleanID.ID).toPromise();
    await this.state.reset();
    return orderWithCleanID.ID;
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

  async setBuyerLocationByID(buyerLocationID: string): Promise<MarketplaceOrder> {
    const patch = { xp: { ['BuyerLocationID']: buyerLocationID } };
    try {
      return await this.patch(patch as MarketplaceOrder);
    } catch (ex) {
      if (ex.error.Errors[0].ErrorCode === 'NotFound') {
        throw Error('You no longer have access to this buyer location. Please enter or select a different one.');
      }
    }
  }

  async listPayments(): Promise<ListPayment> {
    return await this.paymentHelper.ListPaymentsOnOrder(this.order.ID);
  }

  async createSavedCCPayment(card: MarketplaceBuyerCreditCard): Promise<Payment> {
    return await this.createCCPayment(card.PartialAccountNumber, card.CardType, card.ID);
  }

  async createOneTimeCCPayment(card: CreditCardToken): Promise<Payment> {
    // This slice() is sooo crucial. Otherwise we would be storing creditcard numbers in xp.
    // Which would be really really bad.
    const partialAccountNum = card.AccountNumber.slice(-4);
    return await this.createCCPayment(partialAccountNum, card.CardType, null);
  }

  // Integration Methods
  // order cloud sandbox service methods, to be replaced by updated sdk in the future
  async estimateShipping(): Promise<OrderWorksheet> {
    return await this.orderCloudSandBoxService.estimateShipping(this.order.ID);
  }

  async selectShipMethod(selection: ShipMethodSelection): Promise<MarketplaceOrder> {
    const orderCalculation = await this.orderCloudSandBoxService.selectShipMethod(this.order.ID, selection);
    this.order = orderCalculation.Order;
    return this.order;
  }

  async calculateOrder(): Promise<MarketplaceOrder> {
    const orderCalculation = await this.orderCloudSandBoxService.calculateOrder(this.order.ID);
    this.order = orderCalculation.Order;
    return this.order;
  }

  // Private Methods

  private async createCCPayment(partialAccountNum: string, cardType: string, creditCardID: string): Promise<Payment> {
    await this.deleteExistingPayments(); // TODO - is this still needed? There used to be an OC bug with multiple payments on an order.
    const payment = {
      Amount: this.order.Total,
      DateCreated: new Date().toDateString(),
      Accepted: false,
      Type: 'CreditCard',
      CreditCardID: creditCardID,
      xp: {
        partialAccountNumber: partialAccountNum,
        cardType: cardType,
      },
    };
    return await this.ocPaymentService.Create('outgoing', this.order.ID, payment).toPromise();
  }

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
