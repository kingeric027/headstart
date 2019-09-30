import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';
import { OcMeService, ListBuyerAddress, OcOrderService, Order, BuyerAddress, ListLineItem, Address } from '@ordercloud/angular-sdk';
import { CurrentOrderService } from 'src/app/shared';
import { ToastrService } from 'ngx-toastr';
import { AddressFormComponent } from 'src/app/shared/components/address-form/address-form.component';
import { ModalState } from 'src/app/shared/models/modal-state.class';

@Component({
  selector: 'checkout-address',
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss'],
})
export class CheckoutAddressComponent implements OnInit {
  @Input() isAnon: boolean;
  @Input() addressType: 'Shipping' | 'Billing';
  @ViewChild(AddressFormComponent, { static: false }) addressFormComponent: AddressFormComponent;
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

  constructor(
    private ocMeService: OcMeService,
    private ocOrderService: OcOrderService,
    private currentOrder: CurrentOrderService,
    private toastrService: ToastrService
  ) {}

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
    this.order = this.currentOrder.order;
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

  saveAddress(address: Address, formDirty: boolean) {
    let request = this.setSavedAddress(address);
    if (
      this.isAnon ||
      formDirty ||
      (this.usingShippingAsBilling && !this.order.ShippingAddressID) ||
      (!address.ID || address.ID === '') //If this is not a saved address. Fix for issue 287
    ) {
      request = this.setOneTimeAddress(address);
    }
    request.subscribe(
      (order) => {
        if (this.addressType === 'Shipping') {
          this.lineItems.Items[0].ShippingAddress = address;
          this.currentOrder.lineItems = this.lineItems;
        }
        this.order = order;
        this.currentOrder.order = this.order;
        this.continue.emit();
      },
      (ex) => {
        if (ex.error.Errors[0].ErrorCode === 'NotFound') {
          this.toastrService.error('You no longer have access to this saved address. Please enter or select a different one.');
        }
      }
    );
  }

  private setOneTimeAddress(address: BuyerAddress): Observable<Order> {
    // If a saved address (with an ID) is changed by the user it is attached to an order as a one time address.
    // However, order.ShippingAddressID (or BillingAddressID) still points to the unmodified address. The ID should be cleared.
    address.ID = null;
    return this.ocOrderService[`Set${this.addressType}Address`]('outgoing', this.order.ID, address);
  }

  private setSavedAddress(address): Observable<Order> {
    const addressKey = `${this.addressType}AddressID`;
    const partialOrder = {};
    partialOrder[addressKey] = address.ID;
    return this.ocOrderService.Patch('outgoing', this.order.ID, partialOrder);
  }
}
