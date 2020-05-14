import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { OcMeService, ListSpec, Spec, OcTokenService } from '@ordercloud/angular-sdk';
import { each as _each } from 'lodash';
import { Observable, of } from 'rxjs';
import { ListMarketplaceMeProduct, MarketplaceMeProduct, AppConfig } from '../shopper-context';
import { MarketplaceSDK, SuperMarketplaceProduct } from 'marketplace-javascript-sdk';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable()
export class MeListRelatedProductsResolver implements Resolve<ListMarketplaceMeProduct> {
  constructor(private service: OcMeService) {}

  resolve(
    route: ActivatedRouteSnapshot
  ): Observable<Array<MarketplaceMeProduct>> | Promise<Array<MarketplaceMeProduct>> | any {
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
export class MeProductResolver implements Resolve<SuperMarketplaceProduct> {
  constructor(
    private service: OcMeService,
    public ocTokenService: OcTokenService,
    private appConfig: AppConfig,
    public httpClient: HttpClient
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<SuperMarketplaceProduct> | Promise<SuperMarketplaceProduct> | any {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/me/products/${route.params.productID}`;
    return this.httpClient
      .get<SuperMarketplaceProduct>(url, { headers: headers })
      .toPromise();
    // return MarketplaceSDK.Products.MeGet(route.params.productID);
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
