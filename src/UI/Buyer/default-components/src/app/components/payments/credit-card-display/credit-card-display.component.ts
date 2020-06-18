import { Component, Input } from '@angular/core';
import { BuyerCreditCard, Address } from 'ordercloud-javascript-sdk';
import { MarketplaceBuyerCreditCard } from 'marketplace';

@Component({
  templateUrl: './credit-card-display.component.html',
  styleUrls: ['./credit-card-display.component.scss'],
})
export class OCMCreditCardDisplay {
  @Input() set card(value: MarketplaceBuyerCreditCard) {
    this.creditCard = value;
    this.address = value?.xp?.CCBillingAddress;
  }
  @Input() highlight?: boolean;
  creditCard: BuyerCreditCard;
  address: Address;
}
