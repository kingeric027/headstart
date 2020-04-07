import { MarketplaceOrder, AppConfig, MarketplaceBuyerCreditCard } from '../../shopper-context';
import {
  ListPayment,
  Payment,
  OcOrderService,
  OcPaymentService,
  BuyerAddress,
  OcMeService,
} from '@ordercloud/angular-sdk';
import { Injectable } from '@angular/core';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';
import { OrderStateService } from './order-state.service';
import { OrderWorksheet, ShipMethodSelection } from '../ordercloud-sandbox/ordercloud-sandbox.models';
import { OrderCloudSandboxService } from '../ordercloud-sandbox/ordercloud-sandbox.service';
import { MarketplaceSDK, CreditCardToken, CreditCardPayment } from 'marketplace-javascript-sdk';

export interface ICheckout {
  submit(card: CreditCardPayment, marketplaceID: string): Promise<string>;
  addComment(comment: string): Promise<MarketplaceOrder>;
  listPayments(): Promise<ListPayment>;
  createSavedCCPayment(card: MarketplaceBuyerCreditCard): Promise<Payment>;
  createOneTimeCCPayment(card: CreditCardToken): Promise<Payment>;
  setShippingAddress(address: BuyerAddress): Promise<MarketplaceOrder>;
  setShippingAddressByID(addressID: string): Promise<MarketplaceOrder>;
  setBuyerLocationByID(buyerLocationID: string): Promise<MarketplaceOrder>;
  estimateShipping(): Promise<OrderWorksheet>;
  selectShipMethod(selection: ShipMethodSelection): Promise<OrderWorksheet>;
  calculateOrder(): Promise<MarketplaceOrder>;
}

@Injectable({
  providedIn: 'root',
})
export class CheckoutService implements ICheckout {
  constructor(
    private ocOrderService: OcOrderService,
    private ocPaymentService: OcPaymentService,
    private ocMeService: OcMeService,
    private paymentHelper: PaymentHelperService,
    private appSettings: AppConfig,
    private state: OrderStateService,
    private orderCloudSandBoxService: OrderCloudSandboxService,
    private appConfig: AppConfig
  ) {}

  async submit(payment: CreditCardPayment): Promise<string> {
    // TODO - auth call on submit probably needs to be enforced in the middleware, not frontend.;
    await MarketplaceSDK.MePayments.Post(payment); // authorize card
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

  async setShippingAddress(address: BuyerAddress): Promise<MarketplaceOrder> {
    // If a saved address (with an ID) is changed by the user it is attached to an order as a one time address.
    // However, order.ShippingAddressID (or BillingAddressID) still points to the unmodified address. The ID should be cleared.
    address.ID = null;
    this.order = await this.ocOrderService.SetShippingAddress('outgoing', this.order.ID, address).toPromise();
    return this.order;
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

  async setBuyerLocationByID(buyerLocationID: string): Promise<MarketplaceOrder> {
    const patch = { BillingAddressID: buyerLocationID, xp: { ApprovalNeeded: '' } };
    const isApprovalNeeded = await this.isApprovalNeeded(buyerLocationID);
    if (isApprovalNeeded) patch.xp.ApprovalNeeded = buyerLocationID;
    try {
      this.order = await this.patch(patch as MarketplaceOrder);
      return this.order;
    } catch (ex) {
      if (ex.error.Errors[0].ErrorCode === 'NotFound') {
        throw Error('You no longer have access to this buyer location. Please enter or select a different one.');
      }
    }
  }

  async isApprovalNeeded(locationID: string): Promise<boolean> {
    const userGroups = await this.ocMeService.ListUserGroups({ searchOn: 'ID', search: locationID }).toPromise();
    return userGroups.Items.some(u => u.ID === `${locationID}-NeedsApproval`);
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

  async selectShipMethod(selection: ShipMethodSelection): Promise<OrderWorksheet> {
    const orderWorksheet = await this.orderCloudSandBoxService.selectShipMethod(this.order.ID, selection);
    this.order = orderWorksheet.Order;
    return orderWorksheet;
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
