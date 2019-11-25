import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { OcSupplierAddressService, Address } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';

@Injectable({
  providedIn: 'root',
})
export class SupplierAddressService extends ResourceCrudService<Address> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ccSupplierAddressService: OcSupplierAddressService) {
    super(router, activatedRoute, ccSupplierAddressService, '/suppliers', 'suppliers', 'locations');
  }
}
