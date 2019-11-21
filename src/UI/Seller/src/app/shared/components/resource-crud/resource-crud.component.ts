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

export abstract class ResourceCrudComponent<ResourceType> implements OnInit, OnDestroy {
  alive = true;
  resourceList: ListResource<ResourceType> = { Meta: {}, Items: [] };
  resourceOptions: Options = {};
  searchText = '';

  // empty string if no resource is selected
  selectedResourceID = '';
  updatedResource = {};
  resourceInSelection = {};
  ocService: ResourceCrudService<ResourceType>;
  filterForm: FormGroup;
  filterConfig: any = {};

  constructor(private changeDetectorRef: ChangeDetectorRef, ocService: any) {
    this.ocService = ocService;
  }

  ngOnInit() {
    this.setFilterForm();
    this.subscribeToResources();
    this.subscribeToOptions();
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

  handleScrollEnd() {
    if (this.resourceList.Meta.TotalPages > this.resourceList.Meta.Page) {
      this.ocService.getNextPage();
    }
  }

  searchResources(searchStr: string) {
    this.searchText = searchStr;
    this.ocService.searchBy(searchStr);
  }

  selectResource(resource: any) {
    this.selectedResourceID = resource.ID;
    this.resourceInSelection = this.copyResource(resource);
    this.updatedResource = this.copyResource(resource);
    this.changeDetectorRef.detectChanges();
  }

  updateResource(fieldName: string, event) {
    const newValue = event.target.value;
    this.updatedResource[fieldName] = newValue;
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

  ngOnDestroy() {
    this.alive = false;
  }
}
