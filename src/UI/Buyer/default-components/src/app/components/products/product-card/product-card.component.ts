import { Component, Input, ViewEncapsulation, ChangeDetectorRef } from '@angular/core';
import { ShopperContextService, MarketplaceMeProduct, PriceSchedule } from 'marketplace';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';
import { exchange } from 'src/app/services/currency.helper';
import { ListPage } from 'marketplace-javascript-sdk';
import { ExchangeRates } from 'marketplace';
import { BuyerCurrency } from 'src/app/models/currency.interface';
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
  _price: BuyerCurrency;
  _rates: ListPage<ExchangeRates>;
  quantity: number;
  shouldDisplayAddToCart = false;
  isViewOnlyProduct = true;
  hasSpecs = false;
  isAddingToCart = false;
  exchangeRates: ExchangeRates[];

  constructor(private cdr: ChangeDetectorRef, private context: ShopperContextService) {}

  @Input() set product(value: MarketplaceMeProduct) {
    this._product = value;
    this._rates = this.context.exchangeRates.Get(); 
    this.setPrice(value, value.PriceSchedule);
    this.isViewOnlyProduct = !value.PriceSchedule;
    this.hasSpecs = value.SpecCount > 0;
  }

  @Input() set isFavorite(value: boolean) {
    this._isFavorite = value;
    this.cdr.detectChanges(); // TODO - remove. Solve another way.
  }

  setPrice(product: MarketplaceMeProduct, priceSchedule: PriceSchedule<any>): void {
    const currentUser = this.context.currentUser.get();
    const productPrice = priceSchedule?.PriceBreaks[0]?.Price;
    this._price = exchange(this._rates, productPrice, product?.xp?.Currency, currentUser.Currency);
  }

  async addToCart(): Promise<void> {
    this.isAddingToCart = true;
    try {
      await this.context.order.cart.add({ 
        ProductID: this._product.ID, 
        Quantity: this.quantity,
        xp: {
          LineItemImageUrl: getPrimaryImageUrl(this._product)
        } 
      });
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
