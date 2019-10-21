import { Component, Input } from '@angular/core';
import { BuyerCreditCard } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './credit-card-display.component.html',
  styleUrls: ['./credit-card-display.component.scss'],
})
export class OCMCreditCardDisplay {
  @Input() card: BuyerCreditCard;
}
