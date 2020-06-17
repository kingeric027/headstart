import { Component, Input, Output } from '@angular/core';
import {
  OcCatalogService,
  OcCategoryService,
  OcUserGroupService,
  Category,
  UserGroup,
  OcProductService,
  ProductAssignment,
  CategoryProductAssignment,
} from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/products/product.service';
import { MarketplaceBuyer, MarketplaceProduct } from 'marketplace-javascript-sdk';

@Component({
  selector: 'buyer-visibility-configuration-component',
  templateUrl: './buyer-visibility-configuration.component.html',
  styleUrls: ['./buyer-visibility-configuration.component.scss'],
})
export class BuyerVisibilityConfiguration {
  @Input()
  set buyer(value: MarketplaceBuyer) {
    this._buyer = value;
    this.fetchData();
  }

  @Input()
  set product(value: MarketplaceProduct) {
    this._product = value;
    this.fetchData();
  }

  _product: MarketplaceProduct = {};
  _buyer: MarketplaceBuyer = {};

  addCatalogAssignments: ProductAssignment[] = [];
  delCatalogAssignments: ProductAssignment[] = [];

  addCategoryAssignments: CategoryProductAssignment[];
  delCategoryAssignments: CategoryProductAssignment[];

  catalogAssignmentsEditable: ProductAssignment[] = [];
  catalogAssignmentsStatic: ProductAssignment[] = [];

  assignedCategoriesStatic: Category[][] = [];
  assignedCategoriesEditable: Category[][] = [];

  isEditing = false;
  isValidBuyerAssignment = false;
  areChanges = false;

  catalogs: UserGroup[] = [];

  // product must be assigned to the catalog for the usergroup and catagory asssignments,
  // however none of the visibility functionality is based on this assignment
  isAssignedToCatalog = false;

  constructor(
    private ocCategoryService: OcCategoryService,
    private ocCatalogService: OcCatalogService,
    private ocUserGroupService: OcUserGroupService,
    private ocProductService: OcProductService,
    private productService: ProductService
  ) {}

  async fetchData(): Promise<void> {
    if (Object.keys(this._product) && Object.keys(this._buyer)) {
      await this.getCatalogAssignments();
      await this.getCatalogs();
      await this.getCatalogAssignments();
      await this.getCategoryAssignments();
    }
  }

  resetCatalogAssignments(catalogAssignments: ProductAssignment[]): void {
    this.catalogAssignmentsEditable = catalogAssignments;
    this.catalogAssignmentsStatic = catalogAssignments;
  }

  resetCategoryAssignments(categoryAssignments: Category[][]): void {
    this.assignedCategoriesEditable = categoryAssignments;
    this.assignedCategoriesStatic = this.assignedCategoriesEditable;
  }

  isAssigned(userGroupID: string): boolean {
    return this.catalogAssignmentsEditable.some(c => c.UserGroupID === userGroupID);
  }

  toggleProductCatalogAssignment(catalog: UserGroup): void {
    if (this.isAssigned(catalog.ID)) {
      this.catalogAssignmentsEditable = this.catalogAssignmentsEditable.filter(c => c.UserGroupID !== catalog.ID);
    } else {
      const newCatalogAssignment: ProductAssignment = {
        ProductID: this._product.ID,
        UserGroupID: catalog.ID,
        BuyerID: this._buyer.ID,
      };
      this.catalogAssignmentsEditable = [...this.catalogAssignmentsEditable, newCatalogAssignment];
    }
    this.checkForProductCatalogAssignmentChanges();
  }

  updateStatus(): void {
    this.areChanges =
      !!this.addCatalogAssignments.length ||
      !!this.delCatalogAssignments.length ||
      !!this.addCategoryAssignments.length ||
      !!this.delCategoryAssignments.length;
  }

  async edit(): Promise<void> {
    this.isEditing = true;
  }

  handleDiscardChanges(): void {
    this.assignedCategoriesEditable = this.assignedCategoriesStatic;
    this.catalogAssignmentsEditable = this.catalogAssignmentsStatic;
    this.checkForProductCatalogAssignmentChanges();
  }

  async getCatalogAssignment(): Promise<void> {
    const catalogAssignmentResponse = await this.ocCatalogService
      .ListProductAssignments({ catalogID: this._buyer.ID, productID: this._product.ID })
      .toPromise();
    this.isAssignedToCatalog = catalogAssignmentResponse.Meta.TotalCount > 0;
  }

  async getCatalogs(): Promise<void> {
    const catalogsResponse = await this.ocUserGroupService
      .List(this._buyer.ID, {
        pageSize: 100,
        filters: {
          'xp.Type': 'Catalog',
        },
      })
      .toPromise();
    this.catalogs = catalogsResponse.Items;
  }

  async getCatalogAssignments(): Promise<void> {
    const catalogAssignmentRequests = this.catalogs.map(c =>
      this.ocProductService.ListAssignments({ userGroupID: c.ID, productID: this._product.ID }).toPromise()
    );
    const catalogAssignmentResponses = await Promise.all(catalogAssignmentRequests);
    const catalogAssignments = catalogAssignmentResponses.reduce((acc, curr) => acc.concat(curr.Items), []);
    this.resetCatalogAssignments(catalogAssignments);
  }

  async getCategoryAssignments(): Promise<void> {
    const categoryResponse = await this.ocCategoryService
      .ListProductAssignments(this._buyer.ID, {
        productID: this._product.ID,
      })
      .toPromise();
    const categoryIDs = categoryResponse.Items.map(c => c.CategoryID);
    const categoryAssignments = await this.getCategoryHierarchies(categoryIDs);
    this.resetCategoryAssignments(categoryAssignments);
  }

  async getCategoryHierarchies(categoryIDs: string[]): Promise<Category[][]> {
    const categoryHierarchyRequests = categoryIDs.map(c => this.getCategoryHierarchy(c));
    const categoryHierarchyResponses = await Promise.all(categoryHierarchyRequests);
    return categoryHierarchyResponses;
  }

  async getCategoryHierarchy(categoryID: string): Promise<Category[]> {
    const asssignedCategory = await this.ocCategoryService.Get(this._buyer.ID, categoryID).toPromise();
    return await this.addToCategoryHierarchy([], asssignedCategory);
  }

  async addToCategoryHierarchy(currentTree = [], category: Category): Promise<Category[]> {
    // recursive function to add currently selected category and all parents to an array

    if (!category.ParentID) {
      return [category, ...currentTree];
    } else {
      const parentCategory = await this.ocCategoryService.Get(this._buyer.ID, category.ParentID).toPromise();
      return await this.addToCategoryHierarchy([category, ...currentTree], parentCategory);
    }
  }

  async assignToCatalogIfNecessary(): Promise<void> {
    if (!this.isAssignedToCatalog) {
      await this.ocCatalogService
        .SaveProductAssignment({ CatalogID: this._buyer.ID, ProductID: this._product.ID })
        .toPromise();
    }
  }

  async executeProductCatalogAssignmentRequests(): Promise<void> {
    await this.assignToCatalogIfNecessary();
    await this.productService.updateProductCatalogAssignments(
      this.addCatalogAssignments,
      this.delCatalogAssignments,
      this._buyer.ID
    );
    await this.productService.updateProductCategoryAssignments(
      this.addCategoryAssignments,
      this.delCategoryAssignments,
      this._buyer.ID
    );
    this.resetCatalogAssignments(this.catalogAssignmentsEditable);
    this.resetCategoryAssignments(this.assignedCategoriesEditable);
    this.checkForProductCatalogAssignmentChanges();
    this.checkForCategoryAssignmentChanges();
  }

  checkForProductCatalogAssignmentChanges(): void {
    this.addCatalogAssignments = this.catalogAssignmentsEditable.filter(
      assignment => !JSON.stringify(this.catalogAssignmentsStatic).includes(assignment.UserGroupID)
    );
    this.delCatalogAssignments = this.catalogAssignmentsStatic.filter(
      assignment => !JSON.stringify(this.catalogAssignmentsEditable).includes(assignment.UserGroupID)
    );
    this.updateStatus();
  }

  checkForCategoryAssignmentChanges(): void {
    this.addCategoryAssignments = this.assignedCategoriesEditable
      .filter(
        editable =>
          !this.assignedCategoriesStatic.some(
            staticCategory => staticCategory[staticCategory.length - 1].ID === editable[editable.length - 1].ID
          )
      )
      .map(a => {
        return {
          CategoryID: a[a.length - 1].ID,
          ProductID: this._product.ID,
        };
      });
    this.delCategoryAssignments = this.assignedCategoriesStatic
      .filter(
        staticCategory =>
          !this.assignedCategoriesEditable.some(
            editable => staticCategory[staticCategory.length - 1].ID === editable[editable.length - 1].ID
          )
      )
      .map(a => {
        return {
          CategoryID: a[a.length - 1].ID,
          ProductID: this._product.ID,
        };
      });
    this.updateStatus();
  }

  updateCategoryAssignments(event: any): void {
    this.assignedCategoriesEditable = event;
    this.checkForCategoryAssignmentChanges();
  }

  handleClose(): void {
    this.isEditing = false;
  }
}
