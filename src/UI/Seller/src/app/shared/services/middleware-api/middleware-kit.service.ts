import { Product, OcTokenService } from '@ordercloud/angular-sdk';
import { AssetForDelivery, ListPage } from '@ordercloud/headstart-sdk';
import { Injectable, Inject } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { Router } from '@angular/router';

export interface MarketplaceKitProduct {
    Product: Product;
    Images: AssetForDelivery;
    Attachments: AssetForDelivery;
    ProductAttachments: KitProduct;
}
export interface KitProductDocument extends Document {
    Doc: KitProduct;
}
export interface KitProduct {
    ProductsInKit: ProductInKit[];
}
export interface ProductInKit {
    ID: string;
    Required: boolean;
    MinQty: number;
    MaxQty: number;
}

@Injectable({
    providedIn: 'root',
})
export class MiddlewareKitService {
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

    async List(): Promise<ListPage<any>> {
        const url = `${this.appConfig.middlewareUrl}/kitproducts`;
        return await this.http.get<ListPage<MarketplaceKitProduct>>(url, this.headers).toPromise();
    }
    async Get(kitProductID: string): Promise<any> {
        const url = `${this.appConfig.middlewareUrl}/kitproducts/${kitProductID}`;
        return await this.http.get<MarketplaceKitProduct>(url, this.headers).toPromise();
    }
}