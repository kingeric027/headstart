import { Component, OnInit, Input, OnChanges } from '@angular/core';
import {
  Buyer,
  OcBuyerService,
  ProductAssignment,
  ProductCatalogAssignment,
  OcCatalogService,
} from '@ordercloud/angular-sdk';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { ProductService } from '@app-seller/products/product.service';
import { MarketplaceProduct, MarketplaceKitProduct } from '@ordercloud/headstart-sdk';
import { MiddlewareKitService } from '@app-seller/shared/services/middleware-api/middleware-kit.service';

@Component({
  selector: 'product-visibility-assignments-component',
  templateUrl: './product-visibility-assignments.component.html',
  styleUrls: ['./product-visibility-assignments.component.scss'],
})
export class ProductVisibilityAssignments implements OnInit, OnChanges {
  @Input()
  product: MarketplaceProduct;
  buyers: Buyer[];
  add: ProductAssignment[];
  del: ProductAssignment[];
  _productCatalogAssignmentsStatic: ProductCatalogAssignment[];
  _productCatalogAssignmentsEditable: ProductCatalogAssignment[];
  kitProductCatalogAssignments: ProductCatalogAssignment[];
  areChanges = false;
  requestedUserConfirmation = false;
  faExclamationCircle = faExclamationCircle;

  constructor(
    private ocBuyerService: OcBuyerService,
    private ocCatalogService: OcCatalogService,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    this.getBuyers();
    this.getProductCatalogAssignments(this.product);
  }

  ngOnChanges(): void {
    this.getProductCatalogAssignments(this.product);
  }

  requestUserConfirmation(): void {
    this.requestedUserConfirmation = true;
  }

  async getBuyers(): Promise<void> {
    const buyers = await this.ocBuyerService.List().toPromise();
    this.buyers = buyers.Items;
  }



  async getProductCatalogAssignments(product: MarketplaceProduct): Promise<void> {
    const productCatalogAssignments = await this.ocCatalogService
      .ListProductAssignments({ productID: product && product.ID })
      .toPromise();
    this._productCatalogAssignmentsStatic = productCatalogAssignments.Items;
    this._productCatalogAssignmentsEditable = productCatalogAssignments.Items;
  }

  isAssigned(buyer: Buyer): boolean {
    return (
      this._productCatalogAssignmentsEditable &&
      this._productCatalogAssignmentsEditable.some(
        productAssignment => productAssignment.CatalogID === buyer.DefaultCatalogID
      )
    );
  }

  checkForProductCatalogAssignmentChanges(): void {
    this.add = this._productCatalogAssignmentsEditable.filter(
      assignment => !JSON.stringify(this._productCatalogAssignmentsStatic).includes(assignment.CatalogID) ||
        !JSON.stringify(this.kitProductCatalogAssignments).includes(assignment.CatalogID)
    );
    debugger;
    this.del = this._productCatalogAssignmentsStatic.filter(
      assignment => !JSON.stringify(this._productCatalogAssignmentsEditable).includes(assignment.CatalogID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
  }

  discardProductCatalogAssignmentChanges(): void {
    this._productCatalogAssignmentsEditable = this._productCatalogAssignmentsStatic;
    this.checkForProductCatalogAssignmentChanges();
  }
}
