import { Component, Input, OnInit } from '@angular/core';
import { faEdit, faTrashAlt } from '@fortawesome/free-regular-svg-icons';
import { faArrowLeft, faPlus } from '@fortawesome/free-solid-svg-icons';
import { BuyerAddress, ListBuyerAddress } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'marketplace';
import { ToastrService } from 'ngx-toastr';
import { ModalState } from '../../../models/modal-state.class';
import { getSuggestedAddresses } from '../../../services/address-suggestion.helper';

@Component({
  templateUrl: './address-list.component.html',
  styleUrls: ['./address-list.component.scss'],
})
export class OCMAddressList implements OnInit {
  @Input() addresses: ListBuyerAddress;
  faPlus = faPlus;
  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;
  faEdit = faEdit;
  currentAddress: BuyerAddress;
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  };
  resultsPerPage = 8;
  areYouSureModal = ModalState.Closed;
  showCreateAddressForm = false;
  isLoading = false;
  suggestedAddresses: ListBuyerAddress;
  constructor(private context: ShopperContextService, private toasterService: ToastrService) {}

  ngOnInit(): void {
    this.reloadAddresses();
  }

  reset(): void {
    this.currentAddress = {};
  }

  showAddAddress(): void {
    this.currentAddress = null;
    this.showCreateAddressForm = true;
  }

  showEditAddress(address: BuyerAddress): void {
    this.currentAddress = address;
    this.showCreateAddressForm = true;
  }

  showAreYouSure(address: BuyerAddress): void {
    this.currentAddress = address;
    this.areYouSureModal = ModalState.Open;
  }

  closeAreYouSure(): void {
    this.currentAddress = null;
    this.areYouSureModal = ModalState.Closed;
  }

  dismissEditAddressForm(): void {
    this.currentAddress = null;
    this.showCreateAddressForm = false;
    this.suggestedAddresses = null;
  }

  addressFormSubmitted(address: BuyerAddress): void {
    window.scrollTo(0, null);
    if (this.currentAddress) {
      this.updateAddress(address);
    } else {
      this.addAddress(address);
    }
  }

  async deleteAddress(address: BuyerAddress): Promise<void> {
    this.areYouSureModal = ModalState.Closed;
    await this.context.currentUser.addresses.delete(address.ID);
    this.addresses.Items = this.addresses.Items.filter(a => a.ID !== address.ID);
  }

  updateRequestOptions(newOptions: { page?: number; search?: string }): void {
    this.requestOptions = Object.assign(this.requestOptions, newOptions);
    this.reloadAddresses();
  }

  protected refresh(): void {
    this.currentAddress = null;
    this.reloadAddresses();
  }

  private async addAddress(address: BuyerAddress): Promise<void> {
    try {
      address.Shipping = true;
      address.Billing = true;
      const newAddress = await this.context.currentUser.addresses.create(address);
      this.addresses.Items = [...this.addresses.Items, newAddress];
      this.showCreateAddressForm = false;
      this.suggestedAddresses = null;
      this.refresh();
    } catch (ex) {
      this.suggestedAddresses = getSuggestedAddresses(ex);
      this.toasterService.error('Invalid Address');
    }
  }

  private async updateAddress(address: BuyerAddress): Promise<any> {
    try {
      address.Shipping = true;
      address.Billing = true;
      address.ID = this.currentAddress.ID;
      await this.context.currentUser.addresses.edit(address.ID, address);
      this.showCreateAddressForm = false;
      this.suggestedAddresses = null;
      this.refresh();
    } catch (ex) {
      this.suggestedAddresses = getSuggestedAddresses(ex);
      this.toasterService.error('Invalid Address');
    }
  }

  private async reloadAddresses(): Promise<void> {
    this.isLoading = true;
    this.addresses = await this.context.currentUser.addresses.list(this.requestOptions);
    this.isLoading = false;
  }
}
