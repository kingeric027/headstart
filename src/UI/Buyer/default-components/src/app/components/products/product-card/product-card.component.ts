import { Component, Input, OnInit, ViewEncapsulation, OnChanges, ChangeDetectorRef } from '@angular/core';
import { BuyerProduct } from '@ordercloud/angular-sdk';
import { find as _find, get as _get, map as _map, without as _without } from 'lodash';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class OCMProductCard {
  _isFavorite = false;
  _product: BuyerProduct = {
    PriceSchedule: {},
    xp: { Images: [] },
  };
  quantity: number;
  shouldDisplayAddToCart = false;
  isViewOnlyProduct = true;
  hasSpecs = false;

  constructor(private cdr: ChangeDetectorRef, private context: ShopperContextService) {}

  @Input() set product(value: BuyerProduct)  {
    this._product = value;
    this.isViewOnlyProduct = !value.PriceSchedule;
    this.hasSpecs = value.SpecCount > 0;
  }

  @Input() set isFavorite(value: boolean) {
    this._isFavorite = value;
    this.cdr.detectChanges(); // TODO - remove. Solve another way.
  }

  addToCart() {
    this.context.currentOrder.addToCart({ ProductID: this._product.ID, Quantity: this.quantity });
  }

  // TODO - we need a unified getImageUrl() function
  getImageUrl() {
    const host = 'https://s3.dualstack.us-east-1.amazonaws.com/staticcintas.eretailing.com/images/product';
    const images = this._product.xp.Images || [];
    const result = _map(images, img => {
      return img.Url.replace('{url}', host);
    });
    const filtered = _without(result, undefined);
    return filtered.length > 0 ? filtered[0] : 'http://placehold.it/300x300';
  }

  toDetails() {
    this.context.router.toProductDetails(this._product.ID);
  }

  setIsFavorite(isFavorite: boolean): void {
    this.context.currentUser.setIsFavoriteProduct(isFavorite, this._product.ID);
  }

  setQuantity(qty: number) {
    this.quantity = qty;
  }
}
