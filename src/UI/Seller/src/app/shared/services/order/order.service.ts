import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { Order, OcOrderService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';

@Injectable({
  providedIn: 'root',
})
export class OrderService extends ResourceCrudService<Order> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocOrderService: OcOrderService) {
    super(router, activatedRoute, ocOrderService, '/orders', 'orders');
  }
  setOrderDirection(orderDirection: string){
    this.patchFilterState({OrderDirection: orderDirection})
  }
}
