import { Component, Input, Output, ViewChild, OnInit } from '@angular/core';
import {
  ListResource,
  ResourceCrudService,
  Options,
} from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { EventEmitter } from '@angular/core';
import { faFilter } from '@fortawesome/free-solid-svg-icons';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'resource-table-component',
  templateUrl: './resource-table.component.html',
  styleUrls: ['./resource-table.component.scss'],
})
export class ResourceTableComponent implements OnInit {
  @ViewChild('popover', { static: false })
  public popover: NgbPopover;
  faFilter = faFilter;
  searchTerm = '';

  @Input()
  resourceList: ListResource<any> = { Meta: {}, Items: [] };
  @Input()
  resourceOptions: Options;
  @Input()
  selectedResourceID: string;
  @Input()
  updatedResource: any;
  @Input()
  resourceInSelection: any;
  @Input()
  resourceName: string;
  @Input()
  ocService: ResourceCrudService<any>;
  @Output()
  searched: EventEmitter<any> = new EventEmitter();
  @Output()
  hitScrollEnd: EventEmitter<any> = new EventEmitter();
  @Output()
  changesSaved: EventEmitter<any> = new EventEmitter();
  @Output()
  resourceSelected: EventEmitter<any> = new EventEmitter();
  @Output()
  applyFilters: EventEmitter<any> = new EventEmitter();

  JSON = JSON;

  ngOnInit() {
    this.searchTerm = this.resourceOptions.search || '';
  }

  searchedResources(event) {
    this.searched.emit(event);
  }

  handleScrollEnd() {
    this.hitScrollEnd.emit(null);
  }

  handleSaveUpdates() {
    this.changesSaved.emit(null);
  }

  handleSelectResource(resource: any) {
    this.resourceSelected.emit(resource);
  }

  openPopover() {
    this.popover.open();
  }

  closePopover() {
    this.popover.close();
  }

  handleApplyFilters() {
    this.closePopover();
    this.applyFilters.emit(null);
  }

  clearAllFilters() {
    this.ocService.clearAllFilters();
  }
}
