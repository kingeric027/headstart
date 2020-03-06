import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { User, UserGroup, UserGroupAssignment, OcSupplierUserGroupService, OcSupplierUserService } from '@ordercloud/angular-sdk';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { IUserPermissionsService } from '@app-seller/shared/models/user-permissions.interface';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';

@Component({
  selector: 'user-permissions-assignments-component',
  templateUrl: './user-permissions-assignments.component.html',
  styleUrls: ['./user-permissions-assignments.component.scss'],
})
export class UserPermissionsAssignments implements OnChanges {
  @Input() user: User;
  @Input() isCreatingNew: boolean;
  @Input() userPermissionsService: IUserPermissionsService;
  @Output() assignmentsToAdd = new EventEmitter<UserGroupAssignment[]>();

  userOrgID: string;
  userID: string;
  userGroups: UserGroup[];
  add: UserGroupAssignment[];
  del: UserGroupAssignment[];
  _userUserGroupAssignmentsStatic: UserGroupAssignment[] = [];
  _userUserGroupAssignmentsEditable: UserGroupAssignment[] = [];
  areChanges = false;
  requestedUserConfirmation = false;
  faExclamationCircle = faExclamationCircle;

  private readonly options = {
    filters: {
      'xp.Type': 'UserPermissions'
    }
  };

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.user?.currentValue.ID && !this.userID) {
      this.userOrgID = this.userPermissionsService.getParentResourceID();
      this.userOrgID !== REDIRECT_TO_FIRST_PARENT && this.getUserGroups(this.userOrgID);
      this.userID = this.user.ID
      this.getUserGroupAssignments(this.user.ID, this.userOrgID);
    }
    if (this.userID && changes.user?.currentValue?.ID !== changes.user?.previousValue?.ID) {
      this.getUserGroupAssignments(this.user.ID, this.userOrgID);
    }
  }

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async getUserGroups(ID: string): Promise<void> {
    const groups = await this.userPermissionsService.getUserGroups(ID, this.options);
    this.userGroups = groups.Items;
  }

  async getUserGroupAssignments(userID: any, userOrgID: any): Promise<void> {
    const userGroupAssignments = await this.userPermissionsService.listUserAssignments(userID, userOrgID);
    this._userUserGroupAssignmentsStatic = userGroupAssignments.Items;
    this._userUserGroupAssignmentsEditable = userGroupAssignments.Items;
  }

  toggleUserUserGroupAssignment(userGroup: UserGroup): void {
    if (this.isAssigned(userGroup)) {
      this._userUserGroupAssignmentsEditable = this._userUserGroupAssignmentsEditable.filter(
        groupAssignment => groupAssignment.UserGroupID !== userGroup.ID
      );
    } else {
      const newUserUserGroupAssignment = {
        UserID: this.userID,
        UserGroupID: userGroup.ID
      };
      this._userUserGroupAssignmentsEditable = [
        ...this._userUserGroupAssignmentsEditable,
        newUserUserGroupAssignment,
      ];
    }
    this.checkForUserUserGroupAssignmentChanges();
  }

  addUserUserGroupAssignment(userGroup: UserGroup): void {
    if (this.isAssigned(userGroup)) {
      this._userUserGroupAssignmentsEditable = this._userUserGroupAssignmentsEditable.filter(
        groupAssignment => groupAssignment.UserGroupID !== userGroup.ID
      );
    } else {
      const newUserUserGroupAssignment = {
        UserID: 'PENDING',
        UserGroupID: userGroup.ID
      };
      this._userUserGroupAssignmentsEditable = [
        ...this._userUserGroupAssignmentsEditable,
        newUserUserGroupAssignment,
      ];
    }
    this.checkForUserUserGroupAssignmentChanges();
  }

  isAssigned(userGroup: UserGroup) {
    return (
      this._userUserGroupAssignmentsEditable?.some(
        groupAssignment => groupAssignment.UserGroupID === userGroup.ID
      )
    );
  }

  checkForUserUserGroupAssignmentChanges() {
    this.add = this._userUserGroupAssignmentsEditable.filter(
      assignment => !JSON.stringify(this._userUserGroupAssignmentsStatic).includes(assignment.UserGroupID)
    );
    this.del = this._userUserGroupAssignmentsStatic.filter(
      assignment => !JSON.stringify(this._userUserGroupAssignmentsEditable).includes(assignment.UserGroupID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
    if (this.isCreatingNew) this.assignmentsToAdd.emit(this.add);
  }

  discardUserUserGroupAssignmentChanges() {
    this._userUserGroupAssignmentsEditable = this._userUserGroupAssignmentsStatic;
    this.checkForUserUserGroupAssignmentChanges();
  }

  async executeUserUserGroupAssignmentRequests(): Promise<void> {
    this.requestedUserConfirmation = false;
    await this.userPermissionsService.updateUserUserGroupAssignments(this.userOrgID, this.add, this.del);
    await this.getUserGroupAssignments(this.userID, this.userOrgID);
    this.checkForUserUserGroupAssignmentChanges();
  }
}
