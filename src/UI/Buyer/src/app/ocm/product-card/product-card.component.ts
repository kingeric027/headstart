import {
  Component,
  Input,
  EventEmitter,
  Output,
  OnInit,
  ViewEncapsulation,
  OnChanges,
} from '@angular/core';
import { BuyerProduct, LineItem } from '@ordercloud/angular-sdk';
import { Router } from '@angular/router';
import {
  find as _find,
  get as _get,
  map as _map,
  without as _without,
} from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { ocAppConfig } from '@app-buyer/config/app.config';

@Component({
  selector: 'product-card',
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class OCMProductCard implements OnInit, OnChanges {
  @Input() product: BuyerProduct = {
    PriceSchedule: {},
    xp: { Images: [] },
  };
  @Input() isFavorite: boolean;
  @Input() quantityLimits: QuantityLimits = {
    inventory: 0,
    maxPerOrder: 0,
    minPerOrder: 0,
    restrictedQuantities: [],
  };
  @Output() addedToCart = new EventEmitter<LineItem>();
  @Output() setIsFavorite = new EventEmitter<boolean>();

  quantity: number;
  shouldDisplayAddToCart: boolean;
  isViewOnlyProduct: boolean;
  isSetFavoriteUsed: boolean;
  hasSpecs: boolean;
  featuredProducts: boolean;

  constructor(private router: Router) {}

  ngOnChanges() {
    this.isSetFavoriteUsed = this.setIsFavorite.observers.length > 0;
    const isAddedToCartUsed = this.addedToCart.observers.length > 0;
    this.isViewOnlyProduct = !this.product.PriceSchedule;
    this.hasSpecs = this.product.SpecCount > 0;
    this.shouldDisplayAddToCart =
      isAddedToCartUsed && !this.isViewOnlyProduct && !this.hasSpecs;
    this.featuredProducts = this.router.url.indexOf('/home') > -1;
  }
  ngOnInit() {
    /**
     * this will be true if the parent component
     * is wired up to listen to the outputted event
     */
  }

  addToCart(li: LineItem) {
    this.addedToCart.emit(li);
  }

  getImageUrl() {
    const images = this.product.xp.Images || [];
    const result = _map(images, (img) => {
      return img.Url.replace('{url}', ocAppConfig.cmsUrl);
    });
    const filtered = _without(result, undefined);
    return filtered.length > 0 ? filtered[0] : 'http://placehold.it/300x300';
  }
}
