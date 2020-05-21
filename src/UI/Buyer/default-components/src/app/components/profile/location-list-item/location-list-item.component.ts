import { Component, Input, OnInit } from '@angular/core';
import { Address,  } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'marketplace';

import {
  MarketplaceAddressBuyer,
} from '../../../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';


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

  toLocationOrders(): void {
    this.context.router.toOrdersByLocation({location: this.location.ID});
  }
}
