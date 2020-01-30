import { Component, Input, OnInit, ViewEncapsulation, OnChanges, ChangeDetectorRef } from '@angular/core';
import { BuyerProduct } from '@ordercloud/angular-sdk';
import { find as _find, get as _get, map as _map, without as _without } from 'lodash';
import { ShopperContextService } from 'marketplace';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';

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

  getImageUrl() {
    return getPrimaryImageUrl(this._product);
  }

  toDetails() {
    this.context.router.toProductDetails(this._product.ID);
  }

  setIsFavorite(isFavorite: boolean): void {
    this.context.currentUser.setIsFavoriteProduct(isFavorite, this._product.ID);
  }

  setQuantity(event: any) {
    this.quantity = event.qty;
  }
}
