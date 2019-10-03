import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { OcMeService, ListBuyerAddress, Order, BuyerAddress, ListLineItem, Address } from '@ordercloud/angular-sdk';
import { CurrentOrderService } from 'src/app/shared';
import { ModalState } from 'src/app/shared/models/modal-state.class';

@Component({
  selector: 'checkout-address',
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss'],
})
export class CheckoutAddressComponent implements OnInit {
  @Input() isAnon: boolean;
  @Input() addressType: 'Shipping' | 'Billing';
  @Output() continue = new EventEmitter();
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

  constructor(private ocMeService: OcMeService, private currentOrder: CurrentOrderService) {}

  ngOnInit() {
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

  private getSavedAddresses() {
    const filters = {};
    filters[this.addressType] = true;
    this.ocMeService
      .ListAddresses({
        filters,
        ...this.requestOptions,
        pageSize: this.resultsPerPage,
      })
      .subscribe((addressList) => (this.existingAddresses = addressList));
  }

  private setSelectedAddress() {
    this.order = this.currentOrder.get();
    this.lineItems = this.currentOrder.lineItems;

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
    const setSavedAddress =
      this.isAnon || formDirty || (this.usingShippingAsBilling && !this.order.ShippingAddressID) || (!address.ID || address.ID === '');
    if (setSavedAddress) {
      this.order = await this.setSavedAddress(address.ID);
    } else {
      this.order = await this.setOneTimeAddress(address);
    }
    if (this.addressType === 'Shipping') {
      this.lineItems.Items[0].ShippingAddress = address;
      this.currentOrder.lineItems = this.lineItems;
    }
    this.continue.emit();
  }

  private async setOneTimeAddress(address: BuyerAddress): Promise<Order> {
    // If a saved address (with an ID) is changed by the user it is attached to an order as a one time address.
    // However, order.ShippingAddressID (or BillingAddressID) still points to the unmodified address. The ID should be cleared.
    address.ID = null;
    if (this.addressType === 'Shipping') {
      return await this.currentOrder.setShippingAddress(address);
    } else if (this.addressType === 'Billing') {
      return await this.currentOrder.setBillingAddress(address);
    }
  }

  private async setSavedAddress(addressID: string): Promise<Order> {
    if (this.addressType === 'Shipping') {
      return await this.currentOrder.setShippingAddressByID(addressID);
    } else if (this.addressType === 'Billing') {
      return await this.currentOrder.setBillingAddressByID(addressID);
    }
  }
}
