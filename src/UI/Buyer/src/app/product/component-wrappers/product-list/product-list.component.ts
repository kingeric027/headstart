import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { ListBuyerProduct, Category, ListCategory, ListFacet } from '@ordercloud/angular-sdk';
import { ModalService, BuildQtyLimits } from '@app-buyer/shared';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { isEmpty as _isEmpty, each as _each } from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { OCMComponent } from '@app-buyer/ocm-default-components/shopper-context';
import { ProductFilters } from '@app-buyer/shared/services/product-filter/product-filter.service';

@Component({
  selector: 'ocm-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class ProductListComponent extends OCMComponent implements OnInit, OnChanges {
  @Input() products: ListBuyerProduct;
  @Input() categories: ListCategory;
  facets: ListFacet[];
  categoryCrumbs: Category[] = [];
  hasQueryParams = false;
  showingFavoritesOnly = false;
  closeIcon = faTimes;
  isModalOpen = false;
  createModalID = 'selectCategory';
  quantityLimits: QuantityLimits[];

  constructor(private modalService: ModalService) {
    super();
  }

  ngOnInit() {
    this.context.productFilterActions.onFiltersChange(this.handleFiltersChange);
  }

  ngOnChanges() {
    this.facets = this.products.Meta.Facets;
    this.quantityLimits = this.products.Items.map((p) => BuildQtyLimits(p));
  }

  private handleFiltersChange = async (filters: ProductFilters) => {
    this.hasQueryParams = true; // TODO - implement
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
    this.modalService.open('selectCategory');
    this.isModalOpen = true;
  }

  // TODO - it may be that this function is never used, but it should be.
  closeCategoryModal() {
    this.isModalOpen = false;
    this.modalService.close('selectCategory');
  }
}
