import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { ListMarketplaceProduct } from '../shopper-context';

@Component({
  template: `
    <ocm-home-page [featuredProducts]="featuredProducts.Items"></ocm-home-page>
  `,
})
export class HomeWrapperComponent implements OnInit {
  featuredProducts: ListMarketplaceProduct;

  constructor(private activatedRoute: ActivatedRoute, public context: ShopperContextService) {}

  ngOnInit() {
    this.featuredProducts = this.activatedRoute.snapshot.data.featuredProducts;
  }
}
