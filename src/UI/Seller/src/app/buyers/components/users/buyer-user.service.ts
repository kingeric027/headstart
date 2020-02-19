import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { User, OcUserService } from '@ordercloud/angular-sdk';
import { BUYER_SUB_RESOURCE_LIST } from '../buyers/buyer.service';

// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerUserService extends ResourceCrudService<User> {
  emptyResource = {
    Username: '',
    FirstName: '',
    LastName: '',
    Email: '',
    Phone: '',
  };

  constructor(router: Router, activatedRoute: ActivatedRoute, ocUserService: OcUserService) {
    super(router, activatedRoute, ocUserService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'users');
  }
}
