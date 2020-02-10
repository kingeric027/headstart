import { Component, ViewChild, OnInit } from '@angular/core';
import { faCheck } from '@fortawesome/free-solid-svg-icons';
import { NgbAccordion } from '@ng-bootstrap/ng-bootstrap';
import { ShopperContextService, MarketplaceOrder, ListPayment, ListLineItem, BuyerCreditCard, ProposedShipmentSelection, ProposedShipment, ListBuyerCreditCard } from 'marketplace';

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
  selectedCard: { card: BuyerCreditCard, cvv: string };
  proposedShipments: ProposedShipment[] = [];
  currentPanel: string;
  faCheck = faCheck;
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

  async ngOnInit() {
    this.context.currentOrder.onOrderChange(order => (this.order = order));
    this.order = this.context.currentOrder.get();
    this.lineItems = this.context.currentOrder.getLineItems();
    this.isAnon = this.context.currentUser.isAnonymous;
    this.currentPanel = this.isAnon ? 'login' : 'shippingAddress';
    this.setValidation('login', !this.isAnon);
  }

  async doneWithShipToAddress() {
    this.proposedShipments = await this.context.currentOrder.getProposedShipments();
    this.toSection('shippingSelection');
  }

  async onSelectShipRate(selection: ProposedShipmentSelection) {
    await this.context.currentOrder.selectShippingRate(selection);
  }

  async doneWithShippingRates() {
    await this.context.currentOrder.calculateTax();
    this.cards = await this.context.currentUser.cards.List();
    this.toSection('payment');
  }

  async onCardSelected(card: { card: BuyerCreditCard, cvv: string}) {
    this.selectedCard = card;
    await this.context.currentOrder.createCCPayment(card.card);
    this.payments = await this.context.currentOrder.listPayments();
    this.toSection('billingAddress');
  }

  async submitOrderWithComment(comment: string): Promise<void> {
    const orderID = this.order.ID;
    await this.context.currentOrder.patch({ Comments: comment });
    await this.context.currentOrder.authOnlyCreditCard(this.selectedCard.card.ID, this.selectedCard.cvv);
    await this.context.currentOrder.submit();

    // todo: "Order Submitted Successfully" message
    this.context.router.toMyOrderDetails(orderID);
  }

  getValidation(id: string) {
    return this.sections.find(x => x.id === id).valid;
  }

  setValidation(id: string, value: boolean) {
    this.sections.find(x => x.id === id).valid = value;
  }

  toSection(id: string) {
    const prevIdx = Math.max(this.sections.findIndex(x => x.id === id) - 1, 0);
    const prev = this.sections[prevIdx].id;
    this.setValidation(prev, true);
    this.accordian.toggle(id);
  }

  beforeChange($event) {
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
