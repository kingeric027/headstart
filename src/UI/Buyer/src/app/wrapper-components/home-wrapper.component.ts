import { Component, OnInit } from '@angular/core'
import { ListPage, MarketplaceMeProduct } from '@ordercloud/headstart-sdk'

@Component({
  template: `
    <ocm-home-page [featuredProducts]="featuredProducts.Items"></ocm-home-page>
  `,
})
export class HomeWrapperComponent implements OnInit {
  featuredProducts: ListPage<MarketplaceMeProduct>

  constructor() {}

  ngOnInit(): void {
    this.featuredProducts = { Items: [] }
  }
}
