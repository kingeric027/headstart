import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { MarketplaceBuyer } from '@ordercloud/headstart-sdk';

// WHOPLE FILE TO BE REPLACED BY SDK

export interface SuperMarketplaceBuyer {
  Buyer: MarketplaceBuyer;
  Markup: BuyerMarkup;
}

interface BuyerMarkup {
  Percent: number;
}

@Injectable({
  providedIn: 'root',
})
export class BuyerTempService {
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  private buildHeaders(): HttpHeaders {
    return new HttpHeaders({
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
  }

  async get(buyerID: string): Promise<SuperMarketplaceBuyer> {
    const url = `${this.appConfig.middlewareUrl}/buyer/${buyerID}`;
    return await this.http.get<SuperMarketplaceBuyer>(url, { headers: this.buildHeaders() }).toPromise();
  }

  async create(superBuyer: SuperMarketplaceBuyer): Promise<SuperMarketplaceBuyer> {
    const url = `${this.appConfig.middlewareUrl}/buyer`;
    return await this.http.post<SuperMarketplaceBuyer>(url, superBuyer, { headers: this.buildHeaders() }).toPromise();
  }

  async save(buyerID: string, superBuyer: SuperMarketplaceBuyer): Promise<SuperMarketplaceBuyer> {
    const url = `${this.appConfig.middlewareUrl}/buyer/${buyerID}`;
    return await this.http.put<SuperMarketplaceBuyer>(url, superBuyer, { headers: this.buildHeaders() }).toPromise();
  }
}
