import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { Buyer, OcBuyerService, ProductAssignment, OcProductService } from '@ordercloud/angular-sdk';
import { MarketPlaceProduct } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';

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
  _productPartyPriceScheduleAssignmentsStatic: ProductAssignment[];
  _productPartyPriceScheduleAssignmentsEditable: ProductAssignment[];
  areChanges = false;
  requestedUserConfirmation: boolean = false;
  faExclamationCircle = faExclamationCircle;

  constructor(
    private ocBuyerService: OcBuyerService,
    private ocProductService: OcProductService,
    private productService: ProductService
  ) {}

  async ngOnInit() {
    this.getBuyers();
    this.getProductPartyPriceScheduleAssignments(this.product);
  }

  ngOnChanges() {
    this.getProductPartyPriceScheduleAssignments(this.product);
  }

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async getBuyers(): Promise<void> {
    const buyers = await this.ocBuyerService.List().toPromise();
    this.buyers = buyers.Items;
  }

  async getProductPartyPriceScheduleAssignments(product: MarketPlaceProduct): Promise<void> {
    const productPartyPriceScheduleAssignments = await this.ocProductService
      .ListAssignments({ productID: product && product.ID })
      .toPromise();
    this._productPartyPriceScheduleAssignmentsStatic = productPartyPriceScheduleAssignments.Items;
    this._productPartyPriceScheduleAssignmentsEditable = productPartyPriceScheduleAssignments.Items;
    console.log(this._productPartyPriceScheduleAssignmentsEditable);
  }

  toggleProductPartyPriceScheduleAssignment(buyer: Buyer) {
    if (this.isAssigned(buyer)) {
      this._productPartyPriceScheduleAssignmentsEditable = this._productPartyPriceScheduleAssignmentsEditable.filter(
        productAssignemnt => productAssignemnt.BuyerID !== buyer.ID
      );
    } else {
      const newProductPartyPriceScheduleAssignment = {
        ProductID: this.product.ID,
        BuyerID: buyer.ID,
        UserID: null,
        UserGroupID: null,
        PriceScheduleID: this.product.DefaultPriceScheduleID,
      };
      this._productPartyPriceScheduleAssignmentsEditable = [
        ...this._productPartyPriceScheduleAssignmentsEditable,
        newProductPartyPriceScheduleAssignment,
      ];
    }
    this.checkForProductPartyPriceScheduleAssignmentChanges();
  }

  isAssigned(buyer: Buyer) {
    return (
      this._productPartyPriceScheduleAssignmentsEditable &&
      this._productPartyPriceScheduleAssignmentsEditable.some(
        productAssignment => productAssignment.BuyerID === buyer.ID
      )
    );
  }

  checkForProductPartyPriceScheduleAssignmentChanges() {
    console.log('Editable', this._productPartyPriceScheduleAssignmentsEditable);
    console.log('Static', this._productPartyPriceScheduleAssignmentsStatic);
    this.add = this._productPartyPriceScheduleAssignmentsEditable.filter(
      assignment => !JSON.stringify(this._productPartyPriceScheduleAssignmentsStatic).includes(assignment.BuyerID)
    );
    this.del = this._productPartyPriceScheduleAssignmentsStatic.filter(
      assignment => !JSON.stringify(this._productPartyPriceScheduleAssignmentsEditable).includes(assignment.BuyerID)
    );
    console.log('To Add', this.add);
    console.log('To Del', this.del);
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
  }

  discardProductPartyPriceScheduleAssignmentChanges() {
    this._productPartyPriceScheduleAssignmentsEditable = this._productPartyPriceScheduleAssignmentsStatic;
    this.checkForProductPartyPriceScheduleAssignmentChanges();
  }

  async executeProductPartyPriceScheduleAssignmentRequests(): Promise<void> {
    this.requestedUserConfirmation = false;
    await this.productService.updateProductPartyPriceScheduleAssignments(this.add, this.del);
    await this.productService.updateProductPartyPriceScheduleAssignments(this.add, this.del);
    await this.getProductPartyPriceScheduleAssignments(this.product);
    this.checkForProductPartyPriceScheduleAssignmentChanges();
  }
}
