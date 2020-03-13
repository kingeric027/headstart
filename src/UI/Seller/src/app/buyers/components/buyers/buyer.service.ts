import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcBuyerService, Buyer } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';

export const BUYER_SUB_RESOURCE_LIST = ['users', 'locations', 'payments', 'approvals', 'categories'];

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerService extends ResourceCrudService<Buyer> {
  constructor(router: Router, activatedRoute: ActivatedRoute, ocBuyerService: OcBuyerService) {
    super(router, activatedRoute, ocBuyerService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST);
  }

  async createNewResource(resource: any): Promise<any> {
    const newBuyer = await MarketplaceSDK.Buyers.Create(resource);
    this.resourceSubject.value.Items = [...this.resourceSubject.value.Items, newBuyer];
    this.resourceSubject.next(this.resourceSubject.value);
    return newBuyer;
  }
}
