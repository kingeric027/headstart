import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import {
  OcProductService,
  Product,
  OcPriceScheduleService,
  OcCatalogService,
  ProductAssignment,
  ProductCatalogAssignment,
} from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { MarketPlaceProduct } from '@app-seller/shared/models/MarketPlaceProduct.interface';

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
    marketPlaceProduct.ID = newProduct.ID;

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

  async updateProductCatalogAssignments(add: ProductAssignment[], del: ProductAssignment[]): Promise<void> {
    const addRequests = add.map(newAssignment => this.addProductCatalogAssignment(newAssignment));
    const deleteRequests = del.map(assignmentToRemove => this.removeProductCatalogAssignment(assignmentToRemove));
    await Promise.all([...addRequests, ...deleteRequests]);
  }

  addProductCatalogAssignment(assignment: ProductCatalogAssignment): Promise<void> {
    return this.ocCatalogService
      .SaveProductAssignment({ CatalogID: assignment.CatalogID, ProductID: assignment.ProductID })
      .toPromise();
  }

  removeProductCatalogAssignment(assignment: ProductCatalogAssignment) {
    return this.ocCatalogService.DeleteProductAssignment(assignment.CatalogID, assignment.ProductID).toPromise();
  }

  emptyResource = {
    ID: null,
    Name: null,
    Description: null,
    QuantityMultiplier: null,
    Shipping: {
      ShipWeight: null,
      ShipHeight: null,
      ShipWidth: null,
      ShipLength: null,
    },
    PriceSchedule: {
      PriceBreaks: [
        {
          Quantity: 1,
          Price: 0,
        },
      ],
    },
    HasVariants: null,
    Notes: null,
    UnitOfMeasure: null,
    Status: null,
    ShipFromAddressID: null,
    Inventory: null,
    DefaultSupplierID: null,
    xp: {
      TaxCode: {
        Category: null,
        Code: null,
        Description: null,
      },
      Data: {},
      Images: [],
    },
  };
}
