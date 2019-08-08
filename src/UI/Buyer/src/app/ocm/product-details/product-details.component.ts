import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectorRef, AfterViewChecked } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { BuyerProduct, BuyerSpec, LineItem } from '@ordercloud/angular-sdk';
import { find as _find, difference as _difference, minBy as _minBy, has as _has } from 'lodash';
import { FullSpecOption } from '@app-buyer/shared/models/full-spec-option.interface';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
@Component({
  selector: 'product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class OCMProductDetails implements OnInit, AfterViewChecked {
  @Input() specs: BuyerSpec[] = [];
  @Input() product: BuyerProduct;
  @Input() isFavorite: boolean;
  @Input() quantityLimits: QuantityLimits;
  @Output() addToCartEvent = new EventEmitter<LineItem>();
  @Output() setIsFavorite = new EventEmitter<boolean>();

  quantity: number;
  quantityInputReady = false;
  specSelections: FullSpecOption[] = [];
  relatedProducts$: Observable<BuyerProduct[]>;
  imageUrls: string[] = [];

  constructor(private router: Router, private changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit() {}

  routeToProductList(): void {
    this.router.navigate(['/products']);
  }

  toDetailsPage(productId: string) {
    this.router.navigate([`/products/${productId}`]);
  }

  addToCart(): void {
    const Specs = this.specSelections.map((o) => ({
      SpecID: o.SpecID,
      OptionID: o.ID,
      Value: o.Value,
    }));
    this.addToCartEvent.emit({
      ProductID: this.product.ID,
      Quantity: this.quantity,
      Specs,
    });
  }

  isOrderable(): boolean {
    // products without a price schedule are view-only.
    return !!this.product.PriceSchedule;
  }

  hasPrice(): boolean {
    // free products dont need to display a price.
    return _has(this.product, 'PriceSchedule.PriceBreaks[0].Price');
  }

  getTotalPrice(): number {
    // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
    // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
    // is the cost per item.
    if (!this.quantity) {
      return null;
    }
    if (!this.hasPrice()) {
      return 0;
    }
    const priceBreaks = this.product.PriceSchedule.PriceBreaks;
    const startingBreak = _minBy(priceBreaks, 'Quantity');

    const selectedBreak = priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity && candidate.Quantity <= this.quantity ? candidate : current;
    }, startingBreak);
    const markup = this.totalSpecMarkup(selectedBreak.Price, this.quantity);

    return (selectedBreak.Price + markup) * this.quantity;
  }

  totalSpecMarkup(unitPrice: number, quantity: number): number {
    const markups = this.specSelections.map((s) => this.singleSpecMarkup(unitPrice, quantity, s));
    return markups.reduce((x, acc) => x + acc, 0); //sum
  }

  singleSpecMarkup(unitPrice: number, quantity: number, spec: FullSpecOption): number {
    switch (spec.PriceMarkupType) {
      case 'NoMarkup':
        return 0;
      case 'AmountPerQuantity':
        return spec.PriceMarkup;
      case 'AmountTotal':
        return spec.PriceMarkup / quantity;
      case 'Percentage':
        return spec.PriceMarkup * unitPrice * 0.01;
    }
  }

  getImageUrls() {
    const images = this.product.xp.Images || [];
    return images.map((i) => i.Url);
  }

  ngAfterViewChecked() {
    // This manually triggers angular's change detection cycle and avoids the imfamous
    // "Expression has changed after it was checked" error.
    // Caused by something in spec form
    this.changeDetectorRef.detectChanges();
  }
}
