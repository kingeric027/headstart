import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { BuyerProduct, ListSpec } from '@ordercloud/angular-sdk';
import {
  map as _map,
  without as _without,
  uniqBy as _uniq,
  some as _some,
  find as _find,
  difference as _difference,
  minBy as _minBy,
  has as _has,
} from 'lodash';
import { OCMComponent } from '../base-component';
import { SpecFormService } from '../spec-form/spec-form.service';

@Component({
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class OCMProductDetails extends OCMComponent {
  @Input() specs: ListSpec;
  @Input() product: BuyerProduct;

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

  constructor(private formService: SpecFormService) {
    super();
    this.specFormService = formService;
  }

  ngOnContextSet() {
    this.isOrderable = !!this.product.PriceSchedule;
    this.imageUrls = this.getImageUrls();
    this.context.currentUser.onFavoriteProductsChange(productIDs => (this.favoriteProducts = productIDs));
    this.specFormService.event.valid = this.specs.Items.length === 0;
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
      ProductID: this.product.ID,
      Quantity: this.quantity,
      Specs: this.specFormService.getLineItemSpecs(this.specs),
    });
  }

  getPriceBreakRange(index) {
    if (!this.product.PriceSchedule && !this.product.PriceSchedule.PriceBreaks.length) {
      return '';
    }
    const priceBreaks = this.product.PriceSchedule.PriceBreaks;
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
    if (!this.product.PriceSchedule && !this.product.PriceSchedule.PriceBreaks.length) {
      return 0;
    }
    const priceBreaks = this.product.PriceSchedule.PriceBreaks;
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
      ? this.specFormService.getSpecMarkup(this.specs, selectedBreak, this.quantity || startingBreak.Quantity)
      : selectedBreak.Price * (this.quantity || startingBreak.Quantity);
  }

  // TODO - we need a unified getImageUrl() function
  getImageUrls(): string[] {
    const images =
      _uniq(this.product.xp.Images, (img: any) => {
        return img.Url;
      }) || [];
    const result = _map(images, img => {
      return img.Url.replace('{url}', this.context.appSettings.cmsUrl);
    });
    return _without(result, undefined) as string[];
  }

  isFavorite(): boolean {
    return this.favoriteProducts.includes(this.product.ID);
  }

  setIsFavorite(isFav: boolean) {
    this.context.currentUser.setIsFavoriteProduct(isFav, this.product.ID);
  }
}
