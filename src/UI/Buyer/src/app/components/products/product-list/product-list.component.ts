import { Component, Input, OnInit, OnDestroy } from '@angular/core'
import { ListFacet, Category, ListPage } from 'ordercloud-javascript-sdk'
import { faTimes } from '@fortawesome/free-solid-svg-icons'
import { getScreenSizeBreakPoint } from 'src/app/services/breakpoint.helper'
import { takeWhile } from 'rxjs/operators'
import { MarketplaceKitProduct } from '@ordercloud/headstart-sdk'
import { MarketplaceMeProduct, ProductFilters } from 'src/app/shopper-context'
import { ShopperContextService } from 'src/app/services/shopper-context/shopper-context.service'

@Component({
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class OCMProductList implements OnInit, OnDestroy {
  @Input() products: ListPage<MarketplaceMeProduct>
  @Input() isProductListLoading: boolean
  alive = true
  facets: ListFacet[]
  categoryCrumbs: Category[]
  favoriteProducts: string[] = []
  hasFilters = false
  showingFavoritesOnly = false
  closeIcon = faTimes
  numberOfItemsInPagination = 10
  searchTermForProducts = ''

  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this.context.productFilters.activeFiltersSubject
      .pipe(takeWhile(() => this.alive))
      .subscribe(this.handleFiltersChange)
    this.context.currentUser.onChange(
      (user) => (this.favoriteProducts = user.FavoriteProductIDs)
    )
    if (getScreenSizeBreakPoint() === 'xs') {
      this.numberOfItemsInPagination = 3
    } else if (getScreenSizeBreakPoint() === 'sm') {
      this.numberOfItemsInPagination = 4
    }
  }

  clearAllFilters(): void {
    this.context.productFilters.clearAllFilters()
  }

  changePage(page: number): void {
    this.context.productFilters.toPage(page)
    window.location.hash = '#top'
  }

  toggleFilterByFavorites(): void {
    this.context.productFilters.filterByFavorites(!this.showingFavoritesOnly)
  }

  isFavorite(productID: string): boolean {
    return this.favoriteProducts.includes(productID)
  }

  setActiveCategory(categoryID: string): void {
    this.context.productFilters.filterByCategory(categoryID)
  }

  ngOnDestroy(): void {
    this.alive = false
  }

  private handleFiltersChange = (filters: ProductFilters): void => {
    this.showingFavoritesOnly = filters.showOnlyFavorites
    this.hasFilters = this.context.productFilters.hasFilters()
    this.categoryCrumbs = this.context.categories.breadCrumbs
    this.searchTermForProducts = filters.search
  }
}