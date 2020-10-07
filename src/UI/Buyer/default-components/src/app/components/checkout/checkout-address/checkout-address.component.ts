import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Address, BuyerAddress, LineItem, ListPage } from 'ordercloud-javascript-sdk';
import { ShopperContextService } from 'marketplace';
import { MarketplaceOrder, MarketplaceAddressBuyer } from '@ordercloud/headstart-sdk';

import { getSuggestedAddresses } from '../../../services/address-suggestion.helper';
// TODO - Make this component "Dumb" by removing the dependence on context service 
// and instead have it use inputs and outputs to interact with the CheckoutComponent.
// Goal is to get all the checkout logic and state into one component. 

@Component({
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss'],
})
export class OCMCheckoutAddress implements OnInit {
  readonly NEW_ADDRESS_CODE = 'new';
  existingBuyerLocations: ListPage<BuyerAddress>;
  selectedBuyerLocation: BuyerAddress;
  existingShippingAddresses: ListPage<BuyerAddress>;
  selectedShippingAddress: BuyerAddress;
  showNewAddressForm = false;
  suggestedAddresses: BuyerAddress[];
  homeCountry: string;
  
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListPage<LineItem>;
  @Output() continue = new EventEmitter();

  constructor(private context: ShopperContextService) { }

  async ngOnInit(): Promise<void> {
    this.selectedShippingAddress = this.lineItems?.Items[0].ShippingAddress;
    await this.listSavedShippingAddresses();
    await this.listSavedBuyerLocations();
  }

  onBuyerLocationChange(buyerLocationID: string): void {
    this.selectedBuyerLocation = this.existingBuyerLocations.Items.find(location => buyerLocationID === location.ID);
    const shippingAddress = this.existingShippingAddresses.Items.find(location => location.ID === this.selectedBuyerLocation.ID);
    if (shippingAddress) {
      this.selectedShippingAddress = shippingAddress;
    }
  }

  onShippingAddressChange(shippingAddressID: string): void {
    this.showNewAddressForm = shippingAddressID === this.NEW_ADDRESS_CODE;
    this.selectedShippingAddress = this.existingShippingAddresses.Items.find(address => shippingAddressID === address.ID);
    const shippingAddress = this.existingShippingAddresses.Items.find(address => address.ID === this.selectedShippingAddress?.ID);
    if (shippingAddress) {
      this.selectedShippingAddress = shippingAddress;
    }
  }

  async saveAddressesAndContinue(newShippingAddress: Address = null): Promise<void> {
    this.order = await this.context.order.checkout.setBuyerLocationByID(this.selectedBuyerLocation?.ID);
    if (newShippingAddress != null) {
      this.selectedShippingAddress = await this.saveNewShippingAddress(newShippingAddress);
    }
    await this.context.order.checkout.setShippingAddressByID(this.selectedShippingAddress);
    this.continue.emit();
  }

  private async listSavedBuyerLocations(): Promise<void> {
    this.existingBuyerLocations = await this.context.addresses.listBuyerLocations();
    this.homeCountry = this.existingBuyerLocations?.Items[0]?.Country || 'US';
    if (this.existingBuyerLocations?.Items.length === 1) {
      this.selectedBuyerLocation = this.selectedShippingAddress = this.existingBuyerLocations.Items[0];
    }
  }

  private async listSavedShippingAddresses(): Promise<void> {
    this.existingShippingAddresses = await this.context.addresses.list({ filters: { Shipping: true }});
  }

  private async saveNewShippingAddress(address: BuyerAddress): Promise<MarketplaceAddressBuyer> {
    address.Shipping = true;
    address.Billing = false;
    try {
      const savedAddress = await this.context.addresses.create(address);
      return savedAddress;
    } catch (ex) {
      this.suggestedAddresses = getSuggestedAddresses(ex);
    }
  }
}
