import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep } from 'lodash';
import { OcProductService, ListProduct, Product, Meta } from '@ordercloud/angular-sdk';

interface Filters {
  page?: number;
  sortBy?: string;
  search?: string;
}

interface ProductListEditMeta {
  items: Product[];
  meta: Meta;
  filters: Filters;
  selectedIndex: number;
}

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class ProductService {
  public productSubject: BehaviorSubject<ProductListEditMeta> = new BehaviorSubject<ProductListEditMeta>(
    this.getDefaultProductListEditMeta()
  );
  private itemsPerPage = 100;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private ocProductsService: OcProductService
  ) {
    this.activatedRoute.queryParams.subscribe((params) => {
      if (this.router.url.startsWith('/products')) {
        this.readFromUrlQueryParams(params);
      } else {
        this.productSubject.next(this.getDefaultProductListEditMeta());
      }
    });
  }

  // Handle URL updates
  private readFromUrlQueryParams(params: Params): void {
    const { sortBy, search, categoryID } = params;
    const showOnlyFavorites = !!params.favorites;
    this.productSubject.next({ ...this.productSubject.value, filters: { sortBy, search } });
  }

  // Used to update the URL
  mapToUrlQueryParams(model: Filters): Params {
    const { sortBy, search } = model;
    return { sortBy, search };
  }

  async listProducts(pageNumber = 1) {
    const { sortBy, search } = this.productSubject.value.filters;
    const productsResponse = await this.ocProductsService
      .List({
        page: pageNumber,
        search,
        sortBy,
        pageSize: this.itemsPerPage,
      })
      .toPromise();
    if (pageNumber === 1) {
      this.setNewProducts(productsResponse);
    } else {
      this.addProducts(productsResponse);
    }
  }

  setNewProducts(productsResponse: ListProduct) {
    this.productSubject.next({
      ...this.productSubject.value,
      items: productsResponse.Items,
      meta: productsResponse.Meta,
    });
  }

  addProducts(productsResponse: ListProduct) {
    this.productSubject.next({
      ...this.productSubject.value,
      items: [...this.productSubject.value.items, ...productsResponse.Items],
      meta: productsResponse.Meta,
    });
  }

  getNextPage() {
    this.listProducts(this.productSubject.value.filters.page + 1);
  }

  private defaultParams() {
    // default params are grabbed through a function that returns an anonymous object to avoid pass by reference bugs
    return {
      page: undefined,
      sortBy: undefined,
      search: undefined,
    };
  }

  private getDefaultProductListEditMeta(): ProductListEditMeta {
    // default params are grabbed through a function that returns an anonymous object to avoid pass by reference bugs
    return {
      items: [],
      meta: {},
      filters: this.defaultParams(),
      selectedIndex: -1,
    };
  }

  sortBy(field: string) {
    this.patchFilterState({ sortBy: field || undefined, page: undefined });
  }

  searchBy(searchTerm: string) {
    this.patchFilterState({ search: searchTerm || undefined, page: undefined });
  }

  clearSort() {
    this.sortBy(undefined);
  }

  clearSearch() {
    this.searchBy(undefined);
  }

  clearAllFilters() {
    this.patchFilterState(this.getDefaultParms());
  }

  hasFilters(): boolean {
    const filters = this.filterSubject.value;
    return Object.entries(filters).some(([key, value]) => {
      return !!value;
    });
  }
}
