import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { OcAddressService, BuyerAddress } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';

// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerLocationService extends ResourceCrudService<BuyerAddress> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocAddressService: OcAddressService) {
    super(router, activatedRoute, ocAddressService, '/buyers', 'buyers', 'locations');
  }
}
