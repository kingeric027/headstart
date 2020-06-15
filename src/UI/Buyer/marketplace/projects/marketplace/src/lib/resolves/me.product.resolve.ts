import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import {} from 'ordercloud-javascript-sdk';
import { SuperMarketplaceProduct, MarketplaceSDK } from 'marketplace-javascript-sdk';

@Injectable()
export class MeProductResolver implements Resolve<SuperMarketplaceProduct> {
  constructor() {}

  resolve(route: ActivatedRouteSnapshot): Promise<SuperMarketplaceProduct> {
    return MarketplaceSDK.Mes.GetSuperProduct(route.params.productID);
  }
}
