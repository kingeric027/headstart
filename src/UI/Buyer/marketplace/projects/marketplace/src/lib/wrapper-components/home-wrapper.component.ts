import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ListBuyerProduct } from '@ordercloud/angular-sdk';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { BuildQtyLimits } from '../functions/product.quantity.validator';

@Component({
  template: `
    <ocm-home-page [featuredProducts]="featuredProducts.Items" [context]="context"></ocm-home-page>
  `,
})
export class HomeWrapperComponent implements OnInit {
  featuredProducts: ListBuyerProduct;
  favoriteProductIDs: string[];

  constructor(private activatedRoute: ActivatedRoute, public context: ShopperContextService) {}

  ngOnInit() {
    this.featuredProducts = this.activatedRoute.snapshot.data.featuredProducts;
  }
}
