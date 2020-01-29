import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
@Component({
  selector: 'app-supplier-edit',
  templateUrl: './supplier-edit.component.html',
  styleUrls: ['./supplier-edit.component.scss'],
})
export class SupplierEditComponent {
  @Input()
  resourceForm: FormGroup;
  @Input()
  filterConfig;
  @Output()
  updateResource = new EventEmitter<any>();
  updateResourceFromEvent(event: any, field: string): void {
    field === "Active" ? this.updateResource.emit({ value: event.target.checked, field }) :
      this.updateResource.emit({ value: event.target.value, field });
  }
}