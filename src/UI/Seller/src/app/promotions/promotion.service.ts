import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Promotion } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import {
  MarketplacePromoType,
  MarketplacePromoEligibility,
  PromotionXp,
  MinRequirementType,
} from '@app-seller/shared/models/marketplace-promo.interface';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { Promotions } from 'ordercloud-javascript-sdk';
import { MarketplaceSupplier } from '@ordercloud/headstart-sdk';

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
    CanCombine: true,
    AllowAllBuyers: true,
    xp: {
      Type: MarketplacePromoType.Percentage,
      Value: null,
      AppliesTo: MarketplacePromoEligibility.EntireOrder,
      ScopeToSupplier: false,
      Supplier: null,
      SKUs: [],
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
  buildEligibleExpression(safeXp: PromotionXp, selectedSupplier?: MarketplaceSupplier): string {
    let eligibleExpression = '';
    switch(safeXp?.AppliesTo) {
      case MarketplacePromoEligibility.SpecificSupplier:
        eligibleExpression = `item.SupplierID = '${selectedSupplier?.ID}'`;
        break;
      case MarketplacePromoEligibility.SpecificSKUs:
        const skuArr = safeXp?.SKUs?.map(sku => `item.ProductID = '${sku}'`);
        skuArr.forEach((exp, i) => eligibleExpression = i === 0 ? `${eligibleExpression} ${exp}` : `${eligibleExpression} or ${exp}`);
        break;
      default: 
        // NOTE: The default expression is "true", which will allow the promotion to be applied to any order
        // in the platform.  This is selected if no eligibilty requirements are defined during the create
        // process
        eligibleExpression = 'true';
        break;
    };
    switch (safeXp?.MinReq?.Type) {
      case MinRequirementType.MinPurchase:
        if (safeXp?.AppliesTo === MarketplacePromoEligibility.SpecificSupplier) {
          eligibleExpression = `${eligibleExpression} and items.total(SupplierID = '${selectedSupplier?.ID}') >= ${safeXp?.MinReq?.Int}`;
        } else if (safeXp?.AppliesTo === MarketplacePromoEligibility.SpecificSKUs) {
          eligibleExpression = `${eligibleExpression} and Order.Subtotal >= ${safeXp?.MinReq?.Int}`
        } else {
          `Order.Subtotal >= ${safeXp?.MinReq?.Int}`;
        }
        break;
      case MinRequirementType.MinItemQty:
        if (safeXp.AppliesTo === MarketplacePromoEligibility.SpecificSupplier) {
          eligibleExpression = `${eligibleExpression} and items.Quantity(SupplierID = '${selectedSupplier?.ID}') >= ${safeXp?.MinReq?.Int}`
        } else if (safeXp?.AppliesTo === MarketplacePromoEligibility.SpecificSKUs) {
          eligibleExpression = `${eligibleExpression} and Order.LineItemCount >= ${safeXp?.MinReq?.Int}`
        } else {
          eligibleExpression = `Order.LineItemCount >= ${safeXp?.MinReq?.Int}`
        }
        break;
    }
    if (safeXp?.MaxShipCost) {
      safeXp?.MinReq?.Type ? eligibleExpression = `Order.ShippingCost < ${safeXp?.MaxShipCost}`
      :
      eligibleExpression = `Order.ShippingCost < ${safeXp?.MaxShipCost}`
    }
    return eligibleExpression.trim();
  }

  buildValueExpression(safeXp: PromotionXp, selectedSupplier?: MarketplaceSupplier): string {
    let valueExpression = 'Order.Subtotal';
    switch(safeXp?.AppliesTo) {
      case MarketplacePromoEligibility.SpecificSupplier: 
        valueExpression = 'item.LineSubtotal'
        if (safeXp?.Type === MarketplacePromoType.Percentage) { valueExpression = `item.LineSubtotal * ${safeXp?.Value / 100}`}
        if (safeXp?.Type === MarketplacePromoType.FixedAmount) { valueExpression = `${safeXp?.Value} / items.count(SupplierID = '${selectedSupplier?.ID}')`}
        break;
      case MarketplacePromoEligibility.SpecificSKUs:
        if (safeXp?.Type === MarketplacePromoType.Percentage) { valueExpression = `item.LineSubtotal * ${safeXp?.Value / 100}` } 
        if (safeXp?.Type === MarketplacePromoType.FixedAmount) { valueExpression = `item.Quantity * ${safeXp?.Value}` }
        break;
      default: 
        if (safeXp?.Type === MarketplacePromoType.Percentage) { valueExpression = `${valueExpression} * ${safeXp.Value / 100}` }
        if (safeXp?.Type === MarketplacePromoType.FixedAmount) { valueExpression = `${valueExpression} - ${safeXp?.Value}` }
        break;
    };
    if (safeXp?.Type === 'FreeShipping') { valueExpression = 'Order.ShippingCost'};
    return valueExpression.trim();
  }
}
