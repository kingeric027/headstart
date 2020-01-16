import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute, NavigationEnd } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep, uniqBy as _uniqBy } from 'lodash';
import { Meta } from '@ordercloud/angular-sdk';
import { filter, takeWhile } from 'rxjs/operators';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import {
  ListResource,
  Options,
  FilterDictionary,
  RequestStatus,
  SUCCESSFUL_WITH_ITEMS,
  ERROR,
  GETTING_NEW_ITEMS,
  FETCHING_SUBSEQUENT_PAGES,
  REFRESHING_ITEMS,
  SUCCESSFUL_NO_ITEMS_WITH_FILTERS,
  SUCCESSFUL_NO_ITEMS_NO_FILTERS,
} from './resource-crud.types';

export abstract class ResourceCrudService<ResourceType> {
  public resourceSubject: BehaviorSubject<ListResource<ResourceType>> = new BehaviorSubject<ListResource<ResourceType>>(
    { Meta: {}, Items: [] }
  );
  public resourceRequestStatus: BehaviorSubject<RequestStatus> = new BehaviorSubject<RequestStatus>(GETTING_NEW_ITEMS);
  public optionsSubject: BehaviorSubject<Options> = new BehaviorSubject<Options>({});
  private itemsPerPage = 100;

  route = '';
  primaryResourceLevel = '';
  // example: for supplier user service the primary is supplier and the secondary is users
  secondaryResourceLevel = '';
  subResourceList: string[];
  emptyResource: any = {};

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private ocService: any,
    route: string,
    primaryResourceLevel: string,
    subResourceList: string[] = [],
    secondaryResourceLevel: string = ''
  ) {
    this.route = route;
    this.primaryResourceLevel = primaryResourceLevel;
    this.secondaryResourceLevel = secondaryResourceLevel;
    this.subResourceList = subResourceList;

    this.activatedRoute.queryParams.subscribe(params => {
      // this prevents service from reading from query params when not on the route related to the service
      if (this.isOnRelatedRoute()) {
        this.readFromUrlQueryParams(params);
      } else {
        this.optionsSubject.next({});
      }
    });
    this.optionsSubject.subscribe(value => {
      if (this.getParentResourceID() !== REDIRECT_TO_FIRST_PARENT) {
        this.listResources();
      }
    });
  }
  async getMyResource(): Promise<any> {
    console.log('get my resource for this resource not defined');
  }

  private isOnRelatedRoute(): boolean {
    const isOnSubResource =
      this.subResourceList &&
      this.subResourceList.some(subResource => {
        return this.router.url.includes(`/${subResource}`);
      });
    const isOnBaseRoute = this.router.url.includes(this.route);
    const isOnRelatedSubResource = this.router.url.includes(`/${this.secondaryResourceLevel}`);
    if (!isOnBaseRoute) {
      return false;
    } else if (isOnSubResource && this.secondaryResourceLevel && isOnRelatedSubResource) {
      return true;
    } else if (!isOnSubResource && !this.secondaryResourceLevel) {
      return true;
    } else {
      return false;
    }
  }

  // Handle URL updates
  private readFromUrlQueryParams(params: Params): void {
    const { sortBy, search, ...filters } = params;
    this.optionsSubject.next({ sortBy, search, filters });
  }

  // Used to update the URL
  mapToUrlQueryParams(options: Options): Params {
    const { sortBy, search, filters } = options;
    return { sortBy, search, ...filters };
  }

  async listResources(pageNumber = 1, searchText = '') {
    if (this.shouldListResources()) {
      const { sortBy, search, filters } = this.optionsSubject.value;
      const options = {
        page: pageNumber,
        // allows a list call to pass in a search term that will not appear in the query params
        search: searchText || search,
        sortBy,
        pageSize: this.itemsPerPage,
        filters,
      };
      const resourceResponse = await this.listWithStatusIndicator(options);
      if (pageNumber === 1) {
        this.setNewResources(resourceResponse);
      } else {
        this.addResources(resourceResponse);
      }
    }
  }

  private async listWithStatusIndicator(options: Options): Promise<ListResource<ResourceType>> {
    try {
      this.resourceRequestStatus.next(this.getFetchStatus(options));
      const resourceResponse = await this.ocService.List(...this.createListArgs([options])).toPromise();
      this.resourceRequestStatus.next(this.getSucessStatus(options, resourceResponse));
      return resourceResponse;
    } catch (error) {
      this.resourceRequestStatus.next(ERROR);
      throw error;
    }
  }

  getFetchStatus(options: Options) {
    const isSubsequentPage = options.page > 1;
    const areCurrentlyItems = this.resourceSubject.value.Items.length;
    // will not want to show a loading indicator in certain situations so this
    // differentiates between refreshes and new lists
    // when filters are applied REFRESHING_ITEMS will be returned
    if (!areCurrentlyItems && !isSubsequentPage) {
      return GETTING_NEW_ITEMS;
    }
    if (!isSubsequentPage && areCurrentlyItems) {
      return REFRESHING_ITEMS;
    }
    if (isSubsequentPage && areCurrentlyItems) {
      return FETCHING_SUBSEQUENT_PAGES;
    }
    // return isSubsequentPage || !areCurrentlyItems ? GETTING_NEW_ITEMS : REFRESHING_ITEMS;
  }

  getSucessStatus(options: Options, resourceResponse: ListResource<ResourceType>) {
    const areFilters = this.areFiltersOnOptions(options);
    const areItems = !!resourceResponse.Items.length;
    if (areItems) return SUCCESSFUL_WITH_ITEMS;
    return areFilters ? SUCCESSFUL_NO_ITEMS_WITH_FILTERS : SUCCESSFUL_NO_ITEMS_NO_FILTERS;
  }

  shouldListResources() {
    if (!this.secondaryResourceLevel) {
      // for primary resources list if on the route
      return this.router.url.startsWith(this.route);
    } else {
      // for secondary resources list there is a parent ID
      return !!this.getParentResourceID() && this.router.url.includes(this.secondaryResourceLevel);
    }
  }

  constructResourceURLs(resourceID: string = ''): string[] {
    const newUrlPieces = [];
    newUrlPieces.push(this.route);
    if (this.secondaryResourceLevel) {
      newUrlPieces.push(`/${this.getParentResourceID()}`);
      newUrlPieces.push(`/${this.secondaryResourceLevel}`);
    }
    if (resourceID) {
      newUrlPieces.push(`/${resourceID}`);
    }
    return newUrlPieces;
  }

  selectParentResource(resource: any) {
    const newUrl = this.updateUrlForUpdatedParent(resource);
    this.router.navigateByUrl(newUrl);

    // this settimeout ensures that the new parent resource ID is in the url before the resources are listed
    // find a better way to update the resources on the parent resource ID change
    setTimeout(() => {
      this.listResources();
    });
  }

  updateUrlForUpdatedParent(resource: any) {
    const queryParams = this.router.url.split('?')[1];
    let newUrl = `${this.primaryResourceLevel}/${resource.ID}/${this.secondaryResourceLevel}`;
    if (queryParams) {
      newUrl += `?${queryParams}`;
    }
    return newUrl;
  }

  constructNewRouteInformation(resourceID: string = ''): any[] {
    let newUrl = '';
    const queryParams = this.activatedRoute.snapshot.queryParams;

    if (this.secondaryResourceLevel) {
      newUrl += `${this.route}/${this.getParentResourceID()}/${this.secondaryResourceLevel}`;
    } else {
      newUrl += `${this.route}`;
    }
    if (resourceID) {
      newUrl += `/${resourceID}`;
    }
    return [newUrl, queryParams];
  }

  getParentResourceID() {
    const urlPieces = this.router.url.split('/');
    const indexOfParent = urlPieces.indexOf(`${this.primaryResourceLevel}`);
    return urlPieces[indexOfParent + 1];
  }

  getResourceById(resourceID: string): Promise<any> {
    return this.ocService.Get(...this.createListArgs([resourceID])).toPromise();
  }

  createListArgs(options: any[]) {
    /* ordercloud services follow a patter where the paramters to a function (Save, Create, List)
      are the nearly the same for all resource. However, sub resources (supplier users, buyer payment methods, etc...)
      have the parent resource ID as the first paramter before the expected argument
    */
    if (this.primaryResourceLevel === 'orders') {
      // placeholder conditional for getting the supplier order list page running
      // will need to integrate this with the filter on the order list page as a seller
      // user and potentially refactor later
      return ['Incoming', ...options];
    }
    if (this.secondaryResourceLevel) {
      const parentResourceID = this.getParentResourceID();
      return [parentResourceID, ...options];
    } else {
      return [...options];
    }
  }

  async findOrGetResourceByID(resourceID: string): Promise<any> {
    const resourceInList = this.resourceSubject.value.Items.find(i => (i as any).ID === resourceID);
    if (resourceInList) {
      return resourceInList;
    } else {
      if (resourceID !== REDIRECT_TO_FIRST_PARENT) {
        return await this.getResourceById(resourceID);
      }
    }
  }

  async updateResource(resource: any): Promise<any> {
    const newResource = await this.ocService.Save(...this.createListArgs([resource.ID, resource])).toPromise();
    const resourceIndex = this.resourceSubject.value.Items.findIndex((i: any) => i.ID === newResource.ID);
    this.resourceSubject.value.Items[resourceIndex] = newResource;
    this.resourceSubject.next(this.resourceSubject.value);
    return newResource;
  }

  async deleteResource(resourceID: string): Promise<null> {
    const newResource = await this.ocService.Delete(...this.createListArgs([resourceID])).toPromise();
    this.resourceSubject.value.Items = this.resourceSubject.value.Items.filter((i: any) => i.ID !== resourceID);
    this.resourceSubject.next(this.resourceSubject.value);
    return;
  }

  async createNewResource(resource: any): Promise<any> {
    const newResource = await this.ocService.Create(...this.createListArgs([resource])).toPromise();
    this.resourceSubject.value.Items = [...this.resourceSubject.value.Items, newResource];
    this.resourceSubject.next(this.resourceSubject.value);
    return newResource;
  }

  setNewResources(resourceResponse: ListResource<ResourceType>) {
    this.resourceSubject.next(resourceResponse);
  }

  addResources(resourceResponse: ListResource<ResourceType>) {
    this.resourceSubject.next({
      Meta: resourceResponse.Meta,
      Items: [...this.resourceSubject.value.Items, ...resourceResponse.Items],
    });
  }

  getNextPage() {
    if (this.resourceSubject.value.Meta && this.resourceSubject.value.Meta.Page) {
      this.listResources(this.resourceSubject.value.Meta.Page + 1);
    }
  }

  private patchFilterState(patch: Options) {
    const activeOptions = { ...this.optionsSubject.value, ...patch };
    const queryParams = this.mapToUrlQueryParams(activeOptions);
    this.router.navigate([], { queryParams }); // update url, which will call readFromUrlQueryParams()
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

  addFilters(newFilters: FilterDictionary) {
    const newFilterDictionary = { ...this.optionsSubject.value.filters, ...newFilters };
    this.patchFilterState({ filters: newFilterDictionary });
  }

  removeFilters(filtersToRemove: string[]) {
    const newFilterDictionary = { ...this.optionsSubject.value.filters };
    filtersToRemove.forEach(filter => {
      if (newFilterDictionary[filter]) {
        delete newFilterDictionary[filter];
      }
    });
    this.patchFilterState({ filters: newFilterDictionary });
  }

  clearSort() {
    this.sortBy(undefined);
  }

  clearSearch() {
    this.searchBy(undefined);
  }

  clearAllFilters() {
    this.patchFilterState({ filters: {} });
  }

  clearResources() {
    this.resourceSubject.next({ Meta: {}, Items: [] });
  }

  getRouteFromResourceName(resourceName: string): string {
    return `/${resourceName}`;
  }

  hasFilters(): boolean {
    const filters = this.optionsSubject.value;
    return Object.entries(filters).some(([key, value]) => {
      return !!value;
    });
  }

  areFiltersOnOptions(options: Options): boolean {
    return (
      !!options.search ||
      (options.filters &&
        Object.entries(options.filters).some(([key, value]) => {
          return !!value;
        }))
    );
  }
}
