import { OnInit, OnDestroy, ChangeDetectorRef, AfterContentInit } from '@angular/core';
import { Meta } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';
import { ListResource } from '@app-seller/shared/services/resource-crud/resource-crud.service';

export abstract class ResourceCrudComponent<ResourceType> implements OnInit, OnDestroy {
  alive = true;
  resourceList: ListResource<ResourceType> = { Meta: {}, Items: [] };
  searchText: string = null;

  // empty string if no resource is selected
  selectedResourceID = '';
  updatedResource = {};
  resourceInSelection = {};
  JSON = JSON;
  ocService: any = {};

  constructor(private changeDetectorRef: ChangeDetectorRef, ocService: any) {
    this.ocService = ocService;
  }

  ngOnInit() {
    this.subscribeToResources();
    this.setFilters();
  }

  subscribeToResources() {
    this.ocService.resourceSubject.pipe(takeWhile(() => this.alive)).subscribe((resourceList) => {
      this.resourceList = resourceList;
      this.changeDetectorRef.detectChanges();
    });
  }

  handleScrollEnd() {
    if (this.resourceList.Meta.TotalPages > this.resourceList.Meta.Page) {
      this.ocService.getNextPage();
    }
  }

  setFilters() {
    this.searchText = this.ocService.filterSubject.value.search;
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

  saveUpdates() {
    this.ocService.updateResource(this.updatedResource);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
