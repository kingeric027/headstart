import { Component, Input, OnChanges } from '@angular/core';
import { ListBuyerProduct, Category, ListCategory, ListFacet } from '@ordercloud/angular-sdk';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { isEmpty as _isEmpty, each as _each } from 'lodash';
import { QuantityLimits } from 'src/app/shared/models/quantity-limits';
import { OCMComponent, ProductFilters } from 'src/app/ocm-default-components/shopper-context';

@Component({
  selector: 'ocm-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class OCMProductList extends OCMComponent implements OnChanges {
  @Input() products: ListBuyerProduct;
  @Input() categories: ListCategory;
  @Input() quantityLimits: QuantityLimits[];
  categoryModalOpen = false;
  facets: ListFacet[];
  categoryCrumbs: Category[] = [];
  hasQueryParams = false;
  showingFavoritesOnly = false;
  closeIcon = faTimes;

  ngOnChanges() {
    this.facets = this.products.Meta.Facets;
    this.context.productFilterActions.onFiltersChange(this.handleFiltersChange);
  }

  private handleFiltersChange = async (filters: ProductFilters) => {
    this.showingFavoritesOnly = filters.showOnlyFavorites;
    this.categoryCrumbs = this.buildBreadCrumbs(filters.categoryID);
  };

  clearAllFilters() {
    this.context.productFilterActions.clearAllFilters();
  }

  changePage(page: number): void {
    this.context.productFilterActions.toPage(page);
  }

  setActiveCategory(categoryID: string): void {
    this.context.productFilterActions.filterByCategory(categoryID);
  }

  toggleFilterByFavorites() {
    this.context.productFilterActions.filterByFavorites(!this.showingFavoritesOnly);
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
    this.categoryModalOpen = true;
  }
}
