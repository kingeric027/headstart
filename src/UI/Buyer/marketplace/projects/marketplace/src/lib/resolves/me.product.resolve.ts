import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { } from 'ordercloud-javascript-sdk';
import { SuperMarketplaceProduct, HeadStartSDK } from '@ordercloud/headstart-sdk';
import { TempSdk } from '../services/temp-sdk/temp-sdk.service';

@Injectable()
export class MeProductResolver implements Resolve<SuperMarketplaceProduct> {
  constructor(private tempSdk: TempSdk) { }

  async resolve(route: ActivatedRouteSnapshot): Promise<SuperMarketplaceProduct> {
    // TODO: strongly type this once headstart sdk includes ProductType 'Kit'
    const superProduct = await HeadStartSDK.Mes.GetSuperProduct(route.params.productID) as any;
    console.log(`SUPER PRODUCT`, superProduct)
    if (superProduct.Product.xp.ProductType === 'Kit') {
      const kitProduct = await this.tempSdk.getKitProduct(superProduct.Product.ID);
      console.log(`KIT PRODUCT`, superProduct)
      return kitProduct;
    }
    return superProduct;
  }
}
