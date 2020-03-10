import { Component, OnInit } from '@angular/core';
import { ListSpec } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { MarketplaceMeProduct } from '../shopper-context';

@Component({
  template: `
    <ocm-product-details [product]="product" [specs]="specs"> </ocm-product-details>
  `,
})
export class ProductDetailWrapperComponent implements OnInit {
  specs: ListSpec;
  product: MarketplaceMeProduct;

  constructor(
    private activatedRoute: ActivatedRoute,
    public context: ShopperContextService, // used in template
    protected currentUser: CurrentUserService
  ) {}

  ngOnInit(): void {
    this.product = this.activatedRoute.snapshot.data.product;
    this.specs = this.activatedRoute.snapshot.data.specs || [];
  }
}
