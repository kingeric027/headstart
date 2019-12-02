import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { faPlus, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { ListBuyerAddress, BuyerAddress } from '@ordercloud/angular-sdk';
import { faTrashAlt, faEdit } from '@fortawesome/free-regular-svg-icons';
import { OCMComponent } from '../../base-component';
import { ModalState } from '../../../models/modal-state.class';

@Component({
  templateUrl: './address-list.component.html',
  styleUrls: ['./address-list.component.scss'],
})
export class OCMAddressList extends OCMComponent implements OnInit, OnChanges {
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

  ngOnInit() {}

  ngOnChanges() {}

  ngOnContextSet() {
    this.reloadAddresses();
  }

  reset() {
    this.currentAddress = {};
  }

  showAddAddress() {
    console.log('show');
    this.currentAddress = null;
    this.showCreateAddressForm = true;
  }

  showEditAddress(address: BuyerAddress) {
    this.currentAddress = address;
    this.showCreateAddressForm = true;
  }

  showAreYouSure(address: BuyerAddress) {
    this.currentAddress = address;
    this.areYouSureModal = ModalState.Open;
  }

  closeAreYouSure() {
    this.currentAddress = null;
    this.areYouSureModal = ModalState.Closed;
  }

  dismissEditAddressForm() {
    this.currentAddress = null;
    this.showCreateAddressForm = false;
  }

  protected refresh() {
    this.currentAddress = null;
    this.reloadAddresses();
  }

  addressFormSubmitted(address: BuyerAddress) {
    this.showCreateAddressForm = false;
    window.scrollTo(0, null);
    if (this.currentAddress) {
      this.updateAddress(address);
    } else {
      this.addAddress(address);
    }
  }

  private async addAddress(address: BuyerAddress) {
    address.Shipping = true;
    address.Billing = true;
    const newAddress = await this.context.myResources.CreateAddress(address).toPromise();
    this.addresses.Items = [...this.addresses.Items, newAddress];
  }

  private async updateAddress(address: BuyerAddress): Promise<any> {
    address.ID = this.currentAddress.ID;
    await this.context.myResources.PatchAddress(address.ID, address).toPromise();
    this.refresh();
  }

  async deleteAddress(address: BuyerAddress) {
    this.areYouSureModal = ModalState.Closed;
    await this.context.myResources.DeleteAddress(address.ID).toPromise();
    this.addresses.Items = this.addresses.Items.filter(a => a.ID !== address.ID);
  }

  updateRequestOptions(newOptions: { page?: number; search?: string }) {
    this.requestOptions = Object.assign(this.requestOptions, newOptions);
    this.reloadAddresses();
  }

  private reloadAddresses() {
    this.context.myResources.ListAddresses(this.requestOptions).subscribe(res => {
      this.addresses = res;
    });
  }
}
