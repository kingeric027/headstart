import { Component, ViewChild, OnInit } from '@angular/core';
import { faCheck } from '@fortawesome/free-solid-svg-icons';
import { NgbAccordion } from '@ng-bootstrap/ng-bootstrap';
import {
  ShopperContextService,
  MarketplaceOrder,
  ListPayment,
  ListLineItem,
  ListBuyerCreditCard,
  ShipMethodSelection,
  ShipEstimate,
  CreditCardPayment,
} from 'marketplace';
import { CheckoutService } from 'marketplace/projects/marketplace/src/lib/services/order/checkout.service';

@Component({
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class OCMCheckout implements OnInit {
  @ViewChild('acc', { static: false }) public accordian: NgbAccordion;
  isAnon: boolean;
  order: MarketplaceOrder;
  lineItems: ListLineItem;
  payments: ListPayment;
  cards: ListBuyerCreditCard;
  selectedCard: CreditCardPayment;
  shipEstimates: ShipEstimate[] = null;
  currentPanel: string;
  faCheck = faCheck;
  checkout: CheckoutService = this.context.order.checkout;
  sections: any = [
    {
      id: 'login',
      valid: false,
    },
    {
      id: 'shippingAddress',
      valid: false,
    },
    {
      id: 'shippingSelection',
      valid: false,
    },
    {
      id: 'payment',
      valid: false,
    },
    {
      id: 'billingAddress',
      valid: false,
    },
    {
      id: 'confirm',
      valid: false,
    },
  ];

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.context.order.onChange(order => (this.order = order));
    this.order = this.context.order.get();
    this.lineItems = this.context.order.cart.get();
    this.isAnon = this.context.currentUser.isAnonymous();
    this.currentPanel = this.isAnon ? 'login' : 'shippingAddress';
    this.setValidation('login', !this.isAnon);
  }

  async doneWithShipToAddress(): Promise<void> {
    const orderWorksheet = await this.checkout.estimateShipping();
    this.shipEstimates = orderWorksheet.ShipEstimateResponse.ShipEstimates;
    this.toSection('shippingSelection');
  }

  async selectShipMethod(selection: ShipMethodSelection): Promise<void> {
    await this.checkout.selectShipMethod(selection);
  }

  async doneWithShippingRates(): Promise<void> {
    await this.checkout.calculateOrder();
    this.cards = await this.context.currentUser.cards.List();
    this.toSection('payment');
  }

  async onCardSelected(output: CreditCardPayment): Promise<void> {
    this.selectedCard = output;
    if (output.SavedCard) {
      await this.checkout.createSavedCCPayment(output.SavedCard);
      delete this.selectedCard.NewCard;
    } else {
      // need to figure out how to use the platform. ran into creditCardID cannot be null.
      // so for now I always save any credit card in OC.
      // await this.context.currentOrder.createOneTimeCCPayment(output.newCard);
      this.selectedCard.SavedCard = await this.context.currentUser.cards.Save(output.NewCard);
      await this.checkout.createSavedCCPayment(this.selectedCard.SavedCard);
    }

    this.payments = await this.checkout.listPayments();
    this.toSection('billingAddress');
  }

  async submitOrderWithComment(comment: string): Promise<void> {
    const orderID = this.order.ID; // submit() will reset this.order
    await this.checkout.addComment(comment);
    const cleanOrderID = await this.checkout.submit(this.selectedCard, this.context.appSettings.marketplaceID);

    // todo: "Order Submitted Successfully" message
    this.context.router.toMyOrderDetails(cleanOrderID);
  }

  getValidation(id: string): any {
    return this.sections.find(x => x.id === id).valid;
  }

  setValidation(id: string, value: boolean): void {
    this.sections.find(x => x.id === id).valid = value;
  }

  toSection(id: string): void {
    const prevIdx = Math.max(this.sections.findIndex(x => x.id === id) - 1, 0);
    const prev = this.sections[prevIdx].id;
    this.setValidation(prev, true);
    this.accordian.toggle(id);
  }

  beforeChange($event): any {
    if (this.currentPanel === $event.panelId) {
      return $event.preventDefault();
    }

    // Only allow a section to open if all previous sections are valid
    for (const section of this.sections) {
      if (section.id === $event.panelId) {
        break;
      }

      if (!section.valid) {
        return $event.preventDefault();
      }
    }
    this.currentPanel = $event.panelId;
  }
}
