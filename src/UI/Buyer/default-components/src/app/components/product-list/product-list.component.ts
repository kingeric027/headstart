import { Component, Input, OnChanges, ChangeDetectorRef, OnInit } from '@angular/core';
import { ListBuyerProduct, ListFacet, Category } from '@ordercloud/angular-sdk';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { isEmpty as _isEmpty, each as _each } from 'lodash';
import { ModalState } from '../../models/modal-state.class';
import { OCMComponent } from '../base-component';
import { ProductFilters } from 'marketplace';
import { getScreenSizeBreakPoint } from 'src/app/services/breakpoint.helper';
import { takeWhile } from 'rxjs/operators';

@Component({
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class OCMProductList extends OCMComponent implements OnInit {
  @Input() products: ListBuyerProduct;
  facets: ListFacet[];
  categoryCrumbs: Category[];
  favoriteProducts: string[] = [];
  hasFilters = false;
  showingFavoritesOnly = false;
  closeIcon = faTimes;
  numberOfItemsInPagination = 10;

  ngOnContextSet() {
    if (this.products) this.facets = this.products.Meta.Facets;
    this.context.productFilters.activeFiltersSubject.pipe(takeWhile(() => this.alive)).subscribe(this.handleFiltersChange);
    this.context.currentUser.onFavoriteProductsChange(productIDs => (this.favoriteProducts = productIDs));
  }

  ngOnInit() {
    if (getScreenSizeBreakPoint() === 'xs') {
      this.numberOfItemsInPagination = 3;
    } else if (getScreenSizeBreakPoint() === 'sm') {
      this.numberOfItemsInPagination = 4;
    }
  }

  private handleFiltersChange = async (filters: ProductFilters) => {
    this.showingFavoritesOnly = filters.showOnlyFavorites;
    this.hasFilters = this.context.productFilters.hasFilters();
    this.categoryCrumbs = this.context.categories.breadCrumbs;
  }

  clearAllFilters() {
    this.context.productFilters.clearAllFilters();
  }

  changePage(page: number): void {
    this.context.productFilters.toPage(page);
  }

  toggleFilterByFavorites() {
    this.context.productFilters.filterByFavorites(!this.showingFavoritesOnly);
  }

  isFavorite(productID: string): boolean {
    return this.favoriteProducts.includes(productID);
  }

  setActiveCategory(categoryID: string): void {
    this.context.productFilters.filterByCategory(categoryID);
  }
}
