import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { OrderStatus, OrderFilters } from '../../shopper-context';
import { CurrentUserService } from '../current-user/current-user.service';
import { OcMeService, ListOrder } from '@ordercloud/angular-sdk';
import { filter } from 'rxjs/operators';

export interface IOrderFilters {
  activeFiltersSubject: BehaviorSubject<OrderFilters>;
  toPage(pageNumber: number): void;
  sortBy(field: string): void;
  searchBy(searchTerm: string): void;
  clearSearch(): void;
  filterByFavorites(showOnlyFavorites: boolean): void;
  filterByStatus(status: OrderStatus): void;
  filterByDateSubmitted(fromDate: string, toDate: string): void;
  clearAllFilters(): void;
}

@Injectable({
  providedIn: 'root',
})
export class OrderFilterService implements IOrderFilters {
  activeOrderID: string; // TODO - make this read-only in components

  public activeFiltersSubject: BehaviorSubject<OrderFilters> = new BehaviorSubject<OrderFilters>(
    this.getDefaultParms()
  );

  constructor(
    private ocMeService: OcMeService,
    private currentUser: CurrentUserService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) {
    this.activatedRoute.queryParams
      .pipe(filter(() => this.router.url.startsWith('/profile/orders')))
      .subscribe(this.readFromUrlQueryParams);
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

  clearSearch(): void {
    this.patchFilterState({ search: undefined });
  }

  filterByFavorites(showOnlyFavorites: boolean): void {
    this.patchFilterState({ showOnlyFavorites, page: undefined });
  }

  filterByStatus(status: OrderStatus): void {
    this.patchFilterState({ status: status || undefined, page: undefined });
  }

  filterByDateSubmitted(fromDate: string, toDate: string): void {
    this.patchFilterState({
      fromDate: fromDate || undefined,
      toDate: toDate || undefined,
    });
  }

  clearAllFilters(): void {
    this.patchFilterState(this.getDefaultParms());
  }

  private readFromUrlQueryParams = (params: Params): void => {
    const { page, sortBy, search, fromDate, toDate } = params;
    const status = params.status;
    const showOnlyFavorites = !!params.favorites;
    this.activeFiltersSubject.next({ page, sortBy, search, showOnlyFavorites, status, fromDate, toDate });
  };

  private getDefaultParms() {
    // default params are grabbed through a function that returns an anonymous object to avoid pass by reference bugs
    return {
      page: undefined,
      sortBy: undefined,
      search: undefined,
      status: undefined,
      showOnlyFavorites: false,
      fromDate: undefined,
      toDate: undefined,
    };
  }

  // Used to update the URL
  mapToUrlQueryParams(model: OrderFilters): Params {
    const { page, sortBy, search, status, fromDate, toDate } = model;
    const favorites = model.showOnlyFavorites ? 'true' : undefined;
    return { page, sortBy, search, status, favorites, fromDate, toDate };
  }

  // Used in requests to the OC API
  async listOrders(): Promise<ListOrder> {
    return await this.ocMeService.ListOrders(this.createListOptions()).toPromise();
  }

  async listApprovableOrders(): Promise<ListOrder> {
    return await this.ocMeService.ListApprovableOrders(this.createListOptions()).toPromise();
  }

  private createListOptions() {
    const { page, sortBy, search, showOnlyFavorites, status, fromDate, toDate } = this.activeFiltersSubject.value;
    const from = fromDate ? `>${fromDate}` : undefined;
    const to = toDate ? `<${toDate}` : undefined;
    const favorites = this.currentUser.get().FavoriteOrderIDs.join('|') || undefined;
    return {
      page,
      search,
      sortBy,
      filters: {
        ID: showOnlyFavorites ? favorites : undefined,
        Status: status,
        DateSubmitted: [from, to].filter(x => x),
      },
    };
  }

  private patchFilterState(patch: OrderFilters) {
    if (this.isNewDateFilter(patch, this.activeFiltersSubject.value)) patch.page = undefined;
    const activeFilters = { ...this.activeFiltersSubject.value, ...patch };
    const queryParams = this.mapToUrlQueryParams(activeFilters);
    this.router.navigate([], { queryParams }); // update url, which will call readFromUrlQueryParams()
  }

  private isNewDateFilter(patch: OrderFilters, activeFilters: OrderFilters): boolean {
    let isNewDateFilter = false;
    if (Object.keys(patch).includes('toDate') || Object.keys(patch).includes('fromDate')) {
      isNewDateFilter = patch.toDate !== activeFilters.toDate || patch.fromDate !== activeFilters.fromDate;
    }
    return isNewDateFilter;
  }
}
