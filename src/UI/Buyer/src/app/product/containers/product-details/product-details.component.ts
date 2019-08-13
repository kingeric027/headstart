import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Validators } from '@angular/forms';
import {
  find as _find,
  difference as _difference,
  without as _without,
  map as _map,
  minBy as _minBy,
} from 'lodash';

import {
  BuyerProduct,
  SpecOption,
  ListBuyerSpec,
  LineItemSpec,
  BuyerSpec,
} from '@ordercloud/angular-sdk';
import { FavoriteProductsService } from '@app-buyer/shared/services/favorites/favorites.service';
import {
  ProductQtyValidator,
  CartService,
  AppStateService,
} from '@app-buyer/shared';
import { ocAppConfig } from '@app-buyer/config/app.config';
import { FieldConfig } from '@app-buyer/product/components/spec-form/field-config.interface';

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
  specConfig: FieldConfig[];
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
    this.specConfig = this.createSpecForm(
      this.activatedRoute.snapshot.data.specs
    );
  }

  routeToProductList(): void {
    this.router.navigate(['/products']);
  }

  createSpecForm(specs: ListBuyerSpec): FieldConfig[] {
    const c: FieldConfig[] = [];
    for (const spec of specs.Items) {
      if (spec.Options.length > 1) {
        c.push({
          type: 'select',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.Options[0].Value,
          options: _map(spec.Options, 'Value'),
        });
      } else if (spec.AllowOpenText) {
        c.push({
          type: 'input',
          label: spec.Name,
          name: spec.Name.replace(/ /g, ''),
          value: spec.DefaultValue,
        });
      }
    }
    c.push({
      type: 'addtocart',
      label: 'Add to Cart',
      name: 'quantity',
      min: 1,
      step: 1,
      validation: [Validators.required, ProductQtyValidator(this.product)],
      options: _map(this.product.PriceSchedule.PriceBreaks, 'Quantity'),
    });
    return c;
  }

  handleChange(event: any): void {
    if (event.event === 'OnChange') {
      this.qtyChanged(event.values);
    }
  }

  private selectedSpecs(values: any): Array<LineItemSpec> {
    const specs: Array<LineItemSpec> = new Array<LineItemSpec>();
    for (const value in values) {
      //if (event.hasOwnProperty(value) && value !== 'ctrls') {
      if (value !== 'ctrls') {
        const spec = _find(
          this.specs.Items,
          (item) => item.Name.replace(/ /g, '') === value
        ) as BuyerSpec;
        if (!spec) continue;
        const option = _find(
          spec.Options,
          (o) => o.Value === values[value]
        ) as SpecOption;
        if (option) {
          specs.push({
            SpecID: spec.ID,
            OptionID: option.ID,
            Value: option.Value,
          });
        }
      }
    }
    return specs;
  }

  addToCart(event: any): void {
    this.cartService
      .addToCart(
        this.product.ID,
        event.values.quantity,
        this.selectedSpecs(event.values)
      )
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
    const markup = this.totalSpecMarkup(selectedBreak.Price, values);
    this.price = (selectedBreak.Price + markup) * values.quantity;
  }

  totalSpecMarkup(unitPrice: number, values: any): number {
    const markups: Array<number> = new Array<number>();
    for (const value in values) {
      if (values.hasOwnProperty(value) && value !== 'ctrls') {
        const spec = _find(
          this.specs.Items,
          (item) => item.Name.replace(/ /g, '') === value
        ) as BuyerSpec;
        if (!spec) continue;
        const option = _find(
          spec.Options,
          (o) => o.Value === values[value] && o.PriceMarkupType !== 'NoMarkup'
        ) as SpecOption;
        if (option) {
          markups.push(
            this.singleSpecMarkup(unitPrice, values.quantity, option)
          );
        }
      }
    }
    return markups.reduce((x, acc) => x + acc, 0); //sum
  }

  singleSpecMarkup(
    unitPrice: number,
    quantity: number,
    option: SpecOption
  ): number {
    switch (option.PriceMarkupType) {
      case 'NoMarkup':
        return 0;
      case 'AmountPerQuantity':
        return option.PriceMarkup;
      case 'AmountTotal':
        return option.PriceMarkup / quantity;
      case 'Percentage':
        return option.PriceMarkup * unitPrice * 0.01;
    }
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
