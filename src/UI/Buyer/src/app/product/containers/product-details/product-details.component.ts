import {
  Component,
  OnInit,
  ViewChild,
  AfterViewChecked,
  ChangeDetectorRef,
} from '@angular/core';
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
import { AddToCartEvent } from '@app-buyer/shared/models/add-to-cart-event.interface';
import { QuantityInputComponent } from '@app-buyer/shared/components/quantity-input/quantity-input.component';

@Component({
  selector: 'product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class ProductDetailsComponent implements OnInit, AfterViewChecked {
  @ViewChild(QuantityInputComponent, { static: false })
  quantityInputComponent: QuantityInputComponent;
  specs: ListBuyerSpec;
  product: BuyerProduct;
  relatedProducts: BuyerProduct[];
  imageUrls: string[] = [];
  specForm: SpecFormEvent = {
    valid: false,
    markup: 0,
    specs: [],
    type: '',
  };

  constructor(
    private activatedRoute: ActivatedRoute,
    private changeDetectorRef: ChangeDetectorRef,
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

  ngAfterViewChecked() {
    // This manually triggers angular's change detection cycle and avoids the imfamous
    // "Expression has changed after it was checked" error.
    // If you remove the @ViewChild(QuantityInputComponent) this will be unecessary.
    this.changeDetectorRef.detectChanges();
  }

  routeToProductList(): void {
    this.router.navigate(['/products']);
  }

  handleChange(event: SpecFormEvent): void {
    if (event.type === 'Change') {
      this.specForm = event;
    }
  }

  addToCart(event: AddToCartEvent): void {
    this.cartService
      .addToCart(this.product.ID, event.quantity, this.specForm.specs)
      .subscribe(() => {
        this.appStateService.addToCartSubject.next({
          product: this.product,
          quantity: event.quantity,
        });
      });
  }

  getTotalPrice(): number {
    // In OC, the price per item can depend on the quantity ordered. This info is stored on the PriceSchedule as a list of PriceBreaks.
    // Find the PriceBreak with the highest Quantity less than the quantity ordered. The price on that price break
    // is the cost per item.
    if (!this.quantityInputComponent || !this.quantityInputComponent.form) {
      return null;
    }
    if (!this.hasPrice()) {
      return 0;
    }
    const quantity = this.quantityInputComponent.form.value.quantity;
    const priceBreaks = this.product.PriceSchedule.PriceBreaks;
    const startingBreak = _minBy(priceBreaks, 'Quantity');

    const selectedBreak = priceBreaks.reduce((current, candidate) => {
      return candidate.Quantity > current.Quantity &&
        candidate.Quantity <= quantity
        ? candidate
        : current;
    }, startingBreak);
    return (selectedBreak.Price + (this.specForm.markup || 0)) * quantity;
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
