import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { OrderStatus, OrderFilters, OrderViewContext, AppConfig } from '../../shopper-context';
import { CurrentUserService } from '../current-user/current-user.service';
import { OcMeService, ListOrder, OcOrderService, OcTokenService } from '@ordercloud/angular-sdk';
import { filter } from 'rxjs/operators';
import { RouteService } from '../route/route.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

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
    private ocOrderService: OcOrderService,
    private currentUser: CurrentUserService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private routeService: RouteService,

    // remove below when sdk is regenerated
    private ocTokenService: OcTokenService,
    private httpClient: HttpClient,
    private appConfig: AppConfig
  ) {
    this.activatedRoute.queryParams
      .pipe(filter(() => this.router.url.startsWith('/orders')))
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

  filterByLocation(locationID: string): void {
    this.patchFilterState({ location: locationID || undefined, page: undefined });
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
    const { page, sortBy, search, fromDate, toDate, location } = params;
    const status = params.status;
    const showOnlyFavorites = !!params.favorites;
    this.activeFiltersSubject.next({ page, sortBy, search, showOnlyFavorites, status, fromDate, toDate, location });
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
      location: undefined,
      toDate: undefined,
    };
  }

  // Used to update the URL
  mapToUrlQueryParams(model: OrderFilters): Params {
    const { page, sortBy, search, status, fromDate, toDate, location } = model;
    const favorites = model.showOnlyFavorites ? 'true' : undefined;
    return { page, sortBy, search, status, favorites, fromDate, toDate, location };
  }

  // Used in requests to the OC API
  async listOrders(): Promise<ListOrder> {
    const viewContext = this.routeService.getOrderViewContext();
    switch (viewContext) {
      case OrderViewContext.MyOrders:
        return await this.ocMeService.ListOrders(this.createListOptions()).toPromise();
      case OrderViewContext.Approve:
        return await this.ocMeService.ListApprovableOrders(this.createListOptions()).toPromise();
      case OrderViewContext.Location:
        // enforcing a location is selected before filtering
        if (this.activeFiltersSubject.value.location) return await this.ListLocationOrders();
    }
  }

  async ListLocationOrders(): Promise<ListOrder> {
    const locationID = this.activeFiltersSubject.value.location;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.ocTokenService.GetAccess()}`,
    });
    const url = `${this.appConfig.middlewareUrl}/order/location/${locationID}`;
    return this.httpClient
      .get<ListOrder>(url, { headers: headers })
      .toPromise();
  }

  async listApprovableOrders(): Promise<ListOrder> {
    return await this.ocMeService.ListApprovableOrders(this.createListOptions()).toPromise();
  }

  private createListOptions() {
    const {
      page,
      sortBy,
      search,
      showOnlyFavorites,
      status,
      fromDate,
      toDate,
      location,
    } = this.activeFiltersSubject.value;
    const from = fromDate ? `>${fromDate}` : undefined;
    const to = toDate ? `<${toDate}` : undefined;
    const favorites = this.currentUser.get().FavoriteOrderIDs.join('|') || undefined;
    let listOptions = {
      page,
      search,
      sortBy,
      filters: {
        ID: showOnlyFavorites ? favorites : undefined,
        DateSubmitted: [from, to].filter(x => x),
      },
    };
    listOptions = this.addStatusFilters(status, listOptions);
    if (location) {
      listOptions.filters['BillingAddress.ID'] = location;
    }
    return listOptions;
  }

  private addStatusFilters(status: string, listOptions: any): any {
    if (status === OrderStatus.ChangesRequested) {
      listOptions.filters.DateDeclined = '*';
    } else {
      listOptions.filters.Status = status;
    }
    return listOptions;
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
