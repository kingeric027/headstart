import { OnInit, OnDestroy, ChangeDetectorRef, AfterContentInit, NgZone } from '@angular/core';
import { Meta } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';
import {
  ListResource,
  Options,
  ResourceCrudService,
  FilterDictionary,
} from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { FormGroup, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { singular } from 'pluralize';

export abstract class ResourceCrudComponent<ResourceType> implements OnInit, OnDestroy {
  alive = true;
  resourceList: ListResource<ResourceType> = { Meta: {}, Items: [] };
  resourceOptions: Options = {};

  // empty string if no resource is selected
  selectedResourceID = '';
  updatedResource = {};
  resourceInSelection = {};
  ocService: ResourceCrudService<ResourceType>;
  filterForm: FormGroup;
  filterConfig: any = {};
  router: Router;
  isCreatingNew: boolean;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    ocService: any,
    router: Router,
    private activatedRoute: ActivatedRoute,
    private ngZone: NgZone
  ) {
    this.ocService = ocService;
    this.router = router;
  }

  public navigate(url: string, options: any): void {
    /* 
    * Had a bug where clicking on a resource on the second page of resources was triggering an error
    * navigation trigger outside of Angular zone. Might be caused by inheritance or using 
    * changeDetector.detectChange, but couldn't resolve any other way
    * Please remove the need for this if you can
    * https://github.com/angular/angular/issues/25837
    */
    if (Object.keys(options)) {
      this.ngZone.run(() => this.router.navigate([url], options)).then();
    } else {
      this.ngZone.run(() => this.router.navigate([url])).then();
    }
  }

  ngOnInit() {
    this.setFilterForm();
    this.subscribeToResources();
    this.subscribeToOptions();
    this.subscribeToResourceSelection();
  }

  subscribeToResources() {
    this.ocService.resourceSubject.pipe(takeWhile(() => this.alive)).subscribe((resourceList) => {
      this.resourceList = resourceList;
      this.changeDetectorRef.detectChanges();
    });
  }

  subscribeToOptions() {
    this.ocService.optionsSubject.pipe(takeWhile(() => this.alive)).subscribe((options) => {
      this.resourceOptions = options;
      this.setFilterForm();
      this.changeDetectorRef.detectChanges();
    });
  }

  subscribeToResourceSelection() {
    this.activatedRoute.params.subscribe((params) => {
      this.setIsCreatingNew();
      const resourceIDSelected =
        params[`${singular(this.ocService.secondaryResourceLevel || this.ocService.primaryResourceLevel)}ID`];
      if (resourceIDSelected) {
        this.setResourceSelection(resourceIDSelected);
      }
      if (this.isCreatingNew) {
        this.setResoureObjectsForCreatingNew();
      }
    });
  }

  private setIsCreatingNew() {
    const routeUrl = this.router.routerState.snapshot.url;
    const endUrl = routeUrl.slice(routeUrl.length - 4, routeUrl.length);
    this.isCreatingNew = endUrl === '/new';
  }

  handleScrollEnd() {
    if (this.resourceList.Meta.TotalPages > this.resourceList.Meta.Page) {
      this.ocService.getNextPage();
    }
  }

  searchResources(searchStr: string) {
    this.ocService.searchBy(searchStr);
  }

  async setResourceSelection(resourceID: string) {
    this.selectedResourceID = resourceID || '';
    const resource = await this.ocService.findOrGetResourceByID(resourceID);
    this.resourceInSelection = this.copyResource(resource);
    this.updatedResource = this.copyResource(resource);
    this.changeDetectorRef.detectChanges();
  }

  setResoureObjectsForCreatingNew() {
    this.resourceInSelection = {};
    this.updatedResource = {};
  }

  selectResource(resource: any) {
    const [newURL, queryParams] = this.ocService.constructNewRouteInformation(resource.ID || '');
    this.navigate(newURL, { queryParams });
  }

  updateResource(fieldName: string, event) {
    const newValue = event.target.value;
    this.updatedResource = { ...this.updatedResource, [fieldName]: newValue };
    this.changeDetectorRef.detectChanges();
  }

  copyResource(resource: any) {
    return JSON.parse(JSON.stringify(resource));
  }

  async saveUpdates() {
    if (this.isCreatingNew) {
      this.createNewResource();
    } else {
      this.updateExitingResource();
    }
  }

  async deleteResource() {
    await this.ocService.deleteResource(this.selectedResourceID);
    this.selectResource({});
  }

  discardChanges() {
    this.updatedResource = this.resourceInSelection;
    this.changeDetectorRef.detectChanges();
  }

  async updateExitingResource() {
    const updatedResource = await this.ocService.updateResource(this.updatedResource);
    this.resourceInSelection = this.copyResource(updatedResource);
    this.updatedResource = this.copyResource(updatedResource);
  }

  async createNewResource() {
    const newResource = await this.ocService.createNewResource(this.updatedResource);
    this.selectResource(newResource);
  }

  applyFilters() {
    this.ocService.addFilters(this.removeFieldsWithNoValue(this.filterForm.value));
  }

  removeFieldsWithNoValue(formValues: FilterDictionary) {
    const values = { ...formValues };
    Object.entries(values).forEach(([key, value]) => {
      if (!value) {
        delete values[key];
      }
    });
    return values;
  }

  setFilterForm() {
    const formGroup = {};
    if (this.filterConfig && this.filterConfig.Filters) {
      this.filterConfig.Filters.forEach((filter) => {
        const value = (this.resourceOptions.filters && this.resourceOptions.filters[filter.Path]) || '';
        formGroup[filter.Path] = new FormControl(value);
      });
      this.filterForm = new FormGroup(formGroup);
    }
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
