import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ListPageWithFacets, Tokens } from 'ordercloud-javascript-sdk';
import { ListArgs } from '@ordercloud/headstart-sdk';
import { AppConfig, MarketplaceMeProduct, SupplierFilterConfigDocument } from '../../shopper-context';
import { ListPage } from '@ordercloud/headstart-sdk';

// WHOPLE FILE TO BE REPLACED BY SDK

interface MarketplaceCatalogAssignmentRequest {
  CatalogIDs: string[];
}

@Injectable({
  providedIn: 'root',
})
export class TempSdk {
  constructor(private http: HttpClient, private appConfig: AppConfig) {}

  buildHeaders(): HttpHeaders {
    return new HttpHeaders({
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
  }

  createHttpParams(args: ListArgs): HttpParams {
    let params = new HttpParams();
    Object.entries(args).forEach(([key, value]) => {
      if (key !== 'filters' && value) {
        params = params.append(key, value.toString());
      }
    });
    Object.entries(args.filters).forEach(([key, value]) => {
      if ((typeof value !== 'object' && value) || (value && value.length)) {
        params = params.append(key, value.toString());
      }
    });
    return params;
  }

  async listMeProducts(args: ListArgs): Promise<ListPageWithFacets<MarketplaceMeProduct>> {
    const url = `${this.appConfig.middlewareUrl}/me/products`;
    return await this.http
      .get<ListPageWithFacets<MarketplaceMeProduct>>(url, {
        headers: this.buildHeaders(),
        params: this.createHttpParams(args),
      })
      .toPromise();
  }

  async getSupplierFilterConfig(): Promise<ListPage<SupplierFilterConfigDocument>> {
    const url = `${this.appConfig.middlewareUrl}/supplierfilterconfig`;
    return await this.http
      .get<ListPage<SupplierFilterConfigDocument>>(url, {
        headers: this.buildHeaders(),
      })
      .toPromise();
  }

  async submitOrderClaim(orderID: string): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/order/${orderID}/lineitem/status`;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${Tokens.GetAccessToken()}`,
    });
    return await this.http.post(url, {}, { headers }).toPromise();
  }
}
