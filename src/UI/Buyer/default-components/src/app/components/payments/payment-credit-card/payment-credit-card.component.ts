import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ListBuyerCreditCard, BuyerCreditCard, CreditCardToken } from 'marketplace';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { CreditCardFormOutput } from '../credit-card-form/credit-card-form.component';

export interface CheckoutCreditCardOutput {
  savedCard?: BuyerCreditCard;
  newCard?: CreditCardToken;
  cvv: string;
}

@Component({
  templateUrl: './payment-credit-card.component.html',
  styleUrls: ['./payment-credit-card.component.scss'],
})
export class OCMPaymentCreditCard implements OnInit {
  showNewCCForm = false;

  @Input() cards: ListBuyerCreditCard;
  @Output() cardSelected = new EventEmitter<CheckoutCreditCardOutput>();
  form = new FormGroup({
    cardID: new FormControl(null, Validators.required),
    cvv: new FormControl('', Validators.required),
  });

  constructor() {}

  ngOnInit(): void {}

  submit(output: CreditCardFormOutput): void {
    const cardID = this.form.value.cardID;
    const savedCard = this.cards.Items.find(c => c.ID === cardID);
    this.cardSelected.emit({ savedCard, cvv: output.cvv, newCard: output.card });
  }

  toggleShowCCForm(event): void {
    this.showNewCCForm = event.target.value === 'new';
  }
}
