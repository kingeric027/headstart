import { Component, Input, OnChanges } from '@angular/core';
import { faCcVisa, faCcMastercard, faCcDiscover, IconDefinition } from '@fortawesome/free-brands-svg-icons';
import { OCMComponent } from '../base-component';

@Component({
  templateUrl: './credit-card-icon.component.html',
  styleUrls: ['./credit-card-icon.component.scss'],
})
export class OCMCreditCardIcon extends OCMComponent implements OnChanges {
  cardIcon: IconDefinition;
  @Input() cardType: string;
  @Input() size: string;

  ngOnChanges() {
    if (!this.cardType) return;
    this.cardIcon = this.setCardIcon(this.cardType);
  }

  setCardIcon(cardType: string): IconDefinition {
    switch (cardType.toLowerCase()) {
      case 'visa':
        return faCcVisa;
      case 'mastercard':
        return faCcMastercard;
      case 'discover':
        return faCcDiscover;
    }
  }
}
