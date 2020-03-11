import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Supplier, Buyer, OcTokenService } from '@ordercloud/angular-sdk';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import {
  Configuration,
  MarketplaceSDK,
  SuperMarketplaceProduct,
  OrchestrationLog,
  ListPage,
} from 'marketplace-javascript-sdk';
import { ListArgs } from 'marketplace-javascript-sdk/dist/models/ListArgs';
import { MarketplaceBuyerLocation } from 'marketplace-javascript-sdk/dist/models/MarketplaceBuyerLocation';

@Injectable({
  providedIn: 'root',
})
export class MiddlewareAPIService {
  readonly marketplaceID: string;
  readonly headers = {
    headers: new HttpHeaders({
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    }),
  };
  readonly baseUrl: string;
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    this.baseUrl = this.appConfig.middlewareUrl;
    this.marketplaceID = this.appConfig.marketplaceID;
    Configuration.Set({
      baseApiUrl: this.appConfig.middlewareUrl,
    });
  }

  async getSuperMarketplaceProductByID(productID: string): Promise<any> {
    return await MarketplaceSDK.Products.Get(productID);
  }

  async listTaxCodes(taxCategory, search, page, pageSize): Promise<any> {
    return await MarketplaceSDK.TaxCodes.GetTaxCodes({ filters: { Category: taxCategory }, search, page, pageSize });
  }

  async createSupplier(supplier: Supplier): Promise<Supplier> {
    return await MarketplaceSDK.Suppliers.Create(supplier);
  }

  async createBuyer(buyer: Buyer): Promise<Supplier> {
    return await MarketplaceSDK.Buyers.Create(buyer);
  }

  async listOrchestrationLogs(args: ListArgs = {}): Promise<ListPage<OrchestrationLog>> {
    return await MarketplaceSDK.OrchestrationLogs.List(args);
  }

  async getMySupplier(supplierID: string): Promise<Supplier> {
    return await MarketplaceSDK.Suppliers.GetMySupplier(supplierID);
  }

  async getBuyerLocationByID(buyerID: string, buyerLocationID: string): Promise<any> {
    return await MarketplaceSDK.BuyerLocations.Get(buyerID, buyerLocationID);
  }

  async deleteBuyerLocation(buyerID: string, buyerLocationID: string): Promise<void> {
    return await MarketplaceSDK.BuyerLocations.Delete(buyerLocationID, buyerID);
  }

  async deleteProductImage(productID: string, imageUrl: string): Promise<SuperMarketplaceProduct> {
    const imageName = imageUrl.split('/').slice(-1)[0];
    return await MarketplaceSDK.Files.Delete(this.marketplaceID, productID, imageName);
  }

  async createNewSuperMarketplaceProduct(
    superMarketplaceProduct: SuperMarketplaceProduct
  ): Promise<SuperMarketplaceProduct> {
    superMarketplaceProduct.Product.xp.Status = 'Draft';
    superMarketplaceProduct.PriceSchedule.Name = `Default_Marketplace_Buyer${superMarketplaceProduct.Product.Name}`;
    return await MarketplaceSDK.Products.Post(superMarketplaceProduct);
  }

  async updateMarketplaceProduct(superMarketplaceProduct: SuperMarketplaceProduct): Promise<SuperMarketplaceProduct> {
    // TODO: Temporary while Product set doesn't reflect the current strongly typed Xp
    superMarketplaceProduct.Product.xp.Status = 'Draft';
    const url = `${this.baseUrl}/products/${superMarketplaceProduct.Product.ID}`;
    return await this.http.put<SuperMarketplaceProduct>(url, superMarketplaceProduct, this.headers).toPromise();
  }

  async uploadProductImage(file: File, productID: string): Promise<SuperMarketplaceProduct> {
    const url = `${this.baseUrl}/${this.marketplaceID}/images/product/${productID}`;
    return await this.http.post<SuperMarketplaceProduct>(url, this.formify(file), this.headers).toPromise();
  }

  async createBuyerLocation(buyerID: string, buyerLocation: MarketplaceBuyerLocation): Promise<any> {
    const url = `${this.baseUrl}/buyerlocations/${buyerID}`;
    return await this.http.post(url, buyerLocation).toPromise();
  }

  async updateBuyerLocationByID(
    buyerID: string,
    buyerLocationID: string,
    buyerLocation: MarketplaceBuyerLocation
  ): Promise<any> {
    const url = `${this.baseUrl}/buyerlocations/${buyerID}/${buyerLocationID}`;
    return await this.http.put(url, buyerLocation).toPromise();
  }

  private formify(file: File): FormData {
    const form = new FormData();
    form.append('file', file);
    return form;
  }
}
