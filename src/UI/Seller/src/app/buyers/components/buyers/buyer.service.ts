import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcBuyerService, Buyer } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { HeadStartSDK } from '@ordercloud/headstart-sdk';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { SuperMarketplaceBuyer, BuyerTempService } from '@app-seller/shared/services/middleware-api/buyer-temp.service';

export const BUYER_SUB_RESOURCE_LIST = [
  { route: 'users', display: 'ADMIN.NAV.USERS' },
  { route: 'locations', display: 'ALIAS.BUYER_LOCATIONS' },
  { route: 'payments', display: 'ADMIN.NAV.PAYMENTS' },
  { route: 'approvals', display: 'ADMIN.NAV.APPROVALS' },
  { route: 'catalogs', display: 'ADMIN.NAV.CATALOGS' },
  { route: 'categories', display: 'ADMIN.NAV.CATEGORIES' },
];

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class BuyerService extends ResourceCrudService<Buyer> {
  emptyResource = {
    Buyer: {
      Name: '',
      Active: true,
    },
    Markup: {
      Percent: 0,
    },
  };
  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    ocBuyerService: OcBuyerService,
    currentUserService: CurrentUserService,
    private buyerTempService: BuyerTempService
  ) {
    super(router, activatedRoute, ocBuyerService, currentUserService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST);
  }
}
