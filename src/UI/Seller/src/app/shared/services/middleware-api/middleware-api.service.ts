import { Injectable, Inject } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { OcTokenService, Supplier, Buyer } from '@ordercloud/angular-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { SuperMarketplaceProduct, DRAFT } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { OrchestrationLog } from '@app-seller/reports/models/orchestration-log';
import { ListPage } from './listPage.interface';
import { ListArgs } from './listArgs.interface';

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

  async getSuperMarketplaceProductByID(productID: string): Promise<any> {
    const url = `${this.baseUrl}/products/${productID}`;
    return await this.http.get(url, this.headers).toPromise();
  }

  async createNewSuperMarketplaceProduct(
    superMarketplaceProduct: SuperMarketplaceProduct
  ): Promise<SuperMarketplaceProduct> {
    superMarketplaceProduct.Product.xp.Status = DRAFT;
    superMarketplaceProduct.PriceSchedule.Name = `Default_Marketplace_Buyer${superMarketplaceProduct.Product.Name}`;
    const url = `${this.baseUrl}/products`;
    return await this.http.post<SuperMarketplaceProduct>(url, superMarketplaceProduct, this.headers).toPromise();
  }

  async updateMarketplaceProduct(superMarketplaceProduct: SuperMarketplaceProduct): Promise<SuperMarketplaceProduct> {
    // TODO: Temporary while Product set doesn't reflect the current strongly typed Xp
    superMarketplaceProduct.Product.xp.Status = DRAFT;
    const url = `${this.baseUrl}/products/${superMarketplaceProduct.Product.ID}`;
    return await this.http.put<SuperMarketplaceProduct>(url, superMarketplaceProduct, this.headers).toPromise();
  }

  async uploadProductImage(file: File, productID: string): Promise<SuperMarketplaceProduct> {
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}`;
    return await this.http.post<SuperMarketplaceProduct>(url, this.formify(file), this.headers).toPromise();
  }

  async deleteProductImage(productID: string, imageUrl: string): Promise<SuperMarketplaceProduct> {
    const imageName = imageUrl.split('/').slice(-1)[0];
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}/${imageName}`;
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
    return form;
  }

  async getMySupplier(supplierID: string): Promise<Supplier> {
    const url = `${this.baseUrl}/supplier/me/${supplierID}`;
    return await this.http.get(url, this.headers).toPromise();
  }
}
