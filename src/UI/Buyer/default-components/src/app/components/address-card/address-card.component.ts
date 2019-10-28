import { Component, Input, OnChanges, ViewEncapsulation } from '@angular/core';
import { Address } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './address-card.component.html',
  styleUrls: ['./address-card.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class OCMAddressCard implements OnChanges {
  @Input() address: Address = {};
  @Input() addressTitle?: string;

  ngOnChanges() {
    this.address['FullName'] = this.getFullName(this.address);
  }

  // make into pipe?
  getFullName(address: Address) {
    const fullName = `${address.FirstName || ''} ${address.LastName || ''}`;
    return fullName.trim();
  }
}
