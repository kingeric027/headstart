import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService, Order, CatalogAssignment } from '@ordercloud/angular-sdk';
import {
  SuperMarketplaceProduct,
  MarketplaceCatalog,
  HeadStartSDK,
  ListPage,
  MarketplacePriceSchedule,
} from '@ordercloud/headstart-sdk';

// WHOPLE FILE TO BE REPLACED BY SDK

interface MarketplaceCatalogAssignmentRequest {
  CatalogIDs: string[];
}

@Injectable({
  providedIn: 'root',
})
export class CatalogsTempService {
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

  async get(buyerID: string, catalogID: string): Promise<MarketplaceCatalog> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs/${catalogID}`;
    return await this.http.get<MarketplaceCatalog>(url, { headers: this.buildHeaders() }).toPromise();
  }

  async list(buyerID: string): Promise<ListPage<MarketplaceCatalog>> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs`;
    return await this.http.get<ListPage<MarketplaceCatalog>>(url, { headers: this.buildHeaders() }).toPromise();
  }

  async create(buyerID: string, catalog: MarketplaceCatalog): Promise<MarketplaceCatalog> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs`;
    return await this.http.post<MarketplaceCatalog>(url, catalog, { headers: this.buildHeaders() }).toPromise();
  }

  async save(buyerID: string, catalogID: string, catalog: MarketplaceCatalog): Promise<MarketplaceCatalog> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs/${catalogID}`;
    return await this.http.post<MarketplaceCatalog>(url, catalog, { headers: this.buildHeaders() }).toPromise();
  }

  async delete(buyerID: string, catalogID: string): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs/${catalogID}`;
    return await this.http.delete(url, { headers: this.buildHeaders() }).toPromise();
  }

  async createProductAssignment(buyerID: string, catalogID: string, productID: string): Promise<CatalogAssignment> {
    const catalogAssignment = { CatalogID: catalogID, ProductID: productID };
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs/productassignments`;
    return await this.http
      .post<CatalogAssignment>(url, catalogAssignment, { headers: this.buildHeaders() })
      .toPromise();
  }

  async deleteProductAssignment(buyerID: string, catalogID: string, productID: string): Promise<any> {
    const url = `${
      this.appConfig.middlewareUrl
    }/buyers/${buyerID}/catalogs/productassignments/${catalogID}/${productID}`;
    return await this.http.delete(url, { headers: this.buildHeaders() }).toPromise();
  }

  async listProductAssignments(buyerID: string, catalogID = '', productID = ''): Promise<CatalogAssignment> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs/productassignments`;
    return await this.http.get<CatalogAssignment>(url, { headers: this.buildHeaders() }).toPromise();
  }

  async setLocationAssignments(buyerID: string, locationID: string, assignments: string[]): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/${locationID}/catalogs/assignments`;
    const marketplaceCatalogAssignmentRequest: MarketplaceCatalogAssignmentRequest = {
      CatalogIDs: assignments,
    };
    return await this.http.post(url, marketplaceCatalogAssignmentRequest, { headers: this.buildHeaders() }).toPromise();
  }

  async listLocationAssignments(buyerID: string, locationID: string): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs/assignments/${locationID}`;
    return await this.http.get(url, { headers: this.buildHeaders() }).toPromise();
  }

  async syncUserCatalogAssignmentsOnLocationAdd(buyerID: string, locationID: string, userID: string): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/buyers/${buyerID}/catalogs/user/${userID}/location/${locationID}/Add`;
    return await this.http.post(url, {}, { headers: this.buildHeaders() }).toPromise();
  }

  async syncUserCatalogAssignmentsOnLocationRemove(buyerID: string, locationID: string, userID: string): Promise<any> {
    const url = `${
      this.appConfig.middlewareUrl
    }/buyers/${buyerID}/catalogs/user/${userID}/location/${locationID}/Remove`;
    return await this.http.post(url, {}, { headers: this.buildHeaders() }).toPromise();
  }

  async GetPricingOverride(productID: string, buyerID: string): Promise<MarketplacePriceSchedule> {
    const url = `${this.appConfig.middlewareUrl}/products/${productID}/pricingoverride/buyer/${buyerID}`;
    return await this.http.get(url, { headers: this.buildHeaders() }).toPromise();
  }

  async CreatePricingOverride(
    productID: string,
    buyerID: string,
    priceSchedule: MarketplacePriceSchedule
  ): Promise<MarketplacePriceSchedule> {
    const url = `${this.appConfig.middlewareUrl}/products/${productID}/pricingoverride/buyer/${buyerID}`;
    return await this.http.post(url, priceSchedule, { headers: this.buildHeaders() }).toPromise();
  }

  async UpdatePricingOverride(
    productID: string,
    buyerID: string,
    priceSchedule: MarketplacePriceSchedule
  ): Promise<MarketplacePriceSchedule> {
    const url = `${this.appConfig.middlewareUrl}/products/${productID}/pricingoverride/buyer/${buyerID}`;
    return await this.http.put(url, priceSchedule, { headers: this.buildHeaders() }).toPromise();
  }

  async DeletePricingOverride(productID: string, buyerID: string): Promise<any> {
    const url = `${this.appConfig.middlewareUrl}/products/${productID}/pricingoverride/buyer/${buyerID}`;
    return await this.http.delete(url, { headers: this.buildHeaders() }).toPromise();
  }
}
