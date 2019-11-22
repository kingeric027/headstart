import { OnInit, OnDestroy, ChangeDetectorRef, AfterContentInit } from '@angular/core';
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

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    ocService: any,
    router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    this.ocService = ocService;
    this.router = router;
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
      const resourceIDSelected =
        params[`${this.ocService.secondaryResourceLevel || this.ocService.primaryResourceLevel}ID`];
      if (resourceIDSelected) {
        this.setResourceSelection(
          params[`${this.ocService.secondaryResourceLevel || this.ocService.primaryResourceLevel}ID`]
        );
      }
    });
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
    const resource = await this.findOrGetResource(resourceID);
    this.resourceInSelection = this.copyResource(resource);
    this.updatedResource = this.copyResource(resource);
    this.changeDetectorRef.detectChanges();
  }

  async findOrGetResource(resourceID: string) {
    const resourceInList = this.resourceList.Items.find((i) => (i as any).ID === resourceID);
    if (resourceInList) {
      return resourceInList;
    } else {
      return await this.ocService.getResourceById(resourceID);
    }
  }

  selectResource(resource: any) {
    this.ocService.selectResource(resource);
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
    const updatedResource = this.ocService.updateResource(this.updatedResource);
    this.resourceInSelection = this.copyResource(updatedResource);
    this.updatedResource = this.copyResource(updatedResource);
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

  navigateToSubList(subList: string) {
    const newUrl = `${this.ocService.primaryResourceLevel}s/${this.selectedResourceID}/${subList}s`;
    this.router.navigateByUrl(newUrl);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
