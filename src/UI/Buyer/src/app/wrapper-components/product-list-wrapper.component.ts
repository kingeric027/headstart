import { Component, OnInit, OnDestroy } from '@angular/core'
import { ShopperContextService } from '../services/shopper-context/shopper-context.service'
import { takeWhile } from 'rxjs/operators'
import { MarketplaceMeProduct } from '../shopper-context'
import { ListPage } from 'ordercloud-javascript-sdk'

@Component({
  template: `
    <ocm-product-list
      *ngIf="products"
      [products]="products"
      [isProductListLoading]="isProductListLoading"
    ></ocm-product-list>
  `,
})
export class ProductListWrapperComponent implements OnInit, OnDestroy {
  products: ListPage<MarketplaceMeProduct>
  alive = true
  isProductListLoading = true

  constructor(public context: ShopperContextService) {}

  ngOnInit(): void {
    this.context.productFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange)
  }

  ngOnDestroy(): void {
    this.alive = false
  }

  private handleFiltersChange = async (): Promise<void> => {
    this.isProductListLoading = true
    const user = this.context.currentUser.get()
    if (user?.UserGroups?.length) {
      try {
        this.products = await this.context.productFilters.listProducts()
      } finally {
        window.scroll(0, 0)
        this.isProductListLoading = false
      }
    } else {
      this.products = {
        Meta: {
          Page: 1,
          PageSize: 20,
          TotalCount: 0,
          TotalPages: 0,
          ItemRange: [1, 0],
        },
        Items: [],
      }
    }
  }
}
