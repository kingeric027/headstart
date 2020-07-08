import { AppConfig, MarketplaceBuyerCreditCard } from '../../shopper-context';
import {
  Payment,
  Orders,
  Payments,
  BuyerAddress,
  Me,
  OrderWorksheet,
  IntegrationEvents,
  ShipMethodSelection,
  LineItem,
  LineItems,
} from 'ordercloud-javascript-sdk';
import { Injectable } from '@angular/core';
import { PaymentHelperService } from '../payment-helper/payment-helper.service';
import { OrderStateService } from './order-state.service';
import {
  MarketplaceSDK,
  Address,
  OrderCloudIntegrationsCreditCardPayment,
  OrderCloudIntegrationsCreditCardToken,
  MarketplaceOrder,
  ListPage,
  MarketplaceLineItem,
} from 'marketplace-javascript-sdk';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  constructor(
    private paymentHelper: PaymentHelperService,
    private appSettings: AppConfig,
    private state: OrderStateService,
    private appConfig: AppConfig
  ) {}

  async submitWithCreditCard(payment: OrderCloudIntegrationsCreditCardPayment): Promise<string> {
    // TODO - auth call on submit probably needs to be enforced in the middleware, not frontend.;
    await MarketplaceSDK.MePayments.Post(payment); // authorize card
    const orderID = this.submit();
    return orderID;
  }

  async submitWithoutCreditCard(): Promise<string> {
    const orderID = this.submit();
    return orderID;
  }

  async addComment(comment: string): Promise<MarketplaceOrder> {
    return await this.patch({ Comments: comment });
  }

  async incrementOrderIfNeeded(): Promise<void> {
    // 'as any' can be removed after sdk update
    if (!(this.order.xp as any)?.IsResubmitting) {
      this.order = (await Orders.Patch('Outgoing', this.order.ID, {
        ID: `${this.appConfig.marketplaceID}{orderIncrementor}`,
      })) as MarketplaceOrder;
    }
  }

  async setShippingAddress(address: BuyerAddress): Promise<MarketplaceOrder> {
    // If a saved address (with an ID) is changed by the user it is attached to an order as a one time address.
    // However, order.ShippingAddressID (or BillingAddressID) still points to the unmodified address. The ID should be cleared.
    (address as any).ID = null;
    this.order = await MarketplaceSDK.ValidatedAddresses.SetShippingAddress(
      'Outgoing',
      this.order.ID,
      address as Address
    );
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
    const userGroups = await Me.ListUserGroups({ searchOn: 'ID', search: locationID });
    return userGroups.Items.some(u => u.ID === `${locationID}-NeedsApproval`);
  }

  async listPayments(): Promise<ListPage<Payment>> {
    return await this.paymentHelper.ListPaymentsOnOrder(this.order.ID);
  }

  async createSavedCCPayment(card: MarketplaceBuyerCreditCard, amount: number): Promise<Payment> {
    return await this.createCCPayment(card.PartialAccountNumber, card.CardType, card.ID, amount);
  }

  async createOneTimeCCPayment(card: OrderCloudIntegrationsCreditCardToken, amount: number): Promise<Payment> {
    // This slice() is sooo crucial. Otherwise we would be storing creditcard numbers in xp.
    // Which would be really really bad.
    const partialAccountNum = card.AccountNumber.slice(-4);
    return await this.createCCPayment(partialAccountNum, card.CardType, null, amount);
  }

  // Integration Methods
  // order cloud sandbox service methods, to be replaced by updated sdk in the future
  async estimateShipping(): Promise<OrderWorksheet> {
    return await IntegrationEvents.EstimateShipping('Outgoing', this.order.ID);
  }

  async cleanLineItemIDs(orderID: string, lineItems: MarketplaceLineItem[]): Promise<void> {
    /* line item ids are significant for suppliers creating a relationship
     * between their shipments and line items in ordercloud
     * we are sequentially labeling these ids for ease of shipping */
    const lineItemIDChanges = lineItems.map((li, index) => {
      return LineItems.Patch('Outgoing', orderID, li.ID, { ID: this.createIDFromIndex(index) });
    });
    await Promise.all(lineItemIDChanges);
    await this.state.resetLineItems();
  }

  createIDFromIndex(index: number): string {
    /* X was choosen as a prefix for the lineItem ID so that it is easy to
     * direct suppliers where to look for the ID. L and I are sometimes indistinguishable
     * from the number 1 so I avoided those. X is also difficult to confuse with other
     * letters when verbally pronounced */
    const countInList = index + 1;
    const paddedCount = countInList.toString().padStart(3, '0');
    return 'X' + paddedCount;
  }

  async selectShipMethods(selections: ShipMethodSelection[]): Promise<OrderWorksheet> {
    const orderWorksheet = await IntegrationEvents.SelectShipmethods('Outgoing', this.order.ID, {
      ShipMethodSelections: selections,
    });
    this.order = orderWorksheet.Order;
    return orderWorksheet;
  }

  async calculateOrder(): Promise<MarketplaceOrder> {
    const orderCalculation = await IntegrationEvents.Calculate('Outgoing', this.order.ID);
    this.order = orderCalculation.Order;
    return this.order;
  }

  async createPurchaseOrderPayment(amount: number): Promise<Payment> {
    const payment: Payment = {
      Amount: amount,
      DateCreated: new Date().toDateString(),
      Type: 'PurchaseOrder',
    };
    return await Payments.Create('Outgoing', this.order.ID, payment);
  }

  async deleteExistingPayments(): Promise<any[]> {
    const payments = await Payments.List('Outgoing', this.order.ID);
    const deleteAll = payments.Items.map(payment => Payments.Delete('Outgoing', this.order.ID, payment.ID));
    return Promise.all(deleteAll);
  }

  // Private Methods
  private async submit(): Promise<string> {
    // TODO - auth call on submit probably needs to be enforced in the middleware, not frontend.;
    await this.incrementOrderIfNeeded();
    const submittedOrder = await Orders.Submit('Outgoing', this.order.ID);
    await this.state.reset();
    return submittedOrder.ID;
  }

  private async createCCPayment(
    partialAccountNum: string,
    cardType: string,
    creditCardID: string,
    amount: number
  ): Promise<Payment> {
    const payment: Payment = {
      Amount: amount,
      DateCreated: new Date().toDateString(),
      Accepted: false,
      Type: 'CreditCard',
      CreditCardID: creditCardID,
      xp: {
        partialAccountNumber: partialAccountNum,
        cardType,
      },
    };
    return await Payments.Create('Outgoing', this.order.ID, payment);
  }

  private async patch(order: MarketplaceOrder): Promise<MarketplaceOrder> {
    this.order = (await Orders.Patch('Outgoing', this.order.ID, order)) as MarketplaceOrder;
    return this.order;
  }

  private get order(): MarketplaceOrder {
    return this.state.order;
  }

  private set order(value: MarketplaceOrder) {
    this.state.order = value;
  }
}
