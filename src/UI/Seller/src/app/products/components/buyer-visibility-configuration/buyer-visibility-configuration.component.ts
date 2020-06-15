import { Component, Input } from '@angular/core';
import {
  OcBuyerService,
  OcCatalogService,
  OcCategoryService,
  OcUserGroupService,
  Category,
  UserGroup,
  OcProductService,
  ProductAssignment,
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
  }
  @Input()
  set isAssigned(value: boolean) {
    this.resetIsAssignedStatus(value);
  }

  @Input()
  product: MarketplaceProduct = {};

  _buyer: MarketplaceBuyer = {};

  isAssignedEditable = false;
  isAssignedStatic = false;

  add: ProductAssignment[];
  del: ProductAssignment[];

  catalogAssignmentsEditable: ProductAssignment[] = [];
  catalogAssignmentsStatic: ProductAssignment[] = [];

  isEditing = false;
  isValidBuyerAssignment = false;
  areChanges = false;

  categories: Category[] = [];
  catalogs: UserGroup[] = [];

  constructor(
    private ocCategoryService: OcCategoryService,
    private ocUserGroupService: OcUserGroupService,
    private ocProductService: OcProductService
  ) {}

  resetIsAssignedStatus(value: boolean): void {
    this.isAssignedEditable = value;
    this.isAssignedStatic = value;
  }

  resetCatalogAssignments(catalogAssignments: ProductAssignment[]): void {
    this.catalogAssignmentsEditable = catalogAssignments;
    this.catalogAssignmentsStatic = catalogAssignments;
  }

  toggleProductCatalogAssignment(catalog: ProductAssignment): void {
    if (this.catalogAssignmentsEditable.some(c => c.UserGroupID === catalog.UserGroupID)) {
      this.catalogAssignmentsEditable = this.catalogAssignmentsEditable.filter(
        c => c.UserGroupID !== catalog.UserGroupID
      );
    } else {
      const newCatalogAssignment: ProductAssignment = { ProductID: this.product.ID, UserGroupID: catalog.UserGroupID };
      this.catalogAssignmentsEditable = [...this.catalogAssignmentsEditable, newCatalogAssignment];
    }
  }

  updateIsAssigned(event: any): void {
    this.isAssignedEditable = !this.isAssignedEditable;
    this.updateStatus();
  }

  updateStatus(): void {
    this.areChanges = this.isAssignedEditable !== this.isAssignedStatic;
  }

  edit(): void {
    this.isEditing = true;
    // this.getCategories();
    this.getCatalogs();
  }

  handleDiscardChanges(): void {
    this.catalogAssignmentsEditable = this.catalogAssignmentsStatic;
    this.isAssignedEditable = this.isAssignedStatic;
    this.isEditing = false;
  }

  // todo
  // async getCategories(): Promise<void> {
  //   const categoryResponse = await this.ocCategoryService
  //     .List(this._buyer.ID, { pageSize: 100, depth: 'All' })
  //     .toPromise();
  //   this.categories = categoryResponse.Items;
  // }

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
      this.ocProductService.ListAssignments({ userGroupID: c.ID, productID: this.product.ID }).toPromise()
    );
    const catalogAssignmentResponses = await Promise.all(catalogAssignmentRequests);
    const catalogAssignments = catalogAssignmentResponses.reduce((acc, curr) => acc.concat(curr), []);
    this.resetCatalogAssignments(catalogAssignments);
  }

  checkForProductCatalogAssignmentChanges() {
    this.add = this.catalogAssignmentsEditable.filter(
      assignment => !JSON.stringify(this.catalogAssignmentsStatic).includes(assignment.UserGroupID)
    );
    this.del = this.catalogAssignmentsStatic.filter(
      assignment => !JSON.stringify(this.catalogAssignmentsEditable).includes(assignment.UserGroupID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
  }
}
