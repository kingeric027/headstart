import { Component, Input, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { Observable } from 'rxjs';
import { BuyerProduct, ListSpec } from '@ordercloud/angular-sdk';
import { map as _map, without as _without, uniqBy as _uniq, some as _some,
  find as _find, difference as _difference, minBy as _minBy, has as _has } from 'lodash';
import { OCMComponent } from '../base-component';
import { QuantityLimits } from '../../models/quantity-limits';
import { SpecFormService } from '../spec-form/spec-form.service';

@Component({
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class OCMProductDetails extends OCMComponent implements AfterViewChecked {
  @Input() specs: ListSpec;
  @Input() product: BuyerProduct;
  @Input() quantityLimits: QuantityLimits;

  specFormService: SpecFormService;
  isOrderable = false;
  quantity: number;
  price: number;
  relatedProducts$: Observable<BuyerProduct[]>;
  imageUrls: string[] = [];
  favoriteProducts: string[] = [];

  constructor(private changeDetectorRef: ChangeDetectorRef, private formService: SpecFormService) {
    super();
    this.specFormService = formService;
  }

  ngOnContextSet() {
    this.isOrderable = !!this.product.PriceSchedule;
    this.imageUrls = this.getImageUrls();
    this.context.currentUser.onFavoriteProductsChange((productIDs) => (this.favoriteProducts = productIDs));
    this.specFormService.event.valid = this.specs.Items.length === 0;
  }

  onSpecFormChange(event): void {
    if (event.detail.type === 'Change') {
      this.specFormService.event = event.detail;
      this.getTotalPrice();
    }
  }

  qtyChange(event): void {
    this.quantity = event.detail;
    this.getTotalPrice();
  }

  addToCart(event: any): void {
    this.context.currentOrder.addToCart({
      ProductID: this.product.ID,
      Quantity: this.quantity,
      Specs: this.specFormService.getLineItemSpecs(this.specs)
    });
  }

  getTotalPrice() {
    // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
    // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
    // is the cost per item.
    if (
      !this.product.PriceSchedule &&
      !this.product.PriceSchedule.PriceBreaks.length
    ) {
      return 0;
    }
    const priceBreaks = this.product.PriceSchedule.PriceBreaks;
    const startingBreak = _minBy(priceBreaks, 'Quantity');

    const selectedBreak = priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity && candidate.Quantity <= this.quantity
        ? candidate
        : current;
    }, startingBreak);
    this.price = this.specFormService.event.valid
      ? this.specFormService.getSpecMarkup(this.specs, selectedBreak, this.quantity || startingBreak.Quantity)
      : selectedBreak.Price * (this.quantity || startingBreak.Quantity);
  }

  getImageUrls(): string[] {
    const images =
      _uniq(this.product.xp.Images, (img: any) => {
        return img.Url;
      }) || [];
    const result = _map(images, (img) => {
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

  ngAfterViewChecked() {
    // This manually triggers angular's change detection cycle and avoids the imfamous
    // "Expression has changed after it was checked" error.
    // Caused by something in spec form
    this.changeDetectorRef.detectChanges();
  }
}
