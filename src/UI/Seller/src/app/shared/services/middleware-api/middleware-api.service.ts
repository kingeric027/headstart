import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { AppConfig } from '@app-seller/config/app.config';

@Injectable({
  providedIn: 'root',
})
export class MiddlewareAPIService {
  readonly headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    }),
  };
  readonly baseUrl: string;
  readonly marketplaceID: string;
  constructor(private ocTokenService: OcTokenService, private http: HttpClient, public appSettings: AppConfig) {
    this.baseUrl = this.appSettings.middlewareUrl;
    this.marketplaceID = this.appSettings.marketplaceID;
  }

  async uploadProductImage(file: File, productID: string, index: number): Promise<void> {
    const form = new FormData().append('file', file);
    await this.http
      .post(`${this.baseUrl}/${this.marketplaceID}/images/product/${productID}/${index}`, form, this.headers)
      .toPromise();
  }

  async deleteProductImage(productID: string, index: number): Promise<void> {
    await this.http
      .delete(`${this.baseUrl}/${this.marketplaceID}/images/product/${productID}/${index}`, this.headers)
      .toPromise();
  }
}
