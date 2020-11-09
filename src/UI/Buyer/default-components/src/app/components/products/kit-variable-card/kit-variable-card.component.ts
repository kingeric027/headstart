import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { faCaretDown, faCaretRight, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { MarketplaceMeKitProduct, MeProductInKit } from '@ordercloud/headstart-sdk';
import { ShopperContextService } from 'marketplace';
import { LineItemToAdd } from 'src/app/models/line-item-to-add.interface';
import { ProductSelectionEvent } from 'src/app/models/product-selection-event.interface';

@Component({
  // ocm-kit-variable-card
  templateUrl: './kit-variable-card.component.html',
  styleUrls: ['./kit-variable-card.component.scss']
})
export class OCMKitVariableCard {
  @Input() set productKitDetails(value: MeProductInKit) {
    this._productKitDetails = value;
    this.onInit();
  }
  @Input() set allLineItems(value: LineItemToAdd[]) {
    this._allLineItems = value;
    this.getLineItemTotals()
  }
  @Input() set kitProduct(value: MarketplaceMeKitProduct) {
    this._kitProduct = value;
    this.getLineItemTotals();
  }
  @Output() selectProduct = new EventEmitter<ProductSelectionEvent>();
  @Output() removeLineItem = new EventEmitter<LineItemToAdd>();
  _allLineItems: LineItemToAdd[];
  _kitProduct: MarketplaceMeKitProduct;

  faTrashAlt = faTrashAlt
  variantLineItemsTotalQuantity: number;
  variantLineItemsTotalPrice: number;
  variantLineItems: LineItemToAdd[] = [];
  faCaretDown = faCaretDown;
  faCaretRight = faCaretRight;
  panelActiveIDs: string[];
  imageUrl: string;
  userCurrency: string;
  _productKitDetails: MeProductInKit;

  constructor(
    private context: ShopperContextService,
  ) { }

  onInit(): void {
    const appSettings = this.context.appSettings;
    this.panelActiveIDs = [this._productKitDetails.Product.ID]; // expand panel by default
    this.userCurrency = this.context.currentUser.get().Currency;
    this.imageUrl = this._productKitDetails.Images &&
      this._productKitDetails.Images.length ?
      `${appSettings.middlewareUrl}/assets/${appSettings.sellerID}/products/${this._productKitDetails.Product.ID}/thumbnail?size=S` :
      'http://placehold.it/60x60';
  }

  getLineItemTotals(): void {
    if (!this._kitProduct || !this._allLineItems) {
      return;
    }
    this.variantLineItems = this._allLineItems.filter(li => {
      return li.ProductID === this._productKitDetails.Product.ID && li.xp.KitProductID === this._kitProduct.ID;
    })
    this.variantLineItemsTotalPrice = this.variantLineItems.map(li => li.Price).reduce((a, b) => a + b, 0);
    this.variantLineItemsTotalQuantity = this.variantLineItems.map(li => li.Quantity).reduce((a, b) => a + b, 0);
  }

  selectVariant(): void {
    this.selectProduct.emit({
      productKitDetails: this._productKitDetails,
      variantSelection: {
        productID: this._productKitDetails.Product.ID,
        quantity: 0,
        specForm: {} as FormGroup
      }
    })
  }

  removeVariantLineItem(lineItem: LineItemToAdd): void {
    this.removeLineItem.emit(lineItem);
  }

}
