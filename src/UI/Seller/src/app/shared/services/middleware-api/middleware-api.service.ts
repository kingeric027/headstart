import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
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

  async uploadProductImage(file: File, productID: string): Promise<SuperMarketplaceProduct> {
    const form = new FormData();
    form.append('file', file);
    const url = `${this.appConfig.middlewareUrl}/${this.appConfig.marketplaceID}/images/product/${productID}`;
    return await this.http.post<SuperMarketplaceProduct>(url, form, this.headers).toPromise();
  }
}