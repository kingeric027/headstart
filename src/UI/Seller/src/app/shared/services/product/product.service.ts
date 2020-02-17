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
import { MarketPlaceProduct, DRAFT } from '@app-seller/shared/models/MarketPlaceProduct.interface';

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
    Product: {
      OwnerID: '',
      DefaultPriceScheduleID: '',
      AutoForward: false,
      Active: false,
      ID: null,
      Name: null,
      Description: null,
      QuantityMultiplier: null,
      ShipWeight: null,
      ShipHeight: null,
      ShipWidth: null,
      ShipLength: null,
      ShipFromAddressID: null,
      Inventory: null,
      DefaultSupplierID: null,
      xp: {
        IntegrationData: null,
        Facets: null,
        Images: [],
        Status: null,
        HasVariants: false,
        Note: '',
        Tax: {
          Category: null,
          Code: null,
          Description: null,
        },
        UnitOfMeasure: null,
      },
    },
    PriceSchedule: {
      ID: '',
      Name: '',
      ApplyTax: false,
      ApplyShipping: false,
      MinQuantity: 1,
      MaxQuantity: null,
      UseCumulativeQuantity: false,
      RestrictedQuantity: false,
      PriceBreaks: [
        {
          Quantity: 1,
          Price: null,
        },
      ],
      xp: {},
    },
  };
}
