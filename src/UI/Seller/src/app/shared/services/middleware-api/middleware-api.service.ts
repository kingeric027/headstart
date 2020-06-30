import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService, Order } from '@ordercloud/angular-sdk';
import { SuperMarketplaceProduct } from 'marketplace-javascript-sdk';

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
  ) {}

  async uploadStaticContent(file: File, productID: string, fileName: string): Promise<SuperMarketplaceProduct> {
    const form = new FormData();
    form.append('file', file);
    const url = `${this.appConfig.middlewareUrl}/${
      this.appConfig.marketplaceID
    }/static-content/${productID}/${fileName}`;
    return await this.http.post<SuperMarketplaceProduct>(url, form, this.headers).toPromise();
  }

  async deleteStaticContent(fileName: string, productID: string): Promise<SuperMarketplaceProduct> {
    const url = `${this.appConfig.middlewareUrl}/${
      this.appConfig.marketplaceID
    }/static-content/${productID}/${fileName}`;
    return await this.http.delete<SuperMarketplaceProduct>(url, this.headers).toPromise();
  }

  async acknowledgeQuoteOrder(orderID: string): Promise<Order> {
    const url = `${this.appConfig.middlewareUrl}/order/acknowledgequote/${orderID}`;
    return await this.http.post<Order>(url, this.headers).toPromise();
  }

  async isLocationDeletable(locationID: string): Promise<boolean> {
    const url = `${this.appConfig.middlewareUrl}/supplier/candelete/${locationID}`;
    return await this.http.get<boolean>(url, this.headers).toPromise();
  }
}
