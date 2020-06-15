import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  OcProductService,
  Product,
  OcPriceScheduleService,
  OcCatalogService,
  ProductAssignment,
  ProductCatalogAssignment,
} from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class ProductService extends ResourceCrudService<Product> {
  emptyResource = {
    Product: {
      OwnerID: '',
      DefaultPriceScheduleID: '',
      AutoForward: false,
      Active: false,
      ID: null,
      Name: null,
      Description: null,
      QuantityMultiplier: 1,
      ShipWeight: null,
      ShipHeight: null,
      ShipWidth: null,
      ShipLength: null,
      ShipFromAddressID: null,
      Inventory: null,
      DefaultSupplierID: null,
      xp: {
        IntegrationData: null,
        IsResale: false,
        Facets: {},
        Images: [],
        Status: null,
        HasVariants: false,
        Note: '',
        Tax: {
          Category: null,
          Code: null,
          Description: null,
        },
        UnitOfMeasure: {
          Unit: null,
          Qty: null,
        },
        ProductType: null,
        StaticContent: null,
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
    Specs: [],
    Variants: [],
  };

  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    private ocProductsService: OcProductService,
    private ocPriceScheduleService: OcPriceScheduleService
  ) {
    super(router, activatedRoute, ocProductsService, '/products', 'products');
  }

  async updateProductCatalogAssignments(
    add: ProductAssignment[],
    del: ProductAssignment[],
    buyerID: string
  ): Promise<void> {
    const addRequests = add.map(newAssignment => this.ocProductsService.SaveAssignment(newAssignment).toPromise());
    const deleteRequests = del.map(assignmentToRemove =>
      this.ocProductsService
        .DeleteAssignment(assignmentToRemove.ProductID, buyerID, { userGroupID: assignmentToRemove.UserGroupID })
        .toPromise()
    );
    await Promise.all([...addRequests, ...deleteRequests]);
  }
}
