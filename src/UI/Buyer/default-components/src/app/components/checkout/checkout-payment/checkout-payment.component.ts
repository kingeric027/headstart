import { Component, Output, EventEmitter, OnInit, Input } from '@angular/core';
import { ListBuyerCreditCard } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import { MarketplaceOrder, MarketplaceBuyerCreditCard } from 'marketplace';
import { OrderCloudIntegrationsCreditCardToken } from 'marketplace-javascript-sdk';

@Component({
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss'],
})
export class OCMCheckoutPayment implements OnInit {
  availablePaymentMethods = ['CreditCard'];
  selectedPaymentMethod = 'CreditCard';
  form = new FormGroup({
    selectedPaymentMethod: new FormControl({ value: this.selectedPaymentMethod, disabled: true }),
  });

  @Input() cards: ListBuyerCreditCard;
  @Input() isAnon: boolean;
  @Input() order: MarketplaceOrder;
  @Output() cardSelected = new EventEmitter<SelectedCreditCard>();

  constructor() {}

  ngOnInit(): void {}

  onCardSelected(card: SelectedCreditCard): void {
    this.cardSelected.emit(card);
  }
}

export interface SelectedCreditCard {
  SavedCard?: MarketplaceBuyerCreditCard;
  NewCard?: OrderCloudIntegrationsCreditCardToken;
  CVV: string;
}
