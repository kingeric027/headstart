import { BehaviorSubject } from 'rxjs';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { transform as _transform, pickBy as _pickBy } from 'lodash';
import { cloneDeep as _cloneDeep, uniqBy as _uniqBy } from 'lodash';
import { Meta } from '@ordercloud/angular-sdk';

export interface Options {
  page?: number;
  sortBy?: string;
  search?: string;
  filters?: FilterDictionary;
}

export interface FilterDictionary {
  [filterKey: string]: string;
}

export interface ListResource<ResourceType> {
  Meta: Meta;
  Items: ResourceType[];
}

export abstract class ResourceCrudService<ResourceType> {
  public resourceSubject: BehaviorSubject<ListResource<ResourceType>> = new BehaviorSubject<ListResource<ResourceType>>(
    { Meta: {}, Items: [] }
  );
  public optionsSubject: BehaviorSubject<Options> = new BehaviorSubject<Options>({});
  private itemsPerPage = 100;

  route = '';
  primaryResourceLevel = '';
  // example: for supplier user service the primary is supplier and the secondary is users
  secondaryResourceLevel = '';

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private ocService: any,
    route: string,
    primaryResourceLevel: string,
    secondaryResourceLevel: string = ''
  ) {
    this.route = route;
    this.primaryResourceLevel = primaryResourceLevel;
    this.secondaryResourceLevel = secondaryResourceLevel;
    this.activatedRoute.queryParams.subscribe((params) => {
      if (this.router.url.startsWith(this.route)) {
        this.readFromUrlQueryParams(params);
      } else {
        this.optionsSubject.next({});
      }
    });
    this.optionsSubject.subscribe((value) => {
      this.listResources();
    });
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

  async listResources(pageNumber = 1) {
    if (this.shouldListResources()) {
      const { sortBy, search, filters } = this.optionsSubject.value;
      const options = {
        page: pageNumber,
        search,
        sortBy,
        pageSize: this.itemsPerPage,
        filters,
      };
      const resourceResponse = await this.ocService.List(...this.buildListArgs(options)).toPromise();
      if (pageNumber === 1) {
        this.setNewResources(resourceResponse);
      } else {
        this.addResources(resourceResponse);
      }
    }
  }

  shouldListResources() {
    if (!this.secondaryResourceLevel) {
      return this.router.url.startsWith(this.route);
    } else {
      return !!this.getParentResourceID();
    }
  }

  selectResource(resource: any) {
    const newUrl = this.constructResourceURL(resource.ID || '');
    this.router.navigateByUrl(newUrl);
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
    // update for pluralize
    return `${this.primaryResourceLevel}s/${resource.ID}/${this.secondaryResourceLevel}s`;
  }

  constructResourceURL(resourceID: string = ''): string {
    let newUrl = '';
    if (this.secondaryResourceLevel) {
      newUrl += `${this.route}/${this.getParentResourceID()}/${this.secondaryResourceLevel}s`;
    } else {
      newUrl += `${this.route}`;
    }
    if (resourceID) {
      newUrl += `/${resourceID}`;
    }
    return newUrl;
  }

  buildListArgs(options: Options) {
    if (this.secondaryResourceLevel) {
      const parentResourceID = this.getParentResourceID();
      return [parentResourceID, options];
    } else {
      return [options];
    }
  }

  getParentResourceID() {
    const urlPieces = this.router.url.split('/');
    const indexOfParent = urlPieces.indexOf(`${this.primaryResourceLevel}s`);
    return urlPieces[indexOfParent + 1];
  }

  getResourceById(resourceID: string): Promise<any> {
    return this.ocService.Get(resourceID).toPromise();
  }

  async updateResource(resource: any): Promise<any> {
    const newResource = await this.ocService.Save(resource.ID, resource).toPromise();
    const resourceIndex = this.resourceSubject.value.Items.findIndex((i: any) => i.ID === newResource.ID);
    this.resourceSubject.value.Items[resourceIndex] = newResource;
    this.resourceSubject.next(this.resourceSubject.value);
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
    filtersToRemove.forEach((filter) => {
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
    return `/${resourceName}s`;
  }

  hasFilters(): boolean {
    const filters = this.optionsSubject.value;
    return Object.entries(filters).some(([key, value]) => {
      return !!value;
    });
  }
}
