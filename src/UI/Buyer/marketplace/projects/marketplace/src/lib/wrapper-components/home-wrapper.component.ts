import { Component, OnInit } from '@angular/core';
import { MarketplaceMeProduct } from '../shopper-context';
import { ListPage } from '@ordercloud/headstart-sdk';

@Component({
  template: `
    <ocm-home-page [featuredProducts]="featuredProducts.Items"></ocm-home-page>
  `,
})
export class HomeWrapperComponent implements OnInit {
  featuredProducts: ListPage<MarketplaceMeProduct>;

  constructor() {}

  ngOnInit(): void {
    this.featuredProducts = { Items: [] };
  }
}
