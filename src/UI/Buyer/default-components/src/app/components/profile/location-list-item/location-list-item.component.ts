import { Component, Input } from '@angular/core';
import { Address,  } from 'ordercloud-javascript-sdk';
import { ShopperContextService } from 'marketplace';

import {
  MarketplaceAddressBuyer,
} from '@ordercloud/headstart-sdk';


@Component({
  templateUrl: './location-list-item.component.html',
  styleUrls: ['./location-list-item.component.scss'],
})
export class OCMLocationListItem {
  @Input() location: MarketplaceAddressBuyer;
  @Input() canViewOrders: boolean;

  constructor(private context: ShopperContextService) {}

  // make into pipe?
  getFullName(address: Address): string {
    const fullName = `${address?.FirstName || ''} ${address?.LastName || ''}`;
    return fullName.trim();
  }

  toLocationManagement(): void {
    this.context.router.toLocationManagement(this.location.ID);
  }
}
