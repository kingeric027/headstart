import { Component, Input, Output, EventEmitter } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { UserGroupAssignment } from '@ordercloud/angular-sdk';
import { BuyerUserService } from '../buyer-user.service';
@Component({
  selector: 'app-buyer-user-edit',
  templateUrl: './buyer-user-edit.component.html',
  styleUrls: ['./buyer-user-edit.component.scss'],
})
export class BuyerUserEditComponent {
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
  constructor(public buyerUserService: BuyerUserService) {
    this.isCreatingNew = this.buyerUserService.checkIfCreatingNew();
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
