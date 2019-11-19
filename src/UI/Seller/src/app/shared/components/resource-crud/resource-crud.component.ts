import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Meta } from '@ordercloud/angular-sdk';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
interface ListResource {
  Meta: Meta;
  Items: any[];
}

export abstract class ResourceCrudComponent implements OnInit, OnDestroy {
  alive = true;
  resourceList: ListResource = { Meta: {}, Items: [] };
  searchText: string = null;

  // empty string if no resource is selected
  selectedResourceID = '';
  updatedResource = {};
  resourceInSelection = {};
  JSON = JSON;
  ocService: any = {};

  constructor(private changeDetectorRef: ChangeDetectorRef) {}

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
    this.ocService.getNextPage();
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
