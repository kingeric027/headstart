import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { keyBy as _keyBy, mapValues as _mapValues } from 'lodash';

interface ProductListParams {
  page?: number;
  sortBy?: string;
  search?: string;
  filters: ProductListFilter[];
}

interface ProductListFilter {
  field: string;
  value: string;
  isFacet: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class ProductListService {
  private readonly defaultParams = {
    page: null,
    sortBy: null,
    search: null,
    filters: [],
  };
  private paramsSubject: BehaviorSubject<ProductListParams> = new BehaviorSubject<ProductListParams>(this.defaultParams);

  constructor(private router: Router) {}

  private updateUrl() {
    const { page, sortBy, search, filters } = this.paramsSubject.value;
    const filterObject = _mapValues(_keyBy(filters, 'field'), 'value');
    const queryParams = { page, sortBy, search, ...filterObject };
    this.router.navigate([], { queryParams });
  }

  toPage(pageNumber: number) {
    this.paramsSubject.next({ ...this.paramsSubject.value, page: pageNumber });
    this.updateUrl();
  }

  sortBy(field: string) {
    const newState = { ...this.paramsSubject.value, sortBy: field, page: null };
    this.paramsSubject.next(newState);
    this.updateUrl();
  }

  clearSort() {
    this.sortBy(null);
  }

  searchBy(searchTerm: string) {
    this.paramsSubject.next({ ...this.paramsSubject.value, search: searchTerm, page: null });
    this.updateUrl();
  }

  clearSearch() {
    this.searchBy(null);
  }

  filterBy(field: string, value: string, isFacet: boolean = false) {
    const filters = this.paramsSubject.value.filters;
    filters.push({ field, value, isFacet });
    this.paramsSubject.next({ ...this.paramsSubject.value, filters, page: null });
    this.updateUrl();
  }

  filterByCategory(categoryID: string) {
    this.filterBy('categoryID', categoryID, false);
  }

  clearCategoryFilter() {
    this.removeFilter('categoryID');
  }

  filterByFavorites(showOnlyFavorites: boolean) {
    if (showOnlyFavorites) {
      this.filterBy('favorites', 'true', false);
    } else {
      this.removeFilter('favorites');
    }
  }

  removeFilter(field: string) {
    const filters = this.paramsSubject.value.filters.filter((f) => f.field !== field);
    this.paramsSubject.next({ ...this.paramsSubject.value, filters, page: null });
    this.updateUrl();
  }

  clearAllFilters() {
    this.paramsSubject.next(this.defaultParams);
    this.updateUrl();
  }

  get activeSearchTerm(): string {
    return this.paramsSubject.value.search || null;
  }

  get activeCategory(): string {
    const filter = this.paramsSubject.value.filters.find((f) => f.field === 'categoryID');
    return filter ? filter.value : null;
  }

  get activeSort(): string {
    return this.paramsSubject.value.sortBy || null;
  }

  get page(): number {
    return this.paramsSubject.value.page;
  }

  get activeFilters(): ProductListFilter[] {
    return this.paramsSubject.value.filters.filter((f) => f.field !== 'categoryID' && f.field !== 'favorites');
  }

  get showingOnlyFavorites(): boolean {
    return this.paramsSubject.value.filters.some((f) => f.field === 'favorites');
  }
}
