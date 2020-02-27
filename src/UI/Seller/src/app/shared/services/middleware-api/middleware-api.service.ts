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
  ) { }

  async getSuperMarketplaceProductByID(productID: string): Promise<any> {
    const url = `${this.baseUrl}/products/${productID}`;
    return await this.http.get(url, this.headers).toPromise();
  }

  async createNewSuperMarketplaceProduct(
    superMarketplaceProduct: SuperMarketplaceProduct
  ): Promise<SuperMarketplaceProduct> {
    superMarketplaceProduct.Product.xp.Status = ObjectStatus.Draft;
    superMarketplaceProduct.Product.Active = true;
    superMarketplaceProduct.PriceSchedule.Name = `Default_Marketplace_Buyer${superMarketplaceProduct.Product.Name}`;
    const url = `${this.baseUrl}/products`;
    return await this.http.post<SuperMarketplaceProduct>(url, superMarketplaceProduct, this.headers).toPromise();
  }

  async updateMarketplaceProduct(superMarketplaceProduct: SuperMarketplaceProduct): Promise<SuperMarketplaceProduct> {
    // TODO: Temporary while Product set doesn't reflect the current strongly typed Xp
    superMarketplaceProduct.Product.xp.Status = ObjectStatus.Draft;
    const url = `${this.baseUrl}/products/${superMarketplaceProduct.Product.ID}`;
    return await this.http.put<SuperMarketplaceProduct>(url, superMarketplaceProduct, this.headers).toPromise();
  }

  async uploadProductImage(file: File, productID: string): Promise<SuperMarketplaceProduct> {
    const form = new FormData();
    form.append('file', file);
    const url = `${this.appConfig.middlewareUrl}/${this.appConfig.marketplaceID}/images/product/${productID}`;
    return await this.http.post<SuperMarketplaceProduct>(url, form, this.headers).toPromise();
  }

  async uploadStaticContent(file: File, productID: string, fileName: string): Promise<SuperMarketplaceProduct> {
    try {
      const form = new FormData();
      form.append('file', file);
      const url = `${this.appConfig.middlewareUrl}/${this.appConfig.marketplaceID}/static-content/${productID}/${fileName}`;
      return await this.http.post<SuperMarketplaceProduct>(url, form, this.headers).toPromise();
    } catch (ex) {
      throw ex;
    }
  }

  async deleteStaticContent(fileName: string, productID: string): Promise<SuperMarketplaceProduct> {
    const url = `${this.appConfig.middlewareUrl}/${this.appConfig.marketplaceID}/static-content/${productID}/${fileName}`;
    return await this.http.delete<SuperMarketplaceProduct>(url, this.headers).toPromise();
  }
}