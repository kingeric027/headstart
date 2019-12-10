import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Address } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';
import { SupplierAddressService } from '@app-seller/shared/services/supplier/supplier-address.service';
import { Validators, FormControl, FormGroup } from '@angular/forms';
import { ValidatePhone, ValidateUSZip } from '@app-seller/validators/validators';

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
}
