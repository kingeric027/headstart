import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { SupplierCategoryConfig, ShippingSelection, ShippingOptions, MarketplaceOrder, AppConfig } from '../../shopper-context';

@Injectable({
  providedIn: 'root'
})
export class MarketplaceMiddlewareApiService {
  readonly headers = {
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
        this.headers
      )
      .toPromise();
  }

  generateShippingRates(orderID: string): Promise<ShippingOptions[]> {
    return this.http.post<ShippingOptions[]>(
      `${this.baseUrl}/orders/${orderID}/shipping/generate-rates`, {}, this.headers)
    .toPromise();
  }

  selectShippingRate(orderID: string, selection: ShippingSelection): Promise<MarketplaceOrder> {
    return this.http.put<MarketplaceOrder>(
      `${this.baseUrl}/orders/${orderID}/shipping/select`, selection, this.headers)
    .toPromise();
  }

  calculateTax(orderID: string): Promise<MarketplaceOrder> {
    return this.http.post<MarketplaceOrder>(
      `${this.baseUrl}/orders/${orderID}/tax-transaction`, {}, this.headers)
    .toPromise();
  }
}
