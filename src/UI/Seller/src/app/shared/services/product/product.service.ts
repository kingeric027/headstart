import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import {
  OcProductService,
  ListProduct,
  Product,
  OcPriceScheduleService,
  PriceSchedule,
  OcCatalogService,
} from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { MarketPlaceProduct, PUBLISHED } from '@app-seller/shared/models/MarketPlaceProduct.interface';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class ProductService extends ResourceCrudService<Product> {
  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    private ocProductsService: OcProductService,
    private ocPriceScheduleService: OcPriceScheduleService,
    private ocCatalogService: OcCatalogService
  ) {
    super(router, activatedRoute, ocProductsService, '/products', 'products');
  }

  async getMarketPlaceProductByID(productID: string): Promise<MarketPlaceProduct> {
    const orderCloudProduct = await this.ocProductsService.Get(productID).toPromise();
    let priceSchedule = {};
    if (orderCloudProduct.DefaultPriceScheduleID) {
      priceSchedule = await this.ocPriceScheduleService.Get(orderCloudProduct.DefaultPriceScheduleID).toPromise();
    }
    return { ...orderCloudProduct, PriceSchedule: priceSchedule };
  }

  async createNewMarketPlaceProduct(marketPlaceProduct: MarketPlaceProduct): Promise<MarketPlaceProduct> {
    marketPlaceProduct.Active = true;
    marketPlaceProduct.PriceSchedule.Name = `Default_Marketplace_Buyer${marketPlaceProduct.Name}`;
    const newPriceSchedule = await this.ocPriceScheduleService.Create(marketPlaceProduct.PriceSchedule).toPromise();
    marketPlaceProduct.DefaultPriceScheduleID = newPriceSchedule.ID;
    const newProduct = await this.ocProductsService.Create(marketPlaceProduct).toPromise();

    // assignment of the price schedule to the product cannot be made until after the
    // product is assigned to the catalog, which will not be done until the supplier
    // approves and assigns the product to the buyer catalog

    this.listResources();

    // mocking the return value of the create marketplace product route
    return marketPlaceProduct;
  }

  async updateMarketPlaceProduct(marketPlaceProduct: MarketPlaceProduct): Promise<MarketPlaceProduct> {
    await this.ocProductsService.Save(marketPlaceProduct.ID, marketPlaceProduct).toPromise();
    await this.ocPriceScheduleService
      .Save(marketPlaceProduct.DefaultPriceScheduleID, marketPlaceProduct.PriceSchedule)
      .toPromise();
    this.listResources();

    // mocking the return value of the create marketplace product route
    return marketPlaceProduct;
  }
}
