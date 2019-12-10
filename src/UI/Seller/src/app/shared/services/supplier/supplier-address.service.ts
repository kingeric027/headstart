import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { OcSupplierAddressService, Address } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { SUPPLIER_SUB_RESOURCE_LIST } from './supplier.service';

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
