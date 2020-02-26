import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { BuyerAddress, ListAddress, Address } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ValidateUSZip, ValidatePhone } from '@app-seller/validators/validators';
import { BuyerLocationService } from '../buyer-location.service';
import { BuyerService } from '../../buyers/buyer.service';

function createBuyerLocationForm(supplierLocation: BuyerAddress) {
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
  selector: 'app-buyer-location-table',
  templateUrl: './buyer-location-table.component.html',
  styleUrls: ['./buyer-location-table.component.scss'],
})
export class BuyerLocationTableComponent extends ResourceCrudComponent<BuyerAddress> {

  suggestedAddresses: ListAddress;
  selectedAddress: Address;

  constructor(
    private buyerLocationService: BuyerLocationService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private buyerService: BuyerService,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, buyerLocationService, router, activatedroute, ngZone, createBuyerLocationForm);
  }

  handleAddressSelect(address) {
    this.updatedResource = address;
  }

  async updateExitingResource(): Promise<void> {
    try {
      this.dataIsSaving = true;
      const updatedResource = await this.ocService.updateResource(this.updatedResource);
      this.resourceInSelection = this.copyResource(updatedResource);
      this.setUpdatedResourceAndResourceForm(updatedResource);
      this.suggestedAddresses = null;
      this.dataIsSaving = false;
    } catch (ex) {
      this.suggestedAddresses = this.ocService.getSuggestedAddresses(ex, this.updatedResource);
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
      this.suggestedAddresses = this.ocService.getSuggestedAddresses(ex, this.updatedResource);
      this.dataIsSaving = false;
      throw ex;
    }
  }
}
