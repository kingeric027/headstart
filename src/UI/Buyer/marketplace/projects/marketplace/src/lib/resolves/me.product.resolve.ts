import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { } from 'ordercloud-javascript-sdk';
import { SuperMarketplaceProduct, HeadStartSDK } from '@ordercloud/headstart-sdk';
import { TempSdk } from '../services/temp-sdk/temp-sdk.service';

@Injectable()
export class MeProductResolver implements Resolve<SuperMarketplaceProduct> {
  constructor(private tempSdk: TempSdk) { }

  resolve(route: ActivatedRouteSnapshot): Promise<SuperMarketplaceProduct> {
    return HeadStartSDK.Mes.GetSuperProduct(route.params.productID);
  }
}
