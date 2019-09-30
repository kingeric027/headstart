import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { CurrentUserService } from '../current-user/current-user.service';
import { IProductFilterActions, ProductFilters } from 'shopper-context-interface';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class ProductFilterService implements IProductFilterActions {
  private readonly defaultParams = {
    page: undefined,
    sortBy: undefined,
    search: undefined,
    categoryID: undefined,
    showOnlyFavorites: false,
    activeFacets: {},
  };

  // TODO - allow app devs to filter by custom xp that is not a facet. Create functions for this.
  private readonly nonFacetQueryParams = ['page', 'sortBy', 'categoryID', 'search', 'favorites'];

  private activeFiltersSubject: BehaviorSubject<ProductFilters> = new BehaviorSubject<ProductFilters>(this.defaultParams);

  constructor(private router: Router, private currentUser: CurrentUserService, private activatedRoute: ActivatedRoute) {
    this.activatedRoute.queryParams.subscribe(this.readFromUrlQueryParams);
  }

  // Handle URL updates
  private readFromUrlQueryParams = (params: Params): void => {
    const { page, sortBy, search, categoryID } = params;
    const showOnlyFavorites = !!params.favorites;
    const activeFacets = _pickBy(params, (_value, _key) => !this.nonFacetQueryParams.includes(_key));
    this.activeFiltersSubject.next({ page, sortBy, search, categoryID, showOnlyFavorites, activeFacets });
  };

  // Used to update the URL
  mapToUrlQueryParams(model: ProductFilters): Params {
    const { page, sortBy, search, showOnlyFavorites, activeFacets = {} } = model;
    activeFacets['categoryID'] = model.categoryID;
    activeFacets['favorites'] = showOnlyFavorites ? 'true' : undefined;
    return { page, sortBy, search, ...activeFacets };
  }

  // Used in requests to the OC API
  getOrderCloudParams(): any {
    const { page, sortBy, search, categoryID, showOnlyFavorites, activeFacets = {} } = this.activeFiltersSubject.value;
    const facets = _transform(activeFacets, (result, value, key: any) => (result[`xp.Facets.${key.toLocaleLowerCase()}`] = value), {});
    const favorites = this.currentUser.favoriteProductIDs.join('|') || undefined;
    return {
      categoryID,
      page,
      search,
      sortBy,
      filters: {
        ...facets,
        ID: showOnlyFavorites ? favorites : undefined,
      },
    };
  }

  private patchFilterState(patch: ProductFilters) {
    const activeFilters = { ...this.activeFiltersSubject.value, ...patch };
    const queryParams = this.mapToUrlQueryParams(activeFilters);
    this.router.navigate([], { queryParams }); // update url, which will call readFromUrlQueryParams()
  }

  toPage(pageNumber: number) {
    this.patchFilterState({ page: pageNumber || undefined });
  }

  sortBy(field: string) {
    this.patchFilterState({ sortBy: field || undefined, page: undefined });
  }

  searchBy(searchTerm: string) {
    this.patchFilterState({ search: searchTerm || undefined, page: undefined });
  }

  filterByFacet(field: string, value: string) {
    const activeFacets = this.activeFiltersSubject.value.activeFacets || {};
    activeFacets[field] = value || undefined;
    this.patchFilterState({ activeFacets, page: undefined });
  }

  filterByCategory(categoryID: string) {
    this.patchFilterState({ categoryID: categoryID || undefined, page: undefined });
  }

  filterByFavorites(showOnlyFavorites: boolean) {
    this.patchFilterState({ showOnlyFavorites, page: undefined });
  }

  clearSort() {
    this.sortBy(undefined);
  }

  clearSearch() {
    this.searchBy(undefined);
  }

  clearFacetFilter(field: string) {
    this.filterByFacet(field, undefined);
  }

  clearCategoryFilter() {
    this.filterByCategory(undefined);
  }

  clearAllFilters() {
    this.patchFilterState(this.defaultParams);
  }

  onFiltersChange(callback: (filters: ProductFilters) => void): void {
    // todo - is there a way to prevent duplicate subscriptions?
    this.activeFiltersSubject.subscribe(callback);
  }
}
