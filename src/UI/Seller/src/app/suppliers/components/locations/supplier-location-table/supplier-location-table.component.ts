import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Address, ListPage } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { Validators, FormControl, FormGroup } from '@angular/forms';
import { ValidatePhone, ValidateUSZip } from '@app-seller/validators/validators';
import { SupplierAddressService } from '../supplier-address.service';
import { SupplierService } from '../../suppliers/supplier.service';

function createSupplierLocationForm(supplierLocation: Address) {
  return new FormGroup({
    AddressName: new FormControl(supplierLocation.AddressName, Validators.required),
    CompanyName: new FormControl(supplierLocation.CompanyName, Validators.required),
    Street1: new FormControl(supplierLocation.Street1, Validators.required),
    Street2: new FormControl(supplierLocation.Street2),
    City: new FormControl(supplierLocation.City, Validators.required),
    State: new FormControl(supplierLocation.State, Validators.required),
    Zip: new FormControl(supplierLocation.Zip, [Validators.required, ValidateUSZip]),
    Country: new FormControl(supplierLocation.Country, Validators.required),
    Phone: new FormControl(supplierLocation.Phone, ValidatePhone),
  });
}

@Component({
  selector: 'app-supplier-location-table',
  templateUrl: './supplier-location-table.component.html',
  styleUrls: ['./supplier-location-table.component.scss'],
})
export class SupplierLocationTableComponent extends ResourceCrudComponent<Address> {
  suggestedAddresses: ListPage<Address>;
  selectedAddress: Address;
  canBeDeleted: boolean;

  constructor(
    private supplierAddressService: SupplierAddressService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private supplierService: SupplierService,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, supplierAddressService, router, activatedroute, ngZone, createSupplierLocationForm);
  }

  handleAddressSelect(address) {
    this.updatedResource = address;
  }

  discardChanges(): void {
    this.suggestedAddresses = null;
    this.setUpdatedResourceAndResourceForm(this.resourceInSelection);
  }

  determineIfDeletable(value: boolean): void {
    this.canBeDeleted = value;
  }

  async updateExistingResource(): Promise<void> {
    try {
      this.dataIsSaving = true;
      const updatedResource = await this.ocService.updateResource(this.updatedResource.ID, this.updatedResource);
      this.resourceInSelection = this.ocService.copyResource(updatedResource);
      this.setUpdatedResourceAndResourceForm(updatedResource);
      this.suggestedAddresses = null;
      this.dataIsSaving = false;
    } catch (ex) {
      this.suggestedAddresses = this.ocService.getSuggestedAddresses(ex);
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async createNewResource(): Promise<void> {
    try {
      this.dataIsSaving = true;
      const newResource = await this.ocService.createNewResource(this.updatedResource);
      this.selectResource(newResource);
      this.suggestedAddresses = null;
      this.dataIsSaving = false;
    } catch (ex) {
      this.suggestedAddresses = this.ocService.getSuggestedAddresses(ex);
      this.dataIsSaving = false;
      throw ex;
    }
  }
}
