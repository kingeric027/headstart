import { Component, OnInit } from '@angular/core';
import { ListSpec } from '@ordercloud/angular-sdk';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { CurrentUserService } from '../services/current-user/current-user.service';
import { MarketplaceMeProduct } from '../shopper-context';
import { SuperMarketplaceProduct } from 'marketplace-javascript-sdk';

@Component({
  template: `
    <ocm-product-details [product]="product"> </ocm-product-details>
  `,
})
export class ProductDetailWrapperComponent implements OnInit {
  product: SuperMarketplaceProduct;

  constructor(
    private activatedRoute: ActivatedRoute,
    public context: ShopperContextService, // used in template
    protected currentUser: CurrentUserService
  ) {}

  ngOnInit(): void {
    this.product = this.activatedRoute.snapshot.data.product;
  }
}
