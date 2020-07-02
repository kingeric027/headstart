import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcBuyerService, Buyer } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { SuperMarketplaceBuyer, BuyerTempService } from '@app-seller/shared/services/middleware-api/buyer-temp.service';

export const BUYER_SUB_RESOURCE_LIST = ['users', 'locations', 'payments', 'approvals', 'catalogs', 'categories'];

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerService extends ResourceCrudService<Buyer> {
  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    ocBuyerService: OcBuyerService,
    currentUserService: CurrentUserService,
    private buyerTempService: BuyerTempService
  ) {
    super(router, activatedRoute, ocBuyerService, currentUserService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST);
  }

  async createNewResource(buyer: SuperMarketplaceBuyer): Promise<any> {
    // Create Buyer with active set to false, checks will need to be performed to ensure that
    // the buyer has everything it needs to be active first
    const newBuyer = await this.buyerTempService.create(buyer);
    this.resourceSubject.value.Items = [...this.resourceSubject.value.Items, newBuyer];
    this.resourceSubject.next(this.resourceSubject.value);
    return newBuyer;
  }

  async updateResource(originalID: string, resource: any): Promise<any> {
    // const args = await this.createListArgs([originalID, resource]);
    // const newResource = await this.ocService.Save(...args).toPromise();
    // const resourceIndex = this.resourceSubject.value.Items.findIndex((i: any) => i.ID === newResource.ID);
    // this.resourceSubject.value.Items[resourceIndex] = newResource;
    // this.resourceSubject.next(this.resourceSubject.value);
    // return newResource;
  }
}
