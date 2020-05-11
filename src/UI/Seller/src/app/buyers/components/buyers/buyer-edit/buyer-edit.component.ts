import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
@Component({
  selector: 'app-buyer-edit',
  templateUrl: './buyer-edit.component.html',
  styleUrls: ['./buyer-edit.component.scss'],
})
export class BuyerEditComponent {
  @Input()
  resourceForm: FormGroup;
  @Input()
  filterConfig;
  @Output()
  updateResource = new EventEmitter<any>();
  updateResourceFromEvent(event: any, field: string): void {
    const value = field === 'Active' ? event.target.checked : event.target.value;
    this.updateResource.emit({ value, field });
  }
}
