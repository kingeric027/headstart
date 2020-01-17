import { Injectable, Inject } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';

@Injectable({
  providedIn: 'root',
})
export class MiddlewareAPIService {
  readonly headers = {
    headers: new HttpHeaders({
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    }),
  };
  readonly baseUrl: string;
  readonly marketplaceID: string;
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    this.baseUrl = this.appConfig.middlewareUrl;
    this.marketplaceID = this.appConfig.marketplaceID;
  }

  async uploadProductImage(file: File, productID: string, index: number): Promise<void> {
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}/${index}`;
    await this.http.post(url, this.formify(file), this.headers).toPromise();
  }

  async deleteProductImage(productID: string, index: number): Promise<void> {
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}/${index}`;
    await this.http.delete(url, this.headers).toPromise();
  }

  private formify(file: File): FormData {
    const form = new FormData();
    form.append('file', file);
    return form;
  }
}
