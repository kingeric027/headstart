import { Component, OnInit, ViewChild, AfterViewChecked } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CartService, AppStateService } from '@app-buyer/shared';
import {
  BuyerProduct,
  BuyerSpec,
  LineItemSpec,
  SpecOption,
} from '@ordercloud/angular-sdk';
import { QuantityInputComponent } from '@app-buyer/shared/components/quantity-input/quantity-input.component';
import { AddToCartEvent } from '@app-buyer/shared/models/add-to-cart-event.interface';
import { minBy as _minBy } from 'lodash';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';
import {
  find as _find,
  difference as _difference,
  without as _without,
  map as _map,
} from 'lodash';
import { SpecFormComponent } from '@app-buyer/product/components/spec-form/spec-form.component';
import { ocAppConfig } from '@app-buyer/config/app.config';
@Component({
  selector: 'product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class ProductDetailsComponent implements OnInit {
  @ViewChild(SpecFormComponent, { static: false })
  specFormComponent: SpecFormComponent;
  specs: BuyerSpec[];
  product: BuyerProduct;
  relatedProducts: BuyerProduct[];
  imageUrls: string[] = [];
  specSelections: FullSpecOption[] = [];
  quantityInputReady = false;
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
    this.qtyChanged(1);
  }

  routeToProductList(): void {
    this.router.navigate(['/products']);
  }

  qtyChanged(quantity: any): void {
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
        candidate.Quantity <= quantity
        ? candidate
        : current;
    }, startingBreak);
    const markup = this.totalSpecMarkup(selectedBreak.Price, quantity);
    this.price = (selectedBreak.Price + markup) * quantity;
  }

  addToCart(event: AddToCartEvent): void {
    const specs: LineItemSpec[] = this.specSelections.map((o) => ({
      SpecID: o.SpecID,
      OptionID: o.ID,
      Value: o.Value,
    }));
    this.cartService
      .addToCart(event.product.ID, event.quantity, specs)
      .subscribe(() => this.appStateService.addToCartSubject.next(event));
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

  totalSpecMarkup(unitPrice: number, quantity: number): number {
    const markups = this.specSelections.map((s) =>
      this.singleSpecMarkup(unitPrice, quantity, s)
    );
    return markups.reduce((x, acc) => x + acc, 0); //sum
  }

  specsUpdated(event: FullSpecOption[]) {
    this.specSelections = event;
  }

  missingRequiredSpec(): boolean {
    if (this.specs.length === 0) return false;
    if (this.specFormComponent === undefined) return true;
    return this.specFormComponent.specForm.invalid;
  }

  singleSpecMarkup(
    unitPrice: number,
    quantity: number,
    spec: FullSpecOption
  ): number {
    switch (spec.PriceMarkupType) {
      case 'NoMarkup':
        return 0;
      case 'AmountPerQuantity':
        return spec.PriceMarkup;
      case 'AmountTotal':
        return spec.PriceMarkup / quantity;
      case 'Percentage':
        return spec.PriceMarkup * unitPrice * 0.01;
    }
  }

  getImageUrls() {
    const images = this.product.xp.Images || [];
    const result = _map(images, (img) => {
      return img.Url.replace('{url}', ocAppConfig.cdnUrl);
    });
    return _without(result, undefined);
  }
}

export interface FullSpecOption extends SpecOption {
  SpecID: string;
}
