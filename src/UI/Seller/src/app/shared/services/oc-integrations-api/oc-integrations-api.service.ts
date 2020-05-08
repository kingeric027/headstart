import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService, Order } from '@ordercloud/angular-sdk';
import { SuperMarketplaceProduct } from 'marketplace-javascript-sdk';
import { ExchangeRates } from '@app-seller/shared/models/exchange-rates.interface';

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

  async getAvailableCurrencies(base: string): Promise<ExchangeRates> {
    const url = `${this.appConfig.ocMiddlewareUrl}/exchangerates/${base}`;
    return await this.http.get<ExchangeRates>(url).toPromise();
  }
}
