import { Component, Input, Output, EventEmitter } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserGroupAssignment, User } from '@ordercloud/angular-sdk';
import { BuyerUserService } from '../buyer-user.service';
import { ValidateEmail } from '@app-seller/validators/validators';
@Component({
  selector: 'app-buyer-user-edit',
  templateUrl: './buyer-user-edit.component.html',
  styleUrls: ['./buyer-user-edit.component.scss'],
})
export class BuyerUserEditComponent {
  @Input()
  filterConfig;
  @Input()
  set resourceInSelection(buyerUser: User) {
    this.selectedResource = buyerUser;
    this.createBuyerUserForm(buyerUser);
  }
  @Output()
  updateResource = new EventEmitter<any>();
  @Output()
  userGroupAssignments = new EventEmitter<UserGroupAssignment[]>();
  isCreatingNew: boolean;
  resourceForm: FormGroup;
  selectedResource: User;
  constructor(public buyerUserService: BuyerUserService) {
    this.isCreatingNew = this.buyerUserService.checkIfCreatingNew();
  }
  createBuyerUserForm(user: User) {
    this.resourceForm = new FormGroup({
      Active: new FormControl(user.Active),
      Username: new FormControl(user.Username, Validators.required),
      FirstName: new FormControl(user.FirstName, Validators.required),
      LastName: new FormControl(user.LastName, Validators.required),
      Email: new FormControl(user.Email, [Validators.required, ValidateEmail]),
    });
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
