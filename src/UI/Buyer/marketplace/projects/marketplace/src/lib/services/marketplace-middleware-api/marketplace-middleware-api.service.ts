import { Injectable } from "@angular/core";
import { HttpHeaders, HttpClient } from "@angular/common/http";
import { OcTokenService } from "@ordercloud/angular-sdk";
import { ShopperContextService } from "../shopper-context/shopper-context.service";
import { SupplierCategoryConfig } from "../../shopper-context";

@Injectable({
  providedIn: "root"
})
export class MarketplaceMiddlewareApiService {
  readonly options = {
    headers: new HttpHeaders({
      "Content-Type": "application/json",
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`
    })
  };
  constructor(
    private ocTokenService: OcTokenService,
    private http: HttpClient,
    private context: ShopperContextService
  ) {}

  getMarketplaceSupplierCategories(
    marketplaceID: string
  ): Promise<SupplierCategoryConfig> {
    return this.http
      .get<SupplierCategoryConfig>(
        `${this.context.appSettings.middlewareUrl}/marketplace/${marketplaceID}/supplier/category/config`,
        this.options
      )
      .toPromise();
  }
}
