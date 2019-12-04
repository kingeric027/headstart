import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { Supplier, OcSupplierService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class SupplierService extends ResourceCrudService<Supplier> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocSupplierService: OcSupplierService) {
    super(router, activatedRoute, ocSupplierService, '/suppliers', 'suppliers');
  }

  subResourceList = ['users', 'locations'];
}
