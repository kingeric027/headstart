import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig, applicationConfiguration } from '@app-seller/config/app.config';
import { OcTokenService, Product } from '@ordercloud/angular-sdk';
import { ListPage, Asset } from '@ordercloud/headstart-sdk';

// these models can be deleted after the SDK is updated

export interface MarketplaceKitProduct {
    ID: string;
    Name: string;
    Product: Product;
    Images: Asset[];
    Attachments: Asset[];
    ProductAssignments: KitProduct;
}
export interface KitProductDocument extends Document {
    Doc: KitProduct;
}
export interface KitProduct {
    ProductsInKit: ProductInKit[];
}
export interface ProductInKit {
    ID: string;
    MinQty?: number;
    MaxQty?: number;
    Variants?: object[];
    SpecCombo?: string;
    Static: boolean;
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
    async Update(product: MarketplaceKitProduct) {
        const url = `${this.appConfig.middlewareUrl}/kitproducts/${product.Product.ID}`;
        return await this.http.put<MarketplaceKitProduct>(url, product, this.headers).toPromise();
    }
    async Delete(productID: string) {
        const url = `${this.appConfig.middlewareUrl}/kitproducts/${productID}`;
        return await this.http.delete<MarketplaceKitProduct>(url, this.headers).toPromise();
    }
    async Create(product: MarketplaceKitProduct) {
        const url = `${this.appConfig.middlewareUrl}/kitproducts`;
        return await this.http.post<MarketplaceKitProduct>(url, product, this.headers).toPromise();
    }
}