import { Component, Input, OnInit } from '@angular/core';
import { ShopperContextService, BuyerLocationWithCert } from 'marketplace';
import { MarketplaceAddressBuyer } from '../../../../../../marketplace/node_modules/marketplace-javascript-sdk/dist';

@Component({
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.scss'],
})
export class OCMLocationList implements OnInit {
  locations: MarketplaceAddressBuyer[];
  currentLocation: MarketplaceAddressBuyer;
  requestOptions: { page?: number; search?: string } = { page: undefined, search: undefined };
  resultsPerPage = 8;
  isLoading = false;
  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.reloadAddresses();
  }

  reset(): void {
    this.currentLocation = {};
  }

  updateRequestOptions(newOptions: { page?: number; search?: string }): void {
    this.requestOptions = Object.assign(this.requestOptions, newOptions);
    this.reloadAddresses();
  }

  protected refresh(): void {
    this.currentLocation = null;
    this.reloadAddresses();
  }

  private async reloadAddresses(): Promise<void> {
    this.isLoading = true;
    const locationsList = await this.context.addresses.listBuyerLocations(this.requestOptions);
    this.locations = locationsList.Items;
    this.isLoading = false;
  }
}
