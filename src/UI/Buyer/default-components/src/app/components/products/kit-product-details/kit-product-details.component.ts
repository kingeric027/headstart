import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { LineItemSpec } from 'ordercloud-javascript-sdk';
import { MarketplaceMeKitProduct, MeProductInKit } from '@ordercloud/headstart-sdk';
import { ProductDetailService } from '../product-details/product-detail.service';
import { ShopperContextService } from 'marketplace';

export interface KitVariantSelection {
  productID: string;
  specForm: FormGroup;
  quantity: number;
}

export interface OpenVariantSelectionEvent {
  productKitDetails: MeProductInKit;
  selection: KitVariantSelection;
}

export interface LineItemToAdd {
  ProductID: string;
  Quantity: number;
  Specs: LineItemSpec[];
  Product: {
    // adding purely so i can use productNameWithSpecs pipe without modification
    Name: string;
  };
  Price: number; // adding for display purposes
  xp: {
    ImageUrl: string;
    KitProductName: string;
    KitProductImageUrl: string;
    KitProductID: string;
  };
}
@Component({
  templateUrl: './kit-product-details.component.html',
  styleUrls: ['./kit-product-details.component.scss']
})
export class OCMKitProductDetails {
  isAddingToCart = false;
  _product: MarketplaceMeKitProduct;
  variantSelection: KitVariantSelection;
  openVariantSelectionEvent: OpenVariantSelectionEvent;
  lineItemsToAdd: LineItemToAdd[] = [];

  @Input() set product(product: MarketplaceMeKitProduct) {
    this._product = product
  }

  constructor(
    private productDetailService: ProductDetailService,
    private context: ShopperContextService
  ) { }

  openVariantSelection(event: OpenVariantSelectionEvent): void {
    this.openVariantSelectionEvent = event;
  }
  async addToCart(): Promise<void> {
    this.isAddingToCart = true;
    try {
      await this.context.order.cart.addMany(this.lineItemsToAdd);
    } finally {
      this.isAddingToCart = false;
    }
  }

  addLineItem(newline: LineItemToAdd): void {
    const matchingIndex = this.lineItemsToAdd.findIndex(li => this.productDetailService.isSameLine(li, newline));
    if (matchingIndex > -1) {
      // line item exists, replace it
      this.lineItemsToAdd = this.lineItemsToAdd.map((li, index) => index === matchingIndex ? newline : li)
    } else {
      // line item doesnt exist, add it to array
      this.lineItemsToAdd = [...this.lineItemsToAdd, newline]
    }
  }

  removeLineItem(lineToRemove: LineItemToAdd): void {
    this.lineItemsToAdd = this.lineItemsToAdd.filter(li => !this.productDetailService.isSameLine(li, lineToRemove))
  }

  canAddToCart(): boolean {
    // the cart is valid if all kit products have at least one associated line item
    // variable kit products may have more than one
    if (!this._product || !this._product.ProductAssignments.ProductsInKit.length || !this.lineItemsToAdd?.length) {
      return false;
    }
    const productsAddedToCart = this.lineItemsToAdd.map(li => li.ProductID);
    return this._product.ProductAssignments.ProductsInKit.every(details => details.Optional || productsAddedToCart.includes(details.Product.ID))
  }

}
