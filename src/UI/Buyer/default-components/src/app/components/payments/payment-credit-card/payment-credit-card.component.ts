import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ListBuyerCreditCard, BuyerCreditCard } from 'marketplace';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  templateUrl: './payment-credit-card.component.html',
  styleUrls: ['./payment-credit-card.component.scss']
})
export class OCMPaymentCreditCard implements OnInit {
  @Input() cards: ListBuyerCreditCard; 
  @Output() cardSelected = new EventEmitter<{card: BuyerCreditCard, cvv: string}>();
  form = new FormGroup({ 
    cardID: new FormControl(null, Validators.required), 
    cvv: new FormControl('', Validators.required) 
  });

  constructor() {}

  ngOnInit() {}

  submit() {
    const cardID = this.form.value.cardID;
    const card = this.cards.Items.find(c => c.ID === cardID);
    this.cardSelected.emit({ card, cvv: this.form.value.cvv });
  }
}
