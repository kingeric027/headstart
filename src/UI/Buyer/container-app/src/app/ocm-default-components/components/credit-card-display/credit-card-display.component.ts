import { Component, Input } from '@angular/core';
import { BuyerCreditCard } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../../shopper-context';

@Component({
  templateUrl: './credit-card-display.component.html',
  styleUrls: ['./credit-card-display.component.scss'],
})
export class OCMCreditCardDisplay extends OCMComponent {
  @Input() card: BuyerCreditCard;
}
