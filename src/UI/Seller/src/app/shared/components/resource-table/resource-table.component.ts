import { Component, Input, Output } from '@angular/core';
import { ListResource } from '@app-seller/shared/services/resource-crud/resource-crud.service';
import { EventEmitter } from '@angular/core';

@Component({
  selector: 'resource-table-component',
  templateUrl: './resource-table.component.html',
  styleUrls: ['./resource-table.component.scss'],
})
export class ResourceTableComponent {
  @Input()
  resourceList: ListResource<any> = { Meta: {}, Items: [] };
  @Input()
  searchText: string;
  @Input()
  selectedResourceID: string;
  @Input()
  updatedResource: any;
  @Input()
  resourceInSelection: any;
  @Output()
  searched: EventEmitter<any> = new EventEmitter();
  @Output()
  hitScrollEnd: EventEmitter<any> = new EventEmitter();
  @Output()
  changesSaved: EventEmitter<any> = new EventEmitter();

  searchedResources(event) {
    this.searched.emit(this.searchText);
  }

  handleScrollEnd() {
    this.hitScrollEnd.emit(null);
  }

  handleSaveUpdates() {
    this.changesSaved.emit(null);
  }
}
