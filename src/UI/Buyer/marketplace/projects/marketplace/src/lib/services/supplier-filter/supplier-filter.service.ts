import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { SupplierFilters, ISupplierFilters } from '../../shopper-context';
import { OcSupplierService, ListSupplier } from '@ordercloud/angular-sdk';
import { filter } from 'rxjs/operators';
import { cloneDeep as _cloneDeep } from 'lodash';

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class SupplierFilterService implements ISupplierFilters {

  public activeFiltersSubject: BehaviorSubject<SupplierFilters> = new BehaviorSubject<SupplierFilters>(this.getDefaultParms());

  constructor(
    private router: Router,
    private ocSupplierService: OcSupplierService,
    private activatedRoute: ActivatedRoute
  ) {
    this.activatedRoute.queryParams.subscribe(params => {
      if (this.router.url.startsWith('/suppliers')) {
        this.readFromUrlQueryParams(params);
      } else {
        this.activeFiltersSubject.next(this.getDefaultParms());
      }
    });
  }

  // Handle URL updates
  private readFromUrlQueryParams(params: Params): void {
    const { page, sortBy, search, supplierID } = params;
    this.activeFiltersSubject.next({ page, sortBy, search, supplierID });
  }

  // Used to update the URL
  mapToUrlQueryParams(model: SupplierFilters): Params {
    const { page, sortBy, search, supplierID } = model;
    return {page, sortBy, search, supplierID };
  }

  async listSuppliers(): Promise<ListSupplier> {
    const { page, sortBy, search, supplierID } = this.activeFiltersSubject.value;
    return await this.ocSupplierService.List({
      page,
      search,
      sortBy,
      filters: { ID: supplierID || undefined },
    }).toPromise();
  }

  private patchFilterState(patch: SupplierFilters) {
    const activeFilters = { ...this.activeFiltersSubject.value, ...patch };
    const queryParams = this.mapToUrlQueryParams(activeFilters);
    this.router.navigate([], { queryParams }); // update url, which will call readFromUrlQueryParams()
  }

  private getDefaultParms(): SupplierFilters {
    // default params are grabbed through a function that returns an anonymous object to avoid pass by reference bugs
    return {
      supplierID: undefined,
      page: undefined,
      sortBy: undefined,
      search: undefined,
    };
  }

  toSupplier(supplierID: string) {
    this.patchFilterState({ supplierID: supplierID || undefined, page: undefined });
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
    const filters = this.activeFiltersSubject.value;
    return Object.entries(filters).some(([key, value]) => !!value);
  }
}
