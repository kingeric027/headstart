import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService, Order, Product } from '@ordercloud/angular-sdk';
import { SuperMarketplaceProduct, MarketplaceSupplier, ListPage } from '@ordercloud/headstart-sdk';
import { AssetForDelivery } from 'marketplace-javascript-sdk';

export interface SupplierFilterConfigDocument extends Document {
  Doc: SupplierFilterConfig;
}
export interface SupplierFilterConfig {
  Display: string;
  Path: string;
  Items: Filter[];
  AllowSupplierEdit: boolean;
  AllowSellerEdit: boolean;
  BuyerAppFilterType: BuyerAppFilterType;
}
export declare enum BuyerAppFilterType {
  SelectOption = 'SelectOption',
  NonUI = 'NonUI',
}
export interface Filter {
  Text: string;
  Value: string;
}
@Injectable({
  providedIn: 'root',
})
export class MiddlewareAPIService {
  readonly headers = {
    headers: new HttpHeaders({
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    }),
  };
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) { }

  async acknowledgeQuoteOrder(orderID: string): Promise<Order> {
    const url = `${this.appConfig.middlewareUrl}/order/acknowledgequote/${orderID}`;
    return await this.http.post<Order>(url, this.headers).toPromise();
  }

  async isLocationDeletable(locationID: string): Promise<boolean> {
    const url = `${this.appConfig.middlewareUrl}/supplier/candelete/${locationID}`;
    return await this.http.get<boolean>(url, this.headers).toPromise();
  }

  async updateSupplier(supplierID: string, supplier: any): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/supplier/${supplierID}`;
    return await this.http.patch<MarketplaceSupplier>(url, supplier, this.headers).toPromise();
  }

  async getSupplierFilterConfig(): Promise<ListPage<SupplierFilterConfigDocument>> {
    const url = `${this.appConfig.middlewareUrl}/supplierfilterconfig`;
    return await this.http.get<ListPage<SupplierFilterConfigDocument>>(url, this.headers).toPromise();
  }
}
