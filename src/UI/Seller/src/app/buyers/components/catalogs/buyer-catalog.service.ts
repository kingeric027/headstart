import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Buyer, OcUserGroupService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from '../buyers/buyer.service';

@Injectable({
  providedIn: 'root',
})
export class BuyerCatalogService extends ResourceCrudService<Buyer> {
  emptyResource = {
    Name: '',
    xp: {
      Type: 'Catalog',
    },
  };

  constructor(router: Router, activatedRoute: ActivatedRoute, ocUserGroupService: OcUserGroupService) {
    super(router, activatedRoute, ocUserGroupService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'catalogs');
  }
}
