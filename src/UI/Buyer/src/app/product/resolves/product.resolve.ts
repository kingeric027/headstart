import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import {
  OcMeService,
  ListBuyerProduct,
  BuyerProduct,
  ListBuyerSpec,
  BuyerSpec,
} from '@ordercloud/angular-sdk';
import { Observable } from 'rxjs';

@Injectable()
export class ListProductResolver implements Resolve<ListBuyerProduct> {
  constructor(private service: OcMeService) {}

  resolve(): Observable<ListBuyerProduct> | Promise<ListBuyerProduct> | any {
    return this.service.ListProducts();
  }
}

@Injectable()
export class ProductResolver implements Resolve<BuyerProduct> {
  constructor(private service: OcMeService) {}

  resolve(
    route: ActivatedRouteSnapshot
  ): Observable<BuyerProduct> | Promise<BuyerProduct> | any {
    return this.service.GetProduct(route.params.productID);
  }
}

@Injectable()
export class ListSpecsResolver implements Resolve<ListBuyerSpec> {
  constructor(private service: OcMeService) {}

  resolve(
    route: ActivatedRouteSnapshot
  ): Observable<ListBuyerSpec> | Promise<ListBuyerSpec> | any {
    return this.service.ListSpecs(route.params.productID);
  }
}

@Injectable()
export class SpecsResolver implements Resolve<BuyerSpec> {
  constructor(private service: OcMeService) {}

  resolve(
    route: ActivatedRouteSnapshot
  ): Observable<BuyerSpec> | Promise<BuyerSpec> | any {
    const list = route.parent.data.specList as ListBuyerSpec;
    const productID = route.parent.params.productID;
    const calls: Array<Promise<BuyerSpec>> = new Array<Promise<BuyerSpec>>();
    list.Items.forEach((item: BuyerSpec) => {
      calls.push(this.service.GetSpec(productID, item.ID).toPromise());
    });
    return Promise.all(calls);
  }
}
