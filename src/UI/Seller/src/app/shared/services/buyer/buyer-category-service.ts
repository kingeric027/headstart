import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Category, OcCategoryService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from '@app-seller/buyers/components/buyers/buyer.service';

@Injectable({
  providedIn: 'root',
})
export class BuyerCategoryService extends ResourceCrudService<Category> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocCategoryService: OcCategoryService) {
    super(router, activatedRoute, ocCategoryService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'categories');
  }
}
