import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  find as _find,
  difference as _difference,
  without as _without,
  map as _map,
  minBy as _minBy,
} from 'lodash';

import { BuyerProduct, ListBuyerSpec } from '@ordercloud/angular-sdk';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';
import { CartService, AppStateService } from '@app-buyer/shared';
import { ocAppConfig } from '@app-buyer/config/app.config';
import { SpecFormEvent } from '@app-buyer/product/models/spec-form-values.interface';

@Component({
  selector: 'product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class ProductDetailsComponent implements OnInit {
  specs: ListBuyerSpec;
  product: BuyerProduct;
  relatedProducts: BuyerProduct[];
  imageUrls: string[] = [];
  price: number;

  constructor(
    private activatedRoute: ActivatedRoute,
    private cartService: CartService,
    private appStateService: AppStateService,
    protected favoriteProductService: FavoriteProductsService, // used in template
    private router: Router
  ) {}

  ngOnInit(): void {
    this.product = this.activatedRoute.snapshot.data.product;
    this.specs = this.activatedRoute.snapshot.data.specs;
    this.relatedProducts = this.activatedRoute.snapshot.data.relatedProducts;
  }

  routeToProductList(): void {
    this.router.navigate(['/products']);
  }

  handleChange(event: SpecFormEvent): void {
    if (event.type === 'Change') {
      this.price = event.price;
    }
  }

  addToCart(event: any): void {
    this.cartService
      .addToCart(this.product.ID, event.values.quantity, event.values.specs)
      .subscribe(() => {
        this.appStateService.addToCartSubject.next({
          product: this.product,
          quantity: event.values.quantity,
        });
      });
  }

  qtyChanged(values: any): void {
    // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
    // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
    // is the cost per item.
    if (!this.hasPrice()) {
      this.price = 0;
    }
    const priceBreaks = this.product.PriceSchedule.PriceBreaks;
    const startingBreak = _minBy(priceBreaks, 'Quantity');

    const selectedBreak = priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity &&
        candidate.Quantity <= values.quantity
        ? candidate
        : current;
    }, startingBreak);
    this.price = selectedBreak.Price * values.quantity;
  }

  isOrderable(): boolean {
    // products without a price schedule are view-only.
    return !!this.product.PriceSchedule;
  }

  hasPrice(): boolean {
    // free products dont need to display a price.
    return (
      this.product.PriceSchedule &&
      this.product.PriceSchedule.PriceBreaks.length &&
      this.product.PriceSchedule.PriceBreaks[0].Price > 0
    );
  }

  getImageUrls() {
    const images = this.product.xp.Images || [];
    const result = _map(images, (img) => {
      return img.Url.replace('{url}', ocAppConfig.cdnUrl);
    });
    return _without(result, undefined);
  }
}
