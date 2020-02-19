import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcSupplierAddressService, Address } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { SUPPLIER_SUB_RESOURCE_LIST } from '../suppliers/supplier.service';

@Injectable({
  providedIn: 'root',
})
export class SupplierAddressService extends ResourceCrudService<Address> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ccSupplierAddressService: OcSupplierAddressService) {
    super(
      router,
      activatedRoute,
      ccSupplierAddressService,
      '/suppliers',
      'suppliers',
      SUPPLIER_SUB_RESOURCE_LIST,
      'locations'
    );
  }

  emptyResource = {
    CompanyName: '',
    FirstName: '',
    LastName: '',
    Street1: '',
    Street2: '',
    City: '',
    State: '',
    Zip: '',
    Country: '',
    Phone: '',
    AddressName: 'Your new supplier location',
    xp: null,
  };
}
