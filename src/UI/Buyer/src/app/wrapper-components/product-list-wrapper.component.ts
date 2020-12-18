import { Component, OnInit, OnDestroy } from '@angular/core'
import { ShopperContextService } from '../services/shopper-context/shopper-context.service'
import { takeWhile } from 'rxjs/operators'
import { MarketplaceMeProduct } from '../shopper-context'
import { ListPage } from 'ordercloud-javascript-sdk'
import { uniq as _uniq } from 'lodash'

@Component({
  template: `
    <ocm-product-list
      *ngIf="products"
      [products]="products"
      [shipFromSources]="shipFromSources"
      [isProductListLoading]="isProductListLoading"
    ></ocm-product-list>
  `,
})
export class ProductListWrapperComponent implements OnInit, OnDestroy {
  products: ListPage<MarketplaceMeProduct>
  shipFromSources: any = {}
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
        this.products.Items.forEach(p => {
          const source = this.shipFromSources[p.DefaultSupplierID]
          if (!source) {
            this.shipFromSources[p.DefaultSupplierID] = [p.ShipFromAddressID]
          } else {
            this.shipFromSources[p.DefaultSupplierID] = _uniq([...source, p.ShipFromAddressID])
          }
        })
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
