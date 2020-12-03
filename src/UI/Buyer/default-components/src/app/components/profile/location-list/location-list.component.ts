import { Component, OnInit } from '@angular/core'
import { ShopperContextService } from 'marketplace'
import { ListPage, MarketplaceAddressBuyer } from '@ordercloud/headstart-sdk'

@Component({
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.scss'],
})
export class OCMLocationList implements OnInit {
  locations: ListPage<MarketplaceAddressBuyer>
  currentLocation: MarketplaceAddressBuyer
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  }
  resultsPerPage = 8
  isLoading = false
  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.reloadAddresses()
  }

  reset(): void {
    this.currentLocation = {}
  }

  updateRequestOptions(newOptions: { page?: number; search?: string }): void {
    this.requestOptions = Object.assign(this.requestOptions, newOptions)
    this.reloadAddresses()
  }

  protected refresh(): void {
    this.currentLocation = null
    this.reloadAddresses()
  }

  private async reloadAddresses(): Promise<void> {
    this.isLoading = true
    this.locations = await this.context.addresses.listBuyerLocations(
      this.requestOptions
    )
    this.isLoading = false
  }
}
