import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MarketplaceMeKitProduct, MeProductInKit, Variant } from '@ordercloud/headstart-sdk';
import { KitVariantSelection, LineItemToAdd, OpenVariantSelectionEvent } from '../kit-product-details/kit-product-details.component';
import { ProductDetailService } from '../product-details/product-detail.service';
import { QtyChangeEvent } from '../quantity-input/quantity-input.component';
import { SpecFormEvent } from '../spec-form/spec-form-values.interface';
import { SpecFormService } from '../spec-form/spec-form.service';

@Component({
  // ocm-kit-variant-selector
  templateUrl: './kit-variant-selector.component.html',
  styleUrls: ['./kit-variant-selector.component.scss']
})
export class OCMKitVariantSelector {
  @Input() set event(value: OpenVariantSelectionEvent) {
    this._event = value;
    this.onInit();
  }
  @Input() set allLineItems(value: LineItemToAdd[]) {
    this._allLineItems = value;
  }
  @Input() kitProduct: MarketplaceMeKitProduct;
  @Output() addLineItem = new EventEmitter<LineItemToAdd>();
  _allLineItems: LineItemToAdd[];
  _event: OpenVariantSelectionEvent;
  productKitDetails: MeProductInKit;
  selection: KitVariantSelection;
  disabledVariants: Variant[]
  specForm = {} as FormGroup;
  price = 0;
  quantity = 0;
  quantityValid: boolean;
  errorMessage: string;
  resetFormToggle = true;

  constructor(
    private productDetailService: ProductDetailService,
    private specFormService: SpecFormService
  ) { }

  onInit(): void {
    this.productKitDetails = this._event.productKitDetails;
    this.selection = this._event.selection;
    this.disabledVariants = this.productKitDetails.Variants.filter(v => !v.Active)
    this.setKitMinAndMax();
  }

  onSpecFormChange(event: SpecFormEvent): void {
    this.specForm = event.form;
    this.price = this.getPrice();
  }

  qtyChange({ qty, valid }: QtyChangeEvent): void {
    this.quantityValid = valid;
    if (valid) {
      this.errorMessage = '';
      this.quantity = qty
      this.price = this.getPrice();
    } else {
      const maxQty = this.productKitDetails.Product.PriceSchedule.MaxQuantity;
      const minQty = this.productKitDetails.Product.PriceSchedule.MinQuantity;
      if (qty > maxQty) {
        this.errorMessage = `Quantity must not exceed ${maxQty}`
      } else if (qty < minQty) {
        this.errorMessage = `Quantity must not be less than ${minQty}`
      }
    }
  }

  getPrice(): number {
    return this.productDetailService.getProductPrice(
      this.productKitDetails.Product.PriceSchedule?.PriceBreaks ?? [],
      this.productKitDetails.Specs,
      this.quantity,
      this.specForm,
    );
  }

  addVariantSelection(): void {
    const lineItemToAdd: LineItemToAdd = this.buildLineItem();
    if (
      this.productDetailService.isSameLineAsOthers(lineItemToAdd, this._allLineItems) &&
      !window.confirm('You have an existing selection with the same options. Adding this selection will overwrite your existing selection, would you like to proceed?')
    ) {
      return;
    }

    this.addLineItem.emit(lineItemToAdd)

    // dirty hack to reset quantity input and ocm-spec-form
    this.price = 0;
    this.quantity = 0;
    this.resetFormToggle = false;
    window.setTimeout(() => {
      this.resetFormToggle = true;
    })
  }

  buildLineItem(): LineItemToAdd {
    return {
      ProductID: this.productKitDetails.Product.ID,
      Product: {
        Name: this.productKitDetails.Product.Name
      },
      Price: this.price,
      Quantity: this.quantity,
      Specs: this.specFormService.getLineItemSpecs(this.productKitDetails.Specs, this.specForm),
      xp: {
        ImageUrl: this.specFormService.getLineItemImageUrl(this.productKitDetails.Images, this.productKitDetails.Specs, this.specForm) || 'http://placehold.it/60x60',
        KitProductName: this.kitProduct.Name, // TODO remove this once i rebuild package with types
        KitProductID: this.kitProduct.ID,
        KitProductImageUrl: this.kitProduct.Images && this.kitProduct.Images.length
          ? this.kitProduct.Images[0].Url
          : 'http://placehold.it/60x60'
      }
    }
  }

  setKitMinAndMax(): void {
    // use min/max defined on the kit which can be more restrictive than what's set at the product level
    const isOrderable = this.productKitDetails.Product.PriceSchedule;
    if (isOrderable) {
      this.productKitDetails.Product.PriceSchedule.MinQuantity = this.productKitDetails.MinQty;
      this.productKitDetails.Product.PriceSchedule.MaxQuantity = this.productKitDetails.MaxQty;
    }
  }
}
