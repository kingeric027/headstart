import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import {} from 'ordercloud-javascript-sdk';
import { SuperMarketplaceProduct, HeadStartSDK } from '@ordercloud/headstart-sdk';

@Injectable()
export class MeProductResolver implements Resolve<SuperMarketplaceProduct> {
  constructor() {}

  resolve(route: ActivatedRouteSnapshot): Promise<SuperMarketplaceProduct> {
    return HeadStartSDK.Mes.GetSuperProduct(route.params.productID);
  }
}
