import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { Category, OcCategoryService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from './buyer.service';

@Injectable({
  providedIn: 'root',
})
export class BuyerCategoryService extends ResourceCrudService<Category> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocCategoryService: OcCategoryService) {
    super(router, activatedRoute, ocCategoryService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'categories');
  }
}
