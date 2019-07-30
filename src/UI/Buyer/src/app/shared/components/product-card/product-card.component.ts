import {
  Component,
  Input,
  ViewChild,
  EventEmitter,
  Output,
  OnInit,
  ViewEncapsulation,
} from '@angular/core';
import { QuantityInputComponent } from '@app-buyer/shared/components/quantity-input/quantity-input.component';
import { BuyerProduct, LineItem } from '@ordercloud/angular-sdk';
import { Router } from '@angular/router';
import { find as _find, get as _get } from 'lodash';

@Component({
  selector: 'product-product-card',
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class ProductCardComponent implements OnInit {
  @Input() product: BuyerProduct;
  @Input() favorite: boolean;
  @Output() addedToCart = new EventEmitter<LineItem>();
  @Output() setFavorite = new EventEmitter<boolean>();
  @ViewChild(QuantityInputComponent, { static: false })
  quantityInputComponent: QuantityInputComponent;
  shouldDisplayAddToCart: boolean;
  isViewOnlyProduct: boolean;
  isSetFavoriteUsed: boolean;
  alive = true;

  constructor(private router: Router) {}

  addToCart(li: LineItem) {
    this.addedToCart.emit(li);
  }

  hasSpecs(): boolean {
    return this.product.SpecCount > 0;
  }

  ngOnInit() {
    /**
     * this will be true if the parent component
     * is wired up to listen to the outputted event
     */
    this.isSetFavoriteUsed = this.setFavorite.observers.length > 0;
    const isAddedToCartUsed = this.addedToCart.observers.length > 0;
    this.isViewOnlyProduct = !this.product.PriceSchedule;
    this.shouldDisplayAddToCart =
      isAddedToCartUsed && !this.isViewOnlyProduct && !this.hasSpecs();
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
