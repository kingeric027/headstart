import { Component, OnInit } from '@angular/core';
import { BuyerProduct, ListSpec } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { QuantityLimits } from '../models/quantity-limits';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { BuildQtyLimits } from '../functions/product.quantity.validator';

@Component({
  template: `
    <ocm-product-details [product]="product" [specs]="specs" [quantityLimits]="quantityLimits" [context]="context"> </ocm-product-details>
  `,
})
export class ProductDetailWrapperComponent implements OnInit {
  specs: ListSpec;
  product: BuyerProduct;
  quantityLimits: QuantityLimits;

  constructor(
    private activatedRoute: ActivatedRoute,
    public context: ShopperContextService, // used in template
    protected currentUser: CurrentUserService
  ) {}

  ngOnInit(): void {
    this.product = this.activatedRoute.snapshot.data.product;
    this.specs = this.activatedRoute.snapshot.data.specs || [];
    this.quantityLimits = BuildQtyLimits(this.product);
  }
}
