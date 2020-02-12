import { Component, Input, Output, EventEmitter, OnChanges, OnInit } from '@angular/core';
import { ListBuyerAddress, Order, BuyerAddress, ListLineItem, Address } from '@ordercloud/angular-sdk';
import { ShopperContextService, MarketplaceOrder } from 'marketplace';

// TODO - Make this component "Dumb" by removing the dependence on context service 
// and instead have it use inputs and outputs to interact with the CheckoutComponent.
// Goal is to get all the checkout logic and state into one component. 

@Component({
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss'],
})
export class OCMCheckoutAddress implements OnInit {
  existingAddresses: ListBuyerAddress;
  selectedAddress: BuyerAddress;
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  };
  usingShippingAsBilling = false;
  showAddAddressForm = false;
  
  @Input() addressType: 'Shipping' | 'Billing';
  @Input() isAnon: boolean;
  @Input() order: MarketplaceOrder;
  @Input() lineItems: ListLineItem;
  @Output() continue = new EventEmitter();

  constructor(private context: ShopperContextService) {}

  async ngOnInit() {
    // TODO - we need to investigate this pattern and see if its something we can rely on.
    // These are not promises. They are just inputs that are not set when ngOnInit fires.
    await this.order, await this.lineItems;
    if (!this.isAnon) {
      this.getSavedAddresses();
    }
    // shipping address is defined at the line item level
    this.selectedAddress =
      this.addressType === 'Billing' ? this.order.BillingAddress : this.lineItems.Items[0].ShippingAddress;
  }

  clearRequestOptions() {
    this.updateRequestOptions({ page: undefined, search: undefined });
  }

  toggleShowAddressForm(event) {
    this.showAddAddressForm = event.target.value === 'new';
    const selectedAddress = this.existingAddresses.Items.find(address => event.target.value === address.ID);
    this.existingAddressSelected(selectedAddress);
  }

  updateRequestOptions(options: { page?: number; search?: string }) {
    this.requestOptions = options;
    this.getSavedAddresses();
  }

  private async getSavedAddresses() {
    const filters = {};
    filters[this.addressType] = true;
    const options = { filters, ...this.requestOptions };
    this.existingAddresses = await this.context.currentUser.addresses.list(options);
  }

  existingAddressSelected(address: BuyerAddress) {
    this.selectedAddress = address;
  }

  useShippingAsBilling() {
    if (this.addressType === 'Shipping') {
      return;
    }

    this.usingShippingAsBilling = true;
    this.selectedAddress = this.lineItems.Items[0].ShippingAddress;
    this.saveAddress(this.selectedAddress, false, false);
  }

  async saveAddress(address: Address, formDirty: boolean, shouldSaveAddress: boolean) {
    // TODO: make bellow line better
    const setOneTimeAddress =
      this.isAnon ||
      formDirty ||
      (this.usingShippingAsBilling && !this.order.ShippingAddressID) ||
      !address.ID ||
      address.ID === '';
    if (shouldSaveAddress) {
      this.order = await this.saveAndSetAddress(address);
    } else {
      if (setOneTimeAddress) {
        this.order = await this.setOneTimeAddress(address);
      } else {
        this.order = await this.setSavedAddress(address.ID);
      }
    }
    if (this.addressType === 'Shipping') {
      this.lineItems.Items[0].ShippingAddress = address;
      // TODO - handle this.
      // this.context.currentOrder.lineItems = this.lineItems;
    }
    this.continue.emit();
  }

  private async saveAndSetAddress(address: BuyerAddress): Promise<Order> {
    const addressToSave = this.addShippingAndBillingOptionsOnAddress(address);
    const savedAddress = await this.context.currentUser.addresses.create(addressToSave);
    return await this.setSavedAddress(savedAddress.ID);
  }

  private addShippingAndBillingOptionsOnAddress(address: BuyerAddress): BuyerAddress {
    /* right now there is no distinction between billing and shipping addresses in the UI
     this assummes these values will be set to true in all me addresses created at checkout*/
    address.Shipping = true;
    address.Billing = true;
    return address;
  }

  private async setOneTimeAddress(address: BuyerAddress): Promise<Order> {
    // If a saved address (with an ID) is changed by the user it is attached to an order as a one time address.
    // However, order.ShippingAddressID (or BillingAddressID) still points to the unmodified address. The ID should be cleared.
    address.ID = null;
    if (this.addressType === 'Shipping') {
      return await this.context.currentOrder.setShippingAddress(address);
    } else if (this.addressType === 'Billing') {
      return await this.context.currentOrder.setBillingAddress(address);
    }
  }

  private async setSavedAddress(addressID: string): Promise<Order> {
    if (this.addressType === 'Shipping') {
      return await this.context.currentOrder.setShippingAddressByID(addressID);
    } else if (this.addressType === 'Billing') {
      return await this.context.currentOrder.setBillingAddressByID(addressID);
    }
  }
}
