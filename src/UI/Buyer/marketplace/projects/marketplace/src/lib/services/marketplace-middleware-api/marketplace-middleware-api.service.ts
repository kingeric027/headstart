import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { SupplierCategoryConfig, ShippingSelection, ShippingOptions, MarketplaceOrder, AppConfig } from '../../shopper-context';

@Injectable({
  providedIn: 'root'
})
export class MarketplaceMiddlewareApiService {
  readonly options = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`
    })
  };
  readonly baseUrl: string;
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    public appSettings: AppConfig,
  ) {
    this.baseUrl = this.appSettings.middlewareUrl;
  }

  getMarketplaceSupplierCategories(
    marketplaceID: string
  ): Promise<SupplierCategoryConfig> {
    return this.http
      .get<SupplierCategoryConfig>(
        `${this.baseUrl}/marketplace/${marketplaceID}/supplier/category/config`,
        this.options
      )
      .toPromise();
  }

  generateShippingRates(orderID: string): Promise<ShippingOptions[]> {
    return this.http.post<ShippingOptions[]>(
      `${this.baseUrl}/order/${orderID}/shipping-rate`, {})
    .toPromise();
  }

  selectShippingRate(orderID: string, selection: ShippingSelection): Promise<MarketplaceOrder> {
    return this.http.put<MarketplaceOrder>(
      `${this.baseUrl}/order/${orderID}/shipping-rate`, selection)
    .toPromise();
  }

  calculateTax(orderID: string): Promise<MarketplaceOrder> {
    return this.http.post<MarketplaceOrder>(
      `${this.baseUrl}/order/${orderID}/tax-transaction`, {})
    .toPromise();
  }
}
