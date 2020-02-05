import { Component, Output, EventEmitter, OnInit, Input } from '@angular/core';
import { Payment, ListBuyerCreditCard, BuyerCreditCard } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import { ShopperContextService, MarketplaceOrder } from 'marketplace';

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
  @Output() cardSelected = new EventEmitter<{cardID: string, cvv: string}>();
  
  constructor() {}
  
  ngOnInit() {}

  onCardSelected(card: {cardID: string, cvv: string}) {
    this.cardSelected.emit(card);
  }
}
