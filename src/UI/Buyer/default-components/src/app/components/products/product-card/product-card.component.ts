import { Component, Input, OnInit, ViewEncapsulation, OnChanges, ChangeDetectorRef } from '@angular/core';
import { BuyerProduct } from '@ordercloud/angular-sdk';
import { find as _find, get as _get, map as _map, without as _without } from 'lodash';
import { OCMComponent } from '../../base-component';

@Component({
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class OCMProductCard extends OCMComponent {
  @Input() product: BuyerProduct = {
    PriceSchedule: {},
    xp: { Images: [] },
  };
  @Input() set isFavorite(value: boolean) {
    this.isFav = value;
    this.cdr.detectChanges(); // TODO - remove. Solve another way.
  }

  isFav = false;
  quantity: number;
  shouldDisplayAddToCart: boolean;
  isViewOnlyProduct: boolean;
  hasSpecs: boolean;

  constructor(private cdr: ChangeDetectorRef) {
    super();
  }

  ngOnContextSet() {
    this.isViewOnlyProduct = !this.product.PriceSchedule;
    this.hasSpecs = this.product.SpecCount > 0;
  }

  addToCart() {
    this.context.currentOrder.addToCart({ ProductID: this.product.ID, Quantity: this.quantity });
  }

  // TODO - we need a unified getImageUrl() function
  getImageUrl() {
    const host = 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product';
    const images = this.product.xp.Images || [];
    const result = _map(images, img => {
      return img.Url.replace('{url}', host);
    });
    const filtered = _without(result, undefined);
    return filtered.length > 0 ? filtered[0] : 'http://placehold.it/300x300';
  }

  toDetails() {
    this.context.router.toProductDetails(this.product.ID);
  }

  setIsFavorite(isFavorite: boolean): void {
    this.context.currentUser.setIsFavoriteProduct(isFavorite, this.product.ID);
  }

  setQuantity(qty: number) {
    this.quantity = qty;
  }
}
