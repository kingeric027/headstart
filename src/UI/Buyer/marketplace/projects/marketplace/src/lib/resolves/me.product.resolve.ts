import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { OcMeService, ListBuyerProduct, BuyerProduct, ListBuyerSpec, BuyerSpec } from '@ordercloud/angular-sdk';
import { each as _each } from 'lodash';
import { Observable, of } from 'rxjs';

@Injectable()
export class MeListRelatedProductsResolver implements Resolve<ListBuyerProduct> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Array<BuyerProduct>> | Promise<Array<BuyerProduct>> | any {
    const product = route.parent.data.product as BuyerProduct;
    if (!product.xp || !product.xp.RelatedProducts) {
      return of([]);
    }
    const calls: Array<Promise<BuyerProduct>> = new Array<Promise<BuyerProduct>>();
    product.xp.RelatedProducts.forEach((id: string) => {
      calls.push(this.service.GetProduct(id).toPromise());
    });
    return Promise.all(calls);
  }
}

@Injectable()
export class MeProductResolver implements Resolve<BuyerProduct> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<BuyerProduct> | Promise<BuyerProduct> | any {
    return this.service.GetProduct(route.params.productID);
  }
}

@Injectable()
export class MeListSpecsResolver implements Resolve<ListBuyerSpec> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<ListBuyerSpec> | Promise<ListBuyerSpec> | any {
    return this.service.ListSpecs(route.params.productID);
  }
}

@Injectable()
export class MeSpecsResolver implements Resolve<BuyerSpec> {
  constructor(private service: OcMeService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<BuyerSpec> | Promise<BuyerSpec> | any {
    const list = route.parent.data.specList as ListBuyerSpec;
    const productID = route.parent.params.productID;
    const calls: Array<Promise<BuyerSpec>> = new Array<Promise<BuyerSpec>>();
    list.Items.forEach((item: BuyerSpec) => {
      calls.push(this.service.GetSpec(productID, item.ID).toPromise());
    });
    return Promise.all(calls);
  }
}
