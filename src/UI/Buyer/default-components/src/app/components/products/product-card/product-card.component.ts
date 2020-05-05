import { Component, Input, ViewEncapsulation, ChangeDetectorRef } from '@angular/core';
import { ShopperContextService, MarketplaceMeProduct } from 'marketplace';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';

@Component({
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class OCMProductCard {
  _isFavorite = false;
  _product: MarketplaceMeProduct = {
    PriceSchedule: {},
  };
  quantity: number;
  shouldDisplayAddToCart = false;
  isViewOnlyProduct = true;
  hasSpecs = false;
  isAddingToCart = false;

  constructor(private cdr: ChangeDetectorRef, private context: ShopperContextService) {}

  @Input() set product(value: MarketplaceMeProduct) {
    this._product = value;
    this.isViewOnlyProduct = !value.PriceSchedule;
    this.hasSpecs = value.SpecCount > 0;
  }

  @Input() set isFavorite(value: boolean) {
    this._isFavorite = value;
    this.cdr.detectChanges(); // TODO - remove. Solve another way.
  }

  async addToCart(): Promise<void> {
    this.isAddingToCart = true;
    try {
      await this.context.order.cart.add({ ProductID: this._product.ID, Quantity: this.quantity });
      this.isAddingToCart = false;
    } catch (ex) {
      this.isAddingToCart = false;
      throw ex;
    }
  }

  getImageUrl(): string {
    return getPrimaryImageUrl(this._product);
  }

  toDetails(): void {
    this.context.router.toProductDetails(this._product.ID);
  }

  setIsFavorite(isFavorite: boolean): void {
    this.context.currentUser.setIsFavoriteProduct(isFavorite, this._product.ID);
  }

  showAddToCart(): boolean {
    return !this.isViewOnlyProduct && !this.hasSpecs && this._product.xp.ProductType !== 'Quote';
  }

  setQuantity(event: any): void {
    this.quantity = event.qty;
  }
}
