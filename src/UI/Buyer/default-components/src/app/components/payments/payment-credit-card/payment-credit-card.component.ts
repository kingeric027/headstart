import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ListBuyerCreditCard, BuyerCreditCard } from 'marketplace';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  templateUrl: './payment-credit-card.component.html',
  styleUrls: ['./payment-credit-card.component.scss']
})
export class OCMPaymentCreditCard implements OnInit {
  @Input() cards: ListBuyerCreditCard; 
  @Output() cardSelected = new EventEmitter<{cardID: string, cvv: string}>();
  form = new FormGroup({ 
    cardID: new FormControl(null, Validators.required), 
    cvv: new FormControl('', Validators.required) 
  });

  constructor() {}

  ngOnInit() {}

  submit() {
    this.cardSelected.emit(this.form.value);
  }
}
