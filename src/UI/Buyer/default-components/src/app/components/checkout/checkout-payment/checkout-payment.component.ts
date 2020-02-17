import { Component, Output, EventEmitter, OnInit, Input } from '@angular/core';
import {  ListBuyerCreditCard } from '@ordercloud/angular-sdk';
import { FormGroup, FormControl } from '@angular/forms';
import {  MarketplaceOrder } from 'marketplace';
import { CheckoutCreditCardOutput } from '../../payments/payment-credit-card/payment-credit-card.component';

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
  @Output() cardSelected = new EventEmitter<CheckoutCreditCardOutput>();
  
  constructor() {}
  
  ngOnInit(): void {}

  onCardSelected(card: CheckoutCreditCardOutput): void {
    this.cardSelected.emit(card);
  }
}
