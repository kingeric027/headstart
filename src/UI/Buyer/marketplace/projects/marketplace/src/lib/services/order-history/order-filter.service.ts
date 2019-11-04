import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { OrderStatus, OrderFilters, IOrderFilters } from '../../shopper-context';
import { CurrentUserService } from '../current-user/current-user.service';
import { OcMeService, ListOrder } from '@ordercloud/angular-sdk';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class OrderFilterService implements IOrderFilters {
  activeOrderID: string; // TODO - make this read-only in components

  private activeFiltersSubject: BehaviorSubject<OrderFilters> = new BehaviorSubject<OrderFilters>(this.getDefaultParms());

  constructor(
    private ocMeService: OcMeService,
    private currentUser: CurrentUserService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
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
      page: undefined
    });
  }

  clearAllFilters(): void {
    this.patchFilterState(this.getDefaultParms());
  }

  onFiltersChange(callback: (filters: OrderFilters) => void): void {
    // todo - is there a way to prevent duplicate subscriptions?
    this.activeFiltersSubject.subscribe(callback);
  }

  private readFromUrlQueryParams = (params: Params): void => {
    const { page, sortBy, search, fromDate, toDate } = params;
    const status = params.status || OrderStatus.AllSubmitted;
    const showOnlyFavorites = !!params.favorites;
    this.activeFiltersSubject.next({ page, sortBy, search, showOnlyFavorites, status, fromDate, toDate });
  }

  private getDefaultParms() {
    // default params are grabbed through a function that returns an anonymous object to avoid pass by reference bugs
    console.log('using new function')
    return {
      page: undefined,
      sortBy: undefined,
      search: undefined,
      status: OrderStatus.AllSubmitted,
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
    const { page, sortBy, search, showOnlyFavorites, status, fromDate, toDate } = this.activeFiltersSubject.value;
    const from = fromDate ? `>${fromDate}` : undefined;
    const to = toDate ? `<${toDate}` : undefined;
    const favorites = this.currentUser.favoriteOrderIDs.join('|') || undefined;
    return await this.ocMeService.ListOrders({ page, search, sortBy,
      filters: {
        ID: showOnlyFavorites ? favorites : undefined,
        Status: status,
        DateSubmitted: [from, to].filter(x => x)
      },
    }).toPromise();
  }

  private patchFilterState(patch: OrderFilters) {
    const activeFilters = { ...this.activeFiltersSubject.value, ...patch };
    const queryParams = this.mapToUrlQueryParams(activeFilters);
    this.router.navigate([], { queryParams }); // update url, which will call readFromUrlQueryParams()
  }
}
