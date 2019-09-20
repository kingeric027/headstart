import { Component, Input, OnChanges } from '@angular/core';
import { Address } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../../shopper-context';

@Component({
  templateUrl: './address-card.component.html',
  styleUrls: ['./address-card.component.scss'],
})
export class OCMAddressCard extends OCMComponent implements OnChanges {
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
