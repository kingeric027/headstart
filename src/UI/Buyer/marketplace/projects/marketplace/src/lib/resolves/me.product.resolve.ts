import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { OcMeService, ListSpec, Spec } from '@ordercloud/angular-sdk';
import { each as _each } from 'lodash';
import { Observable, of } from 'rxjs';
import { MarketplaceProduct, ListMarketplaceProduct } from '../shopper-context';

@Injectable()
export class MeListRelatedProductsResolver implements Resolve<ListMarketplaceProduct> {
  constructor(private service: OcMeService) {}

  resolve(
    route: ActivatedRouteSnapshot
  ): Observable<Array<MarketplaceProduct>> | Promise<Array<MarketplaceProduct>> | any {
    // const product = route.parent.data.product as MarketplaceProduct;
    // if (!product.xp || !product.xp.RelatedProducts) {
    //   return of([]);
    // }
    // const calls: Array<Promise<MarketplaceProduct>> = new Array<Promise<MarketplaceProduct>>();
    // product.xp.RelatedProducts.forEach((id: string) => {
    //   calls.push(this.service.GetProduct(id).toPromise());
    // });
    //return Promise.all(calls);
  }
}

@Injectable()
export class MeProductResolver implements Resolve<MarketplaceProduct> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<MarketplaceProduct> | Promise<MarketplaceProduct> | any {
    return this.service.GetProduct(route.params.productID);
  }
}

@Injectable()
export class MeListSpecsResolver implements Resolve<ListSpec> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<ListSpec> | Promise<ListSpec> | any {
    return this.service.ListSpecs(route.params.productID);
  }
}

@Injectable()
export class MeSpecsResolver implements Resolve<Spec> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Spec> | Promise<Spec> | any {
    const list = route.parent.data.specList as ListSpec;
    const productID = route.parent.params.productID;
    const calls: Array<Promise<Spec>> = new Array<Promise<Spec>>();
    list.Items.forEach((item: Spec) => {
      calls.push(this.service.GetSpec(productID, item.ID).toPromise());
    });
    return Promise.all(calls);
  }
}
