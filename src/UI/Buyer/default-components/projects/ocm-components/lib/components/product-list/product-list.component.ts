import { Component, Input, OnChanges } from '@angular/core';
import { ListBuyerProduct, Category, ListCategory, ListFacet } from '@ordercloud/angular-sdk';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { isEmpty as _isEmpty, each as _each } from 'lodash';
import { ModalState } from '../../models/modal-state.class';
import { OCMComponent } from '../base-component';
import { QuantityLimits } from '../../models/quantity-limits';
import { ProductFilters } from 'shopper-context-interface';

@Component({
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class OCMProductList extends OCMComponent {
  @Input() products: ListBuyerProduct;
  @Input() categories: ListCategory;
  @Input() quantityLimits: QuantityLimits[];
  categoryModal = ModalState.Closed;
  facets: ListFacet[];
  categoryCrumbs: Category[] = [];
  hasQueryParams = false;
  showingFavoritesOnly = false;
  closeIcon = faTimes;

  ngOnContextSet() {
    this.facets = this.products.Meta.Facets;
    this.context.productFilters.onFiltersChange(this.handleFiltersChange);
  }

  private handleFiltersChange = async (filters: ProductFilters) => {
    this.showingFavoritesOnly = filters.showOnlyFavorites;
    this.categoryCrumbs = this.buildBreadCrumbs(filters.categoryID);
  }

  clearAllFilters() {
    this.context.productFilters.clearAllFilters();
  }

  changePage(page: number): void {
    this.context.productFilters.toPage(page);
  }

  setActiveCategory(categoryID: string): void {
    this.context.productFilters.filterByCategory(categoryID);
  }

  toggleFilterByFavorites() {
    this.context.productFilters.filterByFavorites(!this.showingFavoritesOnly);
  }

  buildBreadCrumbs(activeCategoryID: string, progress = []): Category[] {
    if (!activeCategoryID || !this.categories || this.categories.Items.length < 1) {
      return progress;
    }
    const category = this.getCategory(activeCategoryID);
    if (!category) return progress;
    progress.unshift(category);

    return this.buildBreadCrumbs(category.ParentID, progress);
  }

  getCategory(categoryID: string): Category {
    return this.categories.Items.find((cat) => cat.ID === categoryID);
  }

  openCategoryModal() {
    this.categoryModal = ModalState.Open;
  }
}