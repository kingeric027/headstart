import { Component, Input, OnInit } from '@angular/core';
import { BuyerAddress, ListBuyerAddress } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.scss'],
})
export class OCMLocationList implements OnInit {
  @Input() addresses: ListBuyerAddress;
  currentAddress: BuyerAddress;
  requestOptions: {
    page?: number;
    search?: string;
    filters?: {
      [key: string]: string | string[];
    };
  } = {
    page: undefined,
    search: undefined,
    filters: { ['Editable']: 'false' },
  };
  resultsPerPage = 8;
  isLoading = false;
  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.reloadAddresses();
  }

  reset(): void {
    this.currentAddress = {};
  }

  updateRequestOptions(newOptions: { page?: number; search?: string }): void {
    this.requestOptions = Object.assign(this.requestOptions, newOptions);
    this.reloadAddresses();
  }

  protected refresh(): void {
    this.currentAddress = null;
    this.reloadAddresses();
  }

  private async reloadAddresses(): Promise<void> {
    this.isLoading = true;
    this.addresses = await this.context.currentUser.addresses.list(this.requestOptions);
    this.isLoading = false;
  }
}
