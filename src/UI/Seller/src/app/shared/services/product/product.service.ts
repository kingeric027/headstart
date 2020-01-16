import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { OcProductService, ListProduct, Product } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { MarketPlaceProduct } from '@app-seller/shared/models/MarketPlaceProduct.interface';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class ProductService extends ResourceCrudService<Product> {
  constructor(router: Router, activatedRoute: ActivatedRoute, private ocProductsService: OcProductService) {
    super(router, activatedRoute, ocProductsService, '/products', 'products');
  }

  async getMarketPlaceProductDetailByID(productID: string): Promise<MarketPlaceProduct> {
    const marketPlaceProduct: MarketPlaceProduct = await this.ocProductsService.Get(productID).toPromise();
    // just returning the oc product for now
    return marketPlaceProduct;
  }

  async saveMarketPlaceProduct(marketPlaceProduct: MarketPlaceProduct): Promise<MarketPlaceProduct> {
    this.ocProductsService.Create(marketPlaceProduct);
    this.listResources();

    // mocking the return value of the create marketplace product route
    return marketPlaceProduct;
  }
}
