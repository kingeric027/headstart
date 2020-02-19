import { Component, OnInit, Input, OnChanges } from '@angular/core';
import {
  Buyer,
  OcBuyerService,
  ProductAssignment,
  ProductCatalogAssignment,
  OcCatalogService,
} from '@ordercloud/angular-sdk';
import { MarketPlaceProduct } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { ProductService } from '@app-seller/products/product.service';

@Component({
  selector: 'product-visibility-assignments-component',
  templateUrl: './product-visibility-assignments.component.html',
  styleUrls: ['./product-visibility-assignments.component.scss'],
})
export class ProductVisibilityAssignments implements OnInit, OnChanges {
  @Input()
  product: MarketPlaceProduct;
  buyers: Buyer[];
  add: ProductAssignment[];
  del: ProductAssignment[];
  _productCatalogAssignmentsStatic: ProductCatalogAssignment[];
  _productCatalogAssignmentsEditable: ProductCatalogAssignment[];
  areChanges = false;
  requestedUserConfirmation = false;
  faExclamationCircle = faExclamationCircle;

  constructor(
    private ocBuyerService: OcBuyerService,
    private ocCatalogService: OcCatalogService,
    private productService: ProductService
  ) {}

  async ngOnInit() {
    this.getBuyers();
    this.getProductCatalogAssignments(this.product);
  }

  ngOnChanges() {
    this.getProductCatalogAssignments(this.product);
  }

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async getBuyers(): Promise<void> {
    const buyers = await this.ocBuyerService.List().toPromise();
    this.buyers = buyers.Items;
  }

  async getProductCatalogAssignments(product: MarketPlaceProduct): Promise<void> {
    const productCatalogAssignments = await this.ocCatalogService
      .ListProductAssignments({ productID: product && product.ID })
      .toPromise();
    this._productCatalogAssignmentsStatic = productCatalogAssignments.Items;
    this._productCatalogAssignmentsEditable = productCatalogAssignments.Items;
  }

  toggleProductCatalogAssignment(buyer: Buyer) {
    if (this.isAssigned(buyer)) {
      this._productCatalogAssignmentsEditable = this._productCatalogAssignmentsEditable.filter(
        productAssignment => productAssignment.CatalogID !== buyer.DefaultCatalogID
      );
    } else {
      const newProductCatalogAssignment = {
        CatalogID: buyer.DefaultCatalogID,
        ProductID: this.product.ID,
      };
      this._productCatalogAssignmentsEditable = [
        ...this._productCatalogAssignmentsEditable,
        newProductCatalogAssignment,
      ];
    }
    this.checkForProductCatalogAssignmentChanges();
  }

  isAssigned(buyer: Buyer) {
    return (
      this._productCatalogAssignmentsEditable &&
      this._productCatalogAssignmentsEditable.some(
        productAssignment => productAssignment.CatalogID === buyer.DefaultCatalogID
      )
    );
  }

  checkForProductCatalogAssignmentChanges() {
    this.add = this._productCatalogAssignmentsEditable.filter(
      assignment => !JSON.stringify(this._productCatalogAssignmentsStatic).includes(assignment.CatalogID)
    );
    this.del = this._productCatalogAssignmentsStatic.filter(
      assignment => !JSON.stringify(this._productCatalogAssignmentsEditable).includes(assignment.CatalogID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
  }

  discardProductCatalogAssignmentChanges() {
    this._productCatalogAssignmentsEditable = this._productCatalogAssignmentsStatic;
    this.checkForProductCatalogAssignmentChanges();
  }

  async executeProductCatalogAssignmentRequests(): Promise<void> {
    this.requestedUserConfirmation = false;
    await this.productService.updateProductCatalogAssignments(this.add, this.del);
    await this.getProductCatalogAssignments(this.product);
    this.checkForProductCatalogAssignmentChanges();
  }
}
