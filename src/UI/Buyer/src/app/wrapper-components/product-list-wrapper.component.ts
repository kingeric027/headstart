import { Component, OnInit, OnDestroy } from '@angular/core'
import { ShopperContextService } from '../services/shopper-context/shopper-context.service'
import { takeWhile } from 'rxjs/operators'
import { MarketplaceMeProduct } from '../shopper-context'
import { ListPage } from 'ordercloud-javascript-sdk'
import { uniq as _uniq } from 'lodash'
import { SupplierFilterService } from '../services/supplier-filter/supplier-filter.service'

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

  constructor(
    public context: ShopperContextService, 
    private supplierFilterService: SupplierFilterService
  ) {}

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
        let sourceIds = {} 
        this.products.Items.forEach(p => {
          if (!p.DefaultSupplierID || !p.ShipFromAddressID) return
          const source = sourceIds[p.DefaultSupplierID]
          if (!source) {
            sourceIds[p.DefaultSupplierID] = [p.ShipFromAddressID]
          } else {
            sourceIds[p.DefaultSupplierID] = _uniq([...source, p.ShipFromAddressID])
          }
        })
        /**
         *           if (!p.DefaultSupplierID || !p.ShipFromAddressID) return;
          const source = this.shipFromSources[p.DefaultSupplierID]
          if (!source) {
            this.shipFromSources[p.DefaultSupplierID] = [await this.supplierFilterService.getSupplierAddress(p.DefaultSupplierID, p.ShipFromAddressID)]
          } else if (this.shipFromSources[p.DefaultSupplierID].map(address => address.ID).indexOf(p.ShipFromAddressID) < 0) {
            this.shipFromSources[p.DefaultSupplierID] = _uniqBy(
              [
                ...this.shipFromSources[p.DefaultSupplierID], 
                await this.supplierFilterService.getSupplierAddress(p.DefaultSupplierID, p.ShipFromAddressID)
              ], 
              'ID')
          }
         */
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
