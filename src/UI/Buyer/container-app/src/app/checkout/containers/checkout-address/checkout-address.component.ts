import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ListBuyerAddress, Order, BuyerAddress, ListLineItem, Address } from '@ordercloud/angular-sdk';
import { ModalState } from 'src/app/shared/models/modal-state.class';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'checkout-address',
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss'],
})
export class CheckoutAddressComponent implements OnInit {
  @Input() addressType: 'Shipping' | 'Billing';
  @Output() continue = new EventEmitter();
  isAnon: boolean;
  addressModal = ModalState.Closed;
  existingAddresses: ListBuyerAddress;
  selectedAddress: BuyerAddress;
  order: Order;
  lineItems: ListLineItem;
  resultsPerPage = 8;
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  };
  usingShippingAsBilling = false;

  constructor(private context: ShopperContextService) {}

  ngOnInit() {
    this.isAnon = this.context.currentUser.isAnonymous;
    if (!this.isAnon) {
      this.getSavedAddresses();
    }

    this.setSelectedAddress();
  }

  clearRequestOptions() {
    this.updateRequestOptions({ page: undefined, search: undefined });
  }

  openAddressModal() {
    this.addressModal = ModalState.Open;
  }

  updateRequestOptions(options: { page?: number; search?: string }) {
    this.requestOptions = options;
    this.getSavedAddresses();
  }

  private async getSavedAddresses() {
    const filters = {};
    filters[this.addressType] = true;
    const options = { filters, ...this.requestOptions, pageSize: this.resultsPerPage };
    this.existingAddresses = await this.context.myResources.ListAddresses(options).toPromise();
  }

  private setSelectedAddress() {
    this.order = this.context.currentOrder.get();
    this.lineItems = this.context.currentOrder.lineItems;

    this.selectedAddress = this.addressType === 'Billing' ? this.order.BillingAddress : this.lineItems.Items[0].ShippingAddress; // shipping address is defined at the line item level
  }

  existingAddressSelected(address: BuyerAddress) {
    this.selectedAddress = address;
    this.addressModal = ModalState.Open;
  }

  useShippingAsBilling() {
    if (this.addressType === 'Shipping') {
      return;
    }

    this.usingShippingAsBilling = true;
    this.selectedAddress = this.lineItems.Items[0].ShippingAddress;
  }

  async saveAddress(address: Address, formDirty: boolean) {
    // TODO: make bellow line better
    const setOneTimeAddress =
      this.isAnon || formDirty || (this.usingShippingAsBilling && !this.order.ShippingAddressID) || !address.ID || address.ID === '';
    if (setOneTimeAddress) {
      this.order = await this.setOneTimeAddress(address);
    } else {
      this.order = await this.setSavedAddress(address.ID);
    }
    if (this.addressType === 'Shipping') {
      this.lineItems.Items[0].ShippingAddress = address;
      this.context.currentOrder.lineItems = this.lineItems;
    }
    this.continue.emit();
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
