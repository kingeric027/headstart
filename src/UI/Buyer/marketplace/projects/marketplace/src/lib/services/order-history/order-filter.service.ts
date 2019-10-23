import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { OrderStatus, OrderFilters, IOrderFilters } from '../../shopper-context';
import { CurrentUserService } from '../current-user/current-user.service';
import { OcMeService, ListOrder } from '@ordercloud/angular-sdk';

@Injectable({
  providedIn: 'root',
})
export class OrderFilterService implements IOrderFilters {
  activeOrderID: string; // TODO - make this read-only in components

  private readonly defaultParams = {
    page: undefined,
    sortBy: undefined,
    search: undefined,
    status: OrderStatus.AllSubmitted,
    showOnlyFavorites: false,
    fromDate: undefined,
    toDate: undefined,
  };
  private activeFiltersSubject: BehaviorSubject<OrderFilters> = new BehaviorSubject<OrderFilters>(this.defaultParams);

  constructor(
    private ocMeService: OcMeService,
    private currentUser: CurrentUserService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) {
    this.activatedRoute.queryParams.subscribe(this.readFromUrlQueryParams);
  }

  toPage(pageNumber: number): void {
    this.patchFilterState({ page: pageNumber || undefined });
  }

  sortBy(field: string): void {
    this.patchFilterState({ sortBy: field || undefined, page: undefined });

  }

  clearSort(): void {
    this.sortBy(undefined);
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

  filterByFromDate(fromDate: Date): void {

  }

  filterByToDate(toDate: Date): void {

  }

  clearAllFilters(): void {
    this.patchFilterState(this.defaultParams);
  }

  onFiltersChange(callback: (filters: OrderFilters) => void): void {
    // todo - is there a way to prevent duplicate subscriptions?
    this.activeFiltersSubject.subscribe(callback);
  }

  private readFromUrlQueryParams = (params: Params): void => {
    const { page, sortBy, search, status, fromDate, toDate  } = params;
    const showOnlyFavorites = !!params.favorites;
    this.activeFiltersSubject.next({ page, sortBy, search, showOnlyFavorites, fromDate, toDate, status });
  }

  // Used to update the URL
  mapToUrlQueryParams(model: OrderFilters): Params {
    const { page, sortBy, search, showOnlyFavorites } = model;
    const favorites = showOnlyFavorites ? 'true' : undefined;
    return { page, sortBy, search, favorites };
  }

  // Used in requests to the OC API
  async listOrders(): Promise<ListOrder> {
    const { page, sortBy, search, showOnlyFavorites, fromDate, toDate } = this.activeFiltersSubject.value;
    const favorites = this.currentUser.favoriteOrderIDs.join('|') || undefined;
    return await this.ocMeService.ListOrders({
      page,
      search,
      sortBy,
      filters: {
        ID: showOnlyFavorites ? favorites : undefined,
      },
    }).toPromise();
  }

  private patchFilterState(patch: OrderFilters) {
    const activeFilters = { ...this.activeFiltersSubject.value, ...patch };
    const queryParams = this.mapToUrlQueryParams(activeFilters);
    this.router.navigate([], { queryParams }); // update url, which will call readFromUrlQueryParams()
  }
}
