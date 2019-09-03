import { Component, Input, Output, EventEmitter } from '@angular/core';
import { faAngleLeft, faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { BuyerProduct } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { Navigator } from '@app-buyer/shared/services/navigator/navigator.service';

@Component({
  templateUrl: './product-carousel.component.html',
  styleUrls: ['./product-carousel.component.scss'],
})
export class OCMProductCarousel {
  @Input() products: BuyerProduct[] = [];
  @Input() displayTitle: string;
  @Input() quantityLimits: QuantityLimits[];
  @Input() favoriteProducts: string[];
  @Input() navigator: Navigator;
  @Output() setIsFavorite = new EventEmitter<{ isFavorite: boolean; productID: string }>();

  index = 0;
  rowLength = 4;
  faAngleLeft = faAngleLeft;
  faAngleRight = faAngleRight;

  left(): void {
    this.index -= this.rowLength;
  }

  right(): void {
    this.index += this.rowLength;
  }

  getProducts(): BuyerProduct[] {
    return this.products.slice(this.index, this.index + this.rowLength);
  }

  isFavorite(productID: string): boolean {
    return this.favoriteProducts.includes(productID);
  }

  setIsFavoriteEvent(isFavorite: boolean, productID: string): void {
    this.setIsFavorite.emit({ isFavorite, productID });
  }
}
