import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { CurrentUserService } from '../current-user/current-user.service';
import { ProductFilters } from '../../shopper-context';
import { OcMeService, ListProduct } from '@ordercloud/angular-sdk';
import { ProductCategoriesService } from '../product-categories/product-categories.service';

export interface IProductFilters {
  activeFiltersSubject: BehaviorSubject<ProductFilters>;
  toPage(pageNumber: number): void;
  sortBy(field: string): void;
  clearSort(): void;
  searchBy(searchTerm: string): void;
  clearSearch(): void;
  filterByFacet(field: string, value: string, isFacet?: boolean): void;
  clearFacetFilter(field: string): void;
  filterByCategory(categoryID: string): void;
  clearCategoryFilter(): void;
  filterByFavorites(showOnlyFavorites: boolean): void;
  clearAllFilters(): void;
  hasFilters(): boolean;
}

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class ProductFilterService implements IProductFilters {
  // TODO - allow app devs to filter by custom xp that is not a facet. Create functions for this.
  private readonly nonFacetQueryParams = ['page', 'sortBy', 'categoryID', 'search', 'favorites'];

  public activeFiltersSubject: BehaviorSubject<ProductFilters> = new BehaviorSubject<ProductFilters>(
    this.getDefaultParms()
  );

  constructor(
    private router: Router,
    private ocMeService: OcMeService,
    private currentUser: CurrentUserService,
    private activatedRoute: ActivatedRoute,
    private categories: ProductCategoriesService
  ) {
    this.activatedRoute.queryParams.subscribe(params => {
      if (this.router.url.startsWith('/products')) {
        this.readFromUrlQueryParams(params);
      } else {
        this.activeFiltersSubject.next(this.getDefaultParms());
      }
    });
  }

  // Handle URL updates
  private readFromUrlQueryParams(params: Params): void {
    const { page, sortBy, search, categoryID } = params;
    this.categories.setActiveCategoryID(categoryID);
    const showOnlyFavorites = !!params.favorites;
    const activeFacets = _pickBy(params, (_value, _key) => !this.nonFacetQueryParams.includes(_key));
    this.activeFiltersSubject.next({ page, sortBy, search, categoryID, showOnlyFavorites, activeFacets });
  }

  // Used to update the URL
  mapToUrlQueryParams(model: ProductFilters): Params {
    const { page, sortBy, search, showOnlyFavorites, activeFacets = {} } = model;
    activeFacets.categoryID = model.categoryID;
    activeFacets.favorites = showOnlyFavorites ? 'true' : undefined;
    return { page, sortBy, search, ...activeFacets };
  }

  async listProducts(): Promise<ListProduct> {
    const { page, sortBy, search, categoryID, showOnlyFavorites, activeFacets = {} } = this.activeFiltersSubject.value;
    const facets = _transform(
      activeFacets,
      (result, value, key: any) => (result[`xp.Facets.${key.toLocaleLowerCase()}`] = value),
      {}
    );
    const favorites = this.currentUser.get().FavoriteProductIDs.join('|') || undefined;
    return await this.ocMeService
      .ListProducts({
        categoryID,
        page,
        search,
        sortBy,
        filters: {
          ...facets,
          ID: showOnlyFavorites ? favorites : undefined,
        },
      })
      .toPromise();
  }

  private patchFilterState(patch: ProductFilters): void {
    const activeFilters = { ...this.activeFiltersSubject.value, ...patch };
    const queryParams = this.mapToUrlQueryParams(activeFilters);
    this.router.navigate([], { queryParams }); // update url, which will call readFromUrlQueryParams()
  }

  private getDefaultParms(): ProductFilters {
    // default params are grabbed through a function that returns an anonymous object to avoid pass by reference bugs
    return {
      page: undefined,
      sortBy: undefined,
      search: undefined,
      categoryID: undefined,
      showOnlyFavorites: false,
      activeFacets: {},
    };
  }

  toPage(pageNumber: number): void {
    this.patchFilterState({ page: pageNumber || undefined });
  }

  sortBy(field: string): void {
    this.patchFilterState({ sortBy: field || undefined, page: undefined });
  }

  searchBy(searchTerm: string): void {
    this.patchFilterState({ search: searchTerm || undefined, page: undefined });
  }

  filterByFacet(field: string, value: string): void {
    const activeFacets = this.activeFiltersSubject.value.activeFacets || {};
    activeFacets[field] = value || undefined;
    this.patchFilterState({ activeFacets, page: undefined });
  }

  filterByCategory(categoryID: string): void {
    this.patchFilterState({ categoryID: categoryID || undefined, page: undefined });
  }

  filterByFavorites(showOnlyFavorites: boolean): void {
    this.patchFilterState({ showOnlyFavorites, page: undefined });
  }

  clearSort(): void {
    this.sortBy(undefined);
  }

  clearSearch(): void {
    this.searchBy(undefined);
  }

  clearFacetFilter(field: string): void {
    this.filterByFacet(field, undefined);
  }

  clearCategoryFilter(): void {
    this.filterByCategory(undefined);
  }

  clearAllFilters(): void {
    this.patchFilterState(this.getDefaultParms());
  }

  hasFilters(): boolean {
    const filters = this.activeFiltersSubject.value;
    return Object.entries(filters).some(([key, value]) => {
      if (key === 'activeFacets') {
        return Object.keys(value).length;
      } else {
        return !!value;
      }
    });
  }
}
