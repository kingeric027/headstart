import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Tokens } from 'ordercloud-javascript-sdk';
import { AppConfig } from '../shopper-context';
import { SuperMarketplaceProduct } from 'marketplace-javascript-sdk';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable()
export class MeProductResolver implements Resolve<SuperMarketplaceProduct> {
  constructor(private appConfig: AppConfig, public httpClient: HttpClient) {}

  resolve(route: ActivatedRouteSnapshot): Promise<SuperMarketplaceProduct> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/me/products/${route.params.productID}`;
    return this.httpClient
      .get<SuperMarketplaceProduct>(url, { headers })
      .toPromise();
  }
}
