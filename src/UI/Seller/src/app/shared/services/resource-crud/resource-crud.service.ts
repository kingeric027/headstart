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

  // route defintes the string of text that the front end needs to match to make list calls
  abstract route = '';

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private ocService: any) {
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
    if (this.router.url.startsWith(this.route)) {
      const { sortBy, search, filters } = this.optionsSubject.value;
      const productsResponse = await this.ocService
        .List({
          page: pageNumber,
          search,
          sortBy,
          pageSize: this.itemsPerPage,
          filters,
        })
        .toPromise();
      if (pageNumber === 1) {
        this.setNewResources(productsResponse);
      } else {
        this.addResources(productsResponse);
      }
    }
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

  hasFilters(): boolean {
    const filters = this.optionsSubject.value;
    return Object.entries(filters).some(([key, value]) => {
      return !!value;
    });
  }
}
