import {
  Component,
  Input,
  EventEmitter,
  Output,
  OnInit,
  ViewEncapsulation,
} from '@angular/core';
import { BuyerProduct, LineItem } from '@ordercloud/angular-sdk';
import { Router } from '@angular/router';
import { find as _find, get as _get } from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'ocm-product-card',
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class OCMProductCard implements OnInit {
  @Input() product: BuyerProduct;
  @Input() isFavorite: boolean;
  @Input() quantityLimits: QuantityLimits;
  @Output() addedToCart = new EventEmitter<LineItem>();
  @Output() setIsFavorite = new EventEmitter<boolean>();

  quantity: number;
  shouldDisplayAddToCart: boolean;
  isViewOnlyProduct: boolean;
  isSetFavoriteUsed: boolean;

  constructor(private router: Router) {}

  ngOnInit() {
    /**
     * this will be true if the parent component
     * is wired up to listen to the outputted event
     */
    this.isSetFavoriteUsed = this.setIsFavorite.observers.length > 0;
    const isAddedToCartUsed = this.addedToCart.observers.length > 0;
    this.isViewOnlyProduct = !this.product.PriceSchedule;
    this.shouldDisplayAddToCart =
      isAddedToCartUsed && !this.isViewOnlyProduct && !this.hasSpecs();
  }

  addToCart(li: LineItem) {
    this.addedToCart.emit(li);
  }

  hasSpecs(): boolean {
    return this.product.SpecCount > 0;
  }

  featuredProducts() {
    return this.router.url.indexOf('/home') > -1;
  }

  getImageUrl() {
    return _get(
      this.product,
      'xp.Images[0].Url',
      'http://placehold.it/300x300'
    );
  }
}
