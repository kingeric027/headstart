import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Promotion, OcPromotionService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import {
  MarketplacePromoType,
  MarketplacePromoEligibility,
} from '@app-seller/shared/models/marketplace-promo.interface';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { Promotions } from 'ordercloud-javascript-sdk';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class PromotionService extends ResourceCrudService<Promotion> {
  emptyResource = {
    ID: null,
    Name: '',
    Description: '',
    LineItemLevel: false,
    Code: '',
    RedemptionLimit: null,
    RedemptionLimitPerUser: null,
    FinePrint: '',
    StartDate: '',
    ExpirationDate: '',
    EligibleExpression: 'true',
    ValueExpression: '',
    CanCombine: false,
    AllowAllBuyers: true,
    xp: {
      Type: MarketplacePromoType.Percentage,
      Value: null,
      AppliesTo: MarketplacePromoEligibility.EntireOrder,
      ScopeToSupplier: false,
      Supplier: null,
      Automatic: false,
      MinReq: {
        Type: null,
        Int: null,
      },
      MaxShipCost: null,
    },
  };

  constructor(router: Router, activatedRoute: ActivatedRoute, currentUserService: CurrentUserService) {
    super(router, activatedRoute, Promotions, currentUserService, '/promotions', 'promotions');
  }
}
