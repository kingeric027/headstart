import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BuyerProduct, ListSpec } from '@ordercloud/angular-sdk';
import {
  map as _map,
  without as _without,
  some as _some,
  find as _find,
  difference as _difference,
  minBy as _minBy,
  has as _has,
} from 'lodash';
import { SpecFormService } from '../spec-form/spec-form.service';
import { ShopperContextService } from 'marketplace';
import { getImageUrls } from 'src/app/services/images.helpers';

@Component({
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class OCMProductDetails implements OnInit {
  _specs: ListSpec;
  _product: BuyerProduct;
  specFormService: SpecFormService;
  isOrderable = false;
  quantity: number;
  price: number;
  percentSavings: number;
  priceBreaks: object;
  priceBreakRange: string[];
  selectedBreak: object;
  relatedProducts$: Observable<BuyerProduct[]>;
  imageUrls: string[] = [];
  favoriteProducts: string[] = [];
  qtyValid = true;
  supplierNote: string;
  specLength: number;

  constructor(private formService: SpecFormService, private context: ShopperContextService) {
    this.specFormService = formService;
  }

  @Input() set specs(value: ListSpec) {
    this._specs = value;
    this.specFormService.event.valid = this._specs.Items.length === 0;
    this.specLength = this._specs.Items.length;
  }

  @Input() set product(value: BuyerProduct) {
    this._product = value;
    this.isOrderable = !!this._product.PriceSchedule;
    this.imageUrls = this.getImageUrls();
    this.supplierNote = this._product.xp.Note;
  }

  ngOnInit() {
    this.context.currentUser.onFavoriteProductsChange(productIDs => (this.favoriteProducts = productIDs));
  }

  onSpecFormChange(event): void {
    if (event.detail.type === 'Change') {
      this.specFormService.event = event.detail;
      this.getTotalPrice();
    }
  }

  qtyChange(event: { qty: number; valid: boolean }): void {
    this.qtyValid = event.valid;
    if (this.qtyValid) {
      this.quantity = event.qty;
      this.getTotalPrice();
    }
  }

  addToCart(event: any): void {
    this.context.currentOrder.addToCart({
      ProductID: this._product.ID,
      Quantity: this.quantity,
      Specs: this.specFormService.getLineItemSpecs(this._specs),
    });
  }

  getPriceBreakRange(index) {
    if (!this._product.PriceSchedule && !this._product.PriceSchedule.PriceBreaks.length) {
      return '';
    }
    const priceBreaks = this._product.PriceSchedule.PriceBreaks;
    const indexOfNextPriceBreak = index + 1;
    if (indexOfNextPriceBreak < priceBreaks.length) {
      return `${priceBreaks[index].Quantity} - ${priceBreaks[indexOfNextPriceBreak].Quantity - 1}`;
    } else {
      return `${priceBreaks[index].Quantity}+`;
    }
  }

  getTotalPrice() {
    // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
    // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
    // is the cost per item.
    if (!this._product.PriceSchedule && !this._product.PriceSchedule.PriceBreaks.length) {
      return 0;
    }
    const priceBreaks = this._product.PriceSchedule.PriceBreaks;
    this.priceBreaks = priceBreaks;
    const startingBreak = _minBy(priceBreaks, 'Quantity');
    const selectedBreak = priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity && candidate.Quantity <= this.quantity ? candidate : current;
    }, startingBreak);
    this.selectedBreak = selectedBreak;
    this.percentSavings = parseInt(
      (((priceBreaks[0].Price - selectedBreak.Price) / priceBreaks[0].Price) * 100).toFixed(0)
    );
    this.price = this.specFormService.event.valid
      ? this.specFormService.getSpecMarkup(this._specs, selectedBreak, this.quantity || startingBreak.Quantity)
      : selectedBreak.Price * (this.quantity || startingBreak.Quantity);
  }

  getImageUrls(): string[] {
    return getImageUrls(this._product);
  }

  isFavorite(): boolean {
    return this.favoriteProducts.includes(this._product.ID);
  }

  setIsFavorite(isFav: boolean) {
    this.context.currentUser.setIsFavoriteProduct(isFav, this._product.ID);
  }

  setActiveSupplier(supplierId: string) {
    this.context.router.toProductList({ activeFacets: { Supplier: supplierId.toLowerCase() } });
  }
}
