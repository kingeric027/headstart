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

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export abstract class ProductService {
  public resourceSubject: BehaviorSubject<ListProduct> = new BehaviorSubject<ListProduct>({ Meta: {}, Items: [] });
  public filterSubject: BehaviorSubject<Filters> = new BehaviorSubject<Filters>(this.getDefaultParms());
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
        this.filterSubject.next(this.getDefaultParms());
      }
    });
    this.filterSubject.subscribe((value) => {
      this.listProducts();
    });
  }

  // Handle URL updates
  private readFromUrlQueryParams(params: Params): void {
    const { sortBy, search } = params;
    console.log('read from url query params');
    this.filterSubject.next({ sortBy, search });
  }

  // Used to update the URL
  mapToUrlQueryParams(model: Filters): Params {
    const { sortBy, search } = model;
    return { sortBy, search };
  }

  async listProducts(pageNumber = 1) {
    const { sortBy, search } = this.filterSubject.value;
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

  async updateResource(resource: any) {
    const newResource = await this.ocProductsService.Save(resource.ID, resource).toPromise();
    const resourceIndex = this.resourceSubject.value.Items.findIndex((i) => i.ID === newResource.ID);
    this.resourceSubject.value.Items[resourceIndex] = newResource;
    this.resourceSubject.next(this.resourceSubject.value);
  }

  setNewProducts(productsResponse: ListProduct) {
    this.resourceSubject.next(productsResponse);
  }

  addProducts(productsResponse: ListProduct) {
    this.resourceSubject.next({
      Meta: productsResponse.Meta,
      Items: [...this.resourceSubject.value.Items, ...productsResponse.Items],
    });
  }

  getNextPage() {
    if (this.resourceSubject.value.Meta && this.resourceSubject.value.Meta.Page) {
      this.listProducts(this.resourceSubject.value.Meta.Page + 1);
    }
  }

  private patchFilterState(patch: Filters) {
    const activeFilters = { ...this.filterSubject.value, ...patch };
    const queryParams = this.mapToUrlQueryParams(activeFilters);
    this.router.navigate([], { queryParams }); // update url, which will call readFromUrlQueryParams()
  }

  private getDefaultParms() {
    // default params are grabbed through a function that returns an anonymous object to avoid pass by reference bugs
    return {
      page: undefined,
      sortBy: undefined,
      search: undefined,
    };
  }

  toPage(pageNumber: number) {
    this.patchFilterState({ page: pageNumber || undefined });
  }

  sortBy(field: string) {
    this.patchFilterState({ sortBy: field || undefined });
  }

  searchBy(searchTerm: string) {
    this.patchFilterState({ search: searchTerm || undefined });
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
