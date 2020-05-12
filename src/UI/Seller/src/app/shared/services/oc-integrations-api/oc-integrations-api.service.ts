import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService, Order } from '@ordercloud/angular-sdk';
import { SuperMarketplaceProduct, ListPage } from 'marketplace-javascript-sdk';
import { SupportedRates } from '@app-seller/shared/models/supported-rates.interface';

@Injectable({
  providedIn: 'root',
})
export class OcIntegrationsAPIService {
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

  async getAvailableCurrencies(): Promise<SupportedRates[]> {
    const url = `${this.appConfig.ocMiddlewareUrl}/exchangerates/supportedrates`;
    const supportedRates = await this.http.get<ListPage<SupportedRates>>(url).toPromise();
    return supportedRates.Items;
  }
}
