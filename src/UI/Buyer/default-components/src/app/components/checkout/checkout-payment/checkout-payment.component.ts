import { Component, Output, EventEmitter, OnInit, Input } from '@angular/core';
import { ListBuyerCreditCard } from '@ordercloud/angular-sdk';
import { MarketplaceBuyerCreditCard } from 'marketplace';
import { OrderCloudIntegrationsCreditCardToken, MarketplaceOrder } from 'marketplace-javascript-sdk';
import { OrderSummaryMeta } from 'src/app/services/purchase-order.helper';

@Component({
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss'],
})
export class OCMCheckoutPayment implements OnInit {
  @Input() cards: ListBuyerCreditCard;
  @Input() isAnon: boolean;
  @Input() order: MarketplaceOrder;
  @Input() orderSummaryMeta: OrderSummaryMeta;
  @Output() cardSelected = new EventEmitter<SelectedCreditCard>();
  @Output() continue = new EventEmitter<void>();

  constructor() {}

  ngOnInit(): void {}

  onCardSelected(card: SelectedCreditCard): void {
    this.cardSelected.emit(card);
  }

  // used when no selection of card is required
  // only acknowledgement of purchase order is required
  onContinue(): void {
    this.continue.emit();
  }
}

export interface SelectedCreditCard {
  SavedCard?: MarketplaceBuyerCreditCard;
  NewCard?: OrderCloudIntegrationsCreditCardToken;
  CVV: string;
}
