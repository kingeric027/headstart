import { Component, OnInit } from '@angular/core';
import { faPlus, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { OcMeService, ListBuyerAddress, BuyerAddress } from '@ordercloud/angular-sdk';
import { faTrashAlt, faEdit } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'profile-address-list',
  templateUrl: './address-list.component.html',
  styleUrls: ['./address-list.component.scss'],
})
export class AddressListComponent implements OnInit {
  faPlus = faPlus;
  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;
  faEdit = faEdit;
  addresses: ListBuyerAddress;
  currentAddress: BuyerAddress;
  requestOptions: { page?: number; search?: string } = {
    page: undefined,
    search: undefined,
  };
  resultsPerPage = 8;
  areYouSureModalOpen = false;
  addAddressModalOpen = false;

  constructor(private ocMeService: OcMeService) {}

  ngOnInit() {
    this.reloadAddresses();
  }

  reset() {
    this.currentAddress = {};
  }

  showAddAddress() {
    this.currentAddress = null;
    this.addAddressModalOpen = true;
  }

  showEditAddress(address: BuyerAddress) {
    this.currentAddress = address;
    this.addAddressModalOpen = true;
  }

  showAreYouSure(address: BuyerAddress) {
    this.currentAddress = address;
    this.areYouSureModalOpen = true;
  }

  closeAreYouSure() {
    this.currentAddress = null;
    this.areYouSureModalOpen = false;
  }

  protected refresh() {
    this.currentAddress = null;
    this.reloadAddresses();
  }

  addressFormSubmitted(address: BuyerAddress) {
    this.addAddressModalOpen = false;
    if (this.currentAddress) {
      this.updateAddress(address);
    } else {
      this.addAddress(address);
    }
  }

  private addAddress(address: BuyerAddress) {
    address.Shipping = true;
    address.Billing = true;
    this.ocMeService.CreateAddress(address).subscribe(
      () => {
        this.refresh();
      },
      (error) => {
        throw error;
      }
    );
  }

  private updateAddress(address: BuyerAddress) {
    address.ID = this.currentAddress.ID;
    this.ocMeService.PatchAddress(address.ID, address).subscribe(
      () => {
        this.refresh();
      },
      (error) => {
        throw error;
      }
    );
  }

  deleteAddress(address: BuyerAddress) {
    this.ocMeService.DeleteAddress(address.ID).subscribe(
      () => {
        this.closeAreYouSure();
        this.reloadAddresses();
      },
      (error) => {
        throw error;
      }
    );
  }

  updateRequestOptions(newOptions: { page?: number; search?: string }) {
    this.requestOptions = Object.assign(this.requestOptions, newOptions);
    this.reloadAddresses();
  }

  private reloadAddresses() {
    this.ocMeService.ListAddresses({ ...this.requestOptions, pageSize: this.resultsPerPage }).subscribe((res) => (this.addresses = res));
  }
}
