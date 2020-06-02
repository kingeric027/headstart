import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { SupplierFilters } from '../../shopper-context';
import { OcSupplierService, ListSupplier } from '@ordercloud/angular-sdk';

export interface ISupplierFilters {
  activeFiltersSubject: BehaviorSubject<SupplierFilters>;
  toPage(pageNumber: number): void;
  sortBy(field: string): void;
  searchBy(searchTerm: string): void;
  clearSearch(): void;
  toSupplier(supplierID: string): void;
  clearAllFilters(): void;
  hasFilters(): boolean;
  filterByFields(filter: any): void;
}

// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
@Injectable({
  providedIn: 'root',
})
export class SupplierFilterService implements ISupplierFilters {
  private readonly nonFilterQueryParams = ['page', 'sortBy', 'search'];

  public activeFiltersSubject: BehaviorSubject<SupplierFilters> = new BehaviorSubject<SupplierFilters>(
    this.getDefaultParms()
  );

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
    const activeFilters = _pickBy(params, (_value, _key) => !this.nonFilterQueryParams.includes(_key));
    this.activeFiltersSubject.next({
      page,
      sortBy,
      search,
      supplierID,
      activeFilters,
    });
  }

  // Used to update the URL
  mapToUrlQueryParams(model: SupplierFilters): Params {
    const { page, sortBy, search, supplierID, activeFilters } = model;
    return { page, sortBy, search, supplierID, ...activeFilters };
  }

  async listSuppliers(): Promise<ListSupplier> {
    const { page, sortBy, search, supplierID, activeFilters } = this.activeFiltersSubject.value;
    return await this.ocSupplierService
      .List({
        page,
        search,
        sortBy,
        filters: this.createFilters(activeFilters, supplierID),
      })
      .toPromise();
  }

  private patchFilterState(patch: SupplierFilters): void {
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
      activeFilters: {},
    };
  }

  private createFilters(activeFilters: any, supplierID: string): any {
    const filters = _transform(
      activeFilters,
      (result, value, key: string) => (result[key.toLocaleLowerCase()] = value),
      {}
    );
    filters.ID = supplierID || undefined;
    return filters;
  }

  toSupplier(supplierID: string): void {
    this.patchFilterState({
      supplierID: supplierID || undefined,
      page: undefined,
    });
  }

  toPage(pageNumber: number): void {
    this.patchFilterState({ page: pageNumber || undefined });
  }

  sortBy(field: string): void {
    this.patchFilterState({ sortBy: field || undefined, page: undefined });
  }

  filterByFields(filter: any): void {
    const activeFilters = this.activeFiltersSubject.value.activeFilters || {};
    const newActiveFilters = { ...activeFilters, ...filter };
    this.patchFilterState({ activeFilters: newActiveFilters, page: undefined });
  }
  searchBy(searchTerm: string): void {
    this.patchFilterState({ search: searchTerm || undefined, page: undefined });
  }

  clearSort(): void {
    this.sortBy(undefined);
  }

  clearSearch(): void {
    this.searchBy(undefined);
  }

  clearAllFilters(): void {
    this.patchFilterState(this.getDefaultParms());
  }

  hasFilters(): boolean {
    const filters = this.activeFiltersSubject.value;
    return Object.entries(filters).some(([key, value]) => !!value);
  }
}
