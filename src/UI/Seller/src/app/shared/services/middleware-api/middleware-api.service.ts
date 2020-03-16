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
  readonly baseUrl: string;
  readonly cmsUrl: string;
  readonly marketplaceID: string;
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    this.cmsUrl = this.appConfig.cmsUrl;
    this.baseUrl = this.appConfig.middlewareUrl;
    this.marketplaceID = this.appConfig.marketplaceID;
  }

  async uploadProductImage(file: File, productID: string): Promise<SuperMarketplaceProduct> {
    debugger;
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}`;
    return await this.http.post<SuperMarketplaceProduct>(url, this.formify(file), this.headers).toPromise();
  }

  async deleteProductImage(productID: string, imageUrl: string): Promise<SuperMarketplaceProduct> {
    const imageName = imageUrl.split('/').slice(-1)[0];
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}/${imageName}`;
    return await this.http.delete<SuperMarketplaceProduct>(url, this.headers).toPromise();
  }

  async uploadStaticContent(file: File, productID: string, fileName: string): Promise<SuperMarketplaceProduct> {
    const url = `${this.baseUrl}/${this.marketplaceID}/static-content/${productID}/${fileName}`;
    return await this.http.post<SuperMarketplaceProduct>(url, this.formify(file), this.headers).toPromise();
  }

  async deleteStaticContent(url: string): Promise<SuperMarketplaceProduct> {
    // const url = `${this.baseUrl}/static-content/${fileName}`;
    return await this.http.delete<SuperMarketplaceProduct>(url, this.headers).toPromise();
  }

  async listTaxCodes(taxCategory, search, page, pageSize): Promise<any> {
    const url = `${
      this.baseUrl
      }/taxcodes?taxCategory=${taxCategory}&search=${search}&pageSize=${pageSize}&page=${page}`;
    return await this.http.get(url, this.headers).toPromise();
  }

  async createSupplier(supplier: Supplier): Promise<Supplier> {
    const url = `${this.baseUrl}/supplier`;
    return await this.http.post(url, supplier, this.headers).toPromise();
  }

  async createBuyer(buyer: Buyer): Promise<Supplier> {
    const url = `${this.baseUrl}/buyer`;
    return await this.http.post(url, buyer, this.headers).toPromise();
  }

  async listOrchestrationLogs(args: ListArgs = {}): Promise<ListPage<OrchestrationLog>> {
    return await this.list(`${this.baseUrl}/orchestration/logs`, args);
  }

  private list<T>(url: string, args: ListArgs = {}): Promise<T> {
    url = this.addUrlParams(url, args.filters);
    delete args.filters;
    url = this.addUrlParams(url, args);
    return this.http.get<T>(url, this.headers).toPromise();
  }

  private addUrlParams(baseUrl: string, object: Record<string, any> = {}): string {
    const symbol = baseUrl.includes('?') ? '&' : '?';
    const fields = Object.entries(object);
    const url = fields
      .filter(([key, value]) => value)
      .reduce((urlSoFar, [key, value]) => `${urlSoFar}${key}=${value}&`, `${baseUrl}${symbol}`);
    return url.replace(/[?&]+$/g, ''); // remove trailling & or ?
  }

  private formify(file: File): FormData {
    const form = new FormData();
    form.append('file', file);
    const url = `${this.appConfig.middlewareUrl}/${this.appConfig.marketplaceID}/images/product/${productID}`;
    return await this.http.post<SuperMarketplaceProduct>(url, form, this.headers).toPromise();
  }
}
