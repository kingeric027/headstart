import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BuyerCreditCard, ListPage } from 'ordercloud-javascript-sdk';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { CreditCardFormOutput } from '../credit-card-form/credit-card-form.component';
import { SelectedCreditCard } from '../../checkout/checkout-payment/checkout-payment.component';

@Component({
  templateUrl: './payment-credit-card.component.html',
  styleUrls: ['./payment-credit-card.component.scss'],
})
export class OCMPaymentCreditCard implements OnInit {
  showNewCCForm = false;

  @Input() cards: ListPage<BuyerCreditCard>;
  @Output() cardSelected = new EventEmitter<SelectedCreditCard>();
  form = new FormGroup({
    cardID: new FormControl(null, Validators.required),
    cvv: new FormControl('', Validators.required),
  });

  constructor() {}

  ngOnInit(): void {}

  submit(output: CreditCardFormOutput): void {
    const cardID = this.form.value.cardID;
    const SavedCard = this.cards.Items.find(c => c.ID === cardID);
    this.cardSelected.emit({ SavedCard, CVV: output.cvv, NewCard: output.card });
  }

  toggleShowCCForm(event: any): void {
    this.showNewCCForm = event.target.value === 'new';
  }
}
