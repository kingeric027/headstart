import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { SupplierCategoryConfig, MarketplaceOrder, AppConfig, ProposedShipment, ProposedShipmentSelection } from '../../shopper-context';

@Injectable({
  providedIn: "root"
})
export class MarketplaceMiddlewareApiService {
  readonly baseUrl: string;
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    public appSettings: AppConfig
  ) {
    this.baseUrl = this.appSettings.middlewareUrl;
  }

  generateHeaders() {
    return {
      headers: new HttpHeaders({
        "Content-Type": "application/json",
        Authorization: `Bearer ${this.ocTokenService.GetAccess()}`
      })
    };
  }

  getMarketplaceSupplierCategories(
    marketplaceID: string
  ): Promise<SupplierCategoryConfig> {
    return this.http
      .get<SupplierCategoryConfig>(
        `${this.baseUrl}/marketplace/${marketplaceID}/supplier/category/config`,
        this.generateHeaders()
      )
      .toPromise();
  }

  getProposedShipments(orderID: string): Promise<ProposedShipment[]> {
    return this.http.get<ProposedShipment[]>(
      `${this.baseUrl}/proposedshipment/${orderID}`, this.generateHeaders())
    .toPromise();
  }

  selectShippingRate(orderID: string, selection: ProposedShipmentSelection): Promise<MarketplaceOrder> {
    return this.http.put<MarketplaceOrder>(
      `${this.baseUrl}/proposedshipment/${orderID}/select`, selection, this.generateHeaders())
    .toPromise();
  }

  calculateTax(orderID: string): Promise<MarketplaceOrder> {
    return this.http
      .post<MarketplaceOrder>(
        `${this.baseUrl}/orders/${orderID}/tax-transaction`,
        {},
        this.generateHeaders()
      )
      .toPromise();
  }
}
