import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  Product,
  OcPriceScheduleService,
  OcCatalogService,
  ProductAssignment,
  OcCategoryService,
  CategoryProductAssignment,
} from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { Products, ListPage } from 'ordercloud-javascript-sdk';
import { ChiliService, TecraDocument, TecraSpec } from '@app-seller/shared/services/middleware-api/middleware-chili.service';
import { ChiliConfig, ChiliSpec } from '@ordercloud/headstart-sdk';

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
          Category: null, // SEB-827 default tax category to TPP
          Code: null,
          Description: null,
        },
        UnitOfMeasure: {
          Unit: null,
          Qty: null,
        },
        ProductType: null,
        StaticContent: null,
        ArtworkRequired: false,
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
    private ocCategoryService: OcCategoryService,
    private ocPriceScheduleService: OcPriceScheduleService,
    private ocCatalogService: OcCatalogService,
    public currentUserService: CurrentUserService,
    public chiliService: ChiliService
  ) {
    super(router, activatedRoute, Products, currentUserService, '/products', 'products');
  }

  async updateProductCatalogAssignments(
    add: ProductAssignment[],
    del: ProductAssignment[],
    buyerID: string,
    priceScheduleID: string
  ): Promise<void> {
    add = add.map(a => {
      a.PriceScheduleID = priceScheduleID;
      return a;
    });
    const addRequests = add.map(newAssignment => Products.SaveAssignment(newAssignment));
    const deleteRequests = del.map(assignmentToRemove =>
      Products.DeleteAssignment(assignmentToRemove.ProductID, buyerID, { userGroupID: assignmentToRemove.UserGroupID })
    );
    await Promise.all([...addRequests, ...deleteRequests]);
  }

  async updateProductCategoryAssignments(
    add: CategoryProductAssignment[],
    del: CategoryProductAssignment[],
    buyerID: string
  ): Promise<void> {
    const addRequests = add.map(newAssignment =>
      this.ocCategoryService.SaveProductAssignment(buyerID, newAssignment).toPromise()
    );
    const deleteRequests = del.map(assignmentToRemove =>
      this.ocCategoryService
        .DeleteProductAssignment(buyerID, assignmentToRemove.CategoryID, assignmentToRemove.ProductID)
        .toPromise()
    );
    await Promise.all([...addRequests, ...deleteRequests]);
    }

    async getTecraDocuments(
        folder: string
    ): Promise<TecraDocument[]> {
        return this.chiliService.getDocuments(folder);
    }
    async getTecraDocumentsByFolder(
        folder: string
    ): Promise<TecraDocument[]> {
        return this.chiliService.getDocumentsByFolder(folder);
    }

    async getTecraSpecs(
        docID: string,
        folder: string
    ): Promise<TecraSpec[]> {
        return this.chiliService.getSpecs(docID, folder);
    }

    async listChiliConfigs(): Promise<ListPage<ChiliConfig>>
    {
        return this.chiliService.listChiliConfigs();
    }

    async saveChiliConfig(config: ChiliConfig): Promise<ChiliConfig>
    {
        return this.chiliService.saveChiliConfig(config);
    }
    async deleteChiliConfig(id: string)
    {
        this.chiliService.deleteChiliConfig(id);
    }
    async saveChiliSpec(spec: ChiliSpec): Promise<ChiliSpec> {
        return this.chiliService.saveChiliSpec(spec);
    }
    async deleteChiliSpec(id: string) {
        this.chiliService.deleteChiliSpec(id);
    }
}
