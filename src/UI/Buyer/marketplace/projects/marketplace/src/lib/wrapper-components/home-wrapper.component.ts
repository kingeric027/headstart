import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MarketplaceMeProduct } from '../shopper-context';
import { ListPage } from 'marketplace-javascript-sdk';

@Component({
  template: `
    <ocm-home-page [featuredProducts]="featuredProducts.Items"></ocm-home-page>
  `,
})
export class HomeWrapperComponent implements OnInit {
  featuredProducts: ListPage<MarketplaceMeProduct>;

  constructor(private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.featuredProducts = this.activatedRoute.snapshot.data.featuredProducts;
  }
}
