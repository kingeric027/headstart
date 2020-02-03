import { Injectable, Inject } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { MarketPlaceProduct, MarketPlaceProductTaxCode } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { ListResource } from '../resource-crud/resource-crud.types';

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

  async uploadProductImage(file: File, productID: string): Promise<MarketPlaceProduct> {
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}`;
    return await this.http.post(url, this.formify(file), this.headers).toPromise();
  }

  async deleteProductImage(productID: string, imageUrl: string): Promise<MarketPlaceProduct> {
    const imageName = imageUrl.split('/').slice(-1)[0];
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}/${imageName}`;
    return await this.http.delete(url, this.headers).toPromise();
  }

  private formify(file: File): FormData {
    const form = new FormData();
    form.append('file', file);
    return form;
  }

  async listTaxCodes(taxCategory, search, page, pageSize): Promise<any> {
    const url = `${
      this.baseUrl
    }/taxcodes?taxCategory=${taxCategory}&search=${search}&pageSize=${pageSize}&page=${page}`;
    return await this.http.get(url, this.headers).toPromise();
  }
}
