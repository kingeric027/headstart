import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { OcBuyerService, Buyer } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';

export const BUYER_SUB_RESOURCE_LIST = ['users', 'locations', 'payments', 'approvals'];

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerService extends ResourceCrudService<Buyer> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocBuyerService: OcBuyerService) {
    super(router, activatedRoute, ocBuyerService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST);
  }
}
