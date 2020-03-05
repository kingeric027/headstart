import { Component, Input, Output, EventEmitter } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { UserGroupAssignment } from '@ordercloud/angular-sdk';
import { SupplierUserService } from '../supplier-user.service';
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
  @Output()
  updateResource = new EventEmitter<any>();
  @Output()
  userGroupAssignments = new EventEmitter<UserGroupAssignment[]>();
  isCreatingNew: boolean;
  constructor(public supplierUserService: SupplierUserService) {
    this.isCreatingNew = this.supplierUserService.checkIfCreatingNew();
  }
  updateResourceFromEvent(event: any, field: string): void {
    field === 'Active'
      ? this.updateResource.emit({ value: event.target.checked, field })
      : this.updateResource.emit({ value: event.target.value, field });
  }
  addUserGroupAssignments(event): void {
    this.userGroupAssignments.emit(event);
  }
}
