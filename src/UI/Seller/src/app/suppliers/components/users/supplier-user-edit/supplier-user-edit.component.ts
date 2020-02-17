import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
@Component({
  selector: 'app-supplier-user-edit',
  templateUrl: './supplier-user-edit.component.html',
  styleUrls: ['./supplier-user-edit.component.scss'],
})
export class SupplierUserEditComponent {
  @Input()
  resourceForm: FormGroup;
  @Input()
  filterConfig;
  @Input()
  resourceInSelection;
  @Input()
  supplierID;
  @Output()
  updateResource = new EventEmitter<any>();
  updateResourceFromEvent(event: any, field: string): void {
    field === 'Active'
      ? this.updateResource.emit({ value: event.target.checked, field })
      : this.updateResource.emit({ value: event.target.value, field });
  }
}
