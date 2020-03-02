import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { User, OcSupplierUserService } from '@ordercloud/angular-sdk';
import { SUPPLIER_SUB_RESOURCE_LIST } from '../suppliers/supplier.service';

// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class SupplierUserService extends ResourceCrudService<User> {
  emptyResource = {
    Username: '',
    FirstName: '',
    LastName: '',
    Email: '',
    Phone: '',
  };

  constructor(router: Router, activatedRoute: ActivatedRoute, ocSupplierUserService: OcSupplierUserService) {
    super(
      router,
      activatedRoute,
      ocSupplierUserService,
      '/suppliers',
      'suppliers',
      SUPPLIER_SUB_RESOURCE_LIST,
      'users'
    );
  }
}
