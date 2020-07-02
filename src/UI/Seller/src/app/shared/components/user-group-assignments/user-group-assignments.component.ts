import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { User, UserGroup, UserGroupAssignment, OcSupplierUserGroupService, OcSupplierUserService } from '@ordercloud/angular-sdk';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { IUserPermissionsService } from '@app-seller/shared/models/user-permissions.interface';
import { REDIRECT_TO_FIRST_PARENT } from '@app-seller/layout/header/header.config';
import { GetDisplayText } from './user-group-assignments.constants';
import { Router } from '@angular/router';

interface AssignmentsToAddUpdate {
  UserGroupType: string;
  Assignments: UserGroupAssignment[];
}

@Component({
  selector: 'user-group-assignments',
  templateUrl: './user-group-assignments.component.html',
  styleUrls: ['./user-group-assignments.component.scss'],
})
export class UserGroupAssignments implements OnChanges {
  @Input() user: User;
  @Input() userGroupType: string;
  @Input() isCreatingNew: boolean;
  @Input() userPermissionsService: IUserPermissionsService;
  @Input() homeCountry: string;
  @Output() assignmentsToAdd = new EventEmitter<AssignmentsToAddUpdate>();
  @Output() hasAssignments = new EventEmitter<boolean>();

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
  options = {filters: { 'xp.Type': ''}};
  displayText = '';
  readOnly = true;

  constructor(
    private router: Router
  ) {}
  
  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    this._userUserGroupAssignmentsEditable = [];
    this.updateForUserGroupAssignmentType();
    this.userOrgID = await this.userPermissionsService.getParentResourceID();
    await this.getUserGroups(this.userOrgID);
    if (changes.user?.currentValue.ID && !this.userID) {
      this.userID = this.user.ID
      if(this.userOrgID && this.userOrgID !== REDIRECT_TO_FIRST_PARENT){
        this.getUserGroupAssignments(this.user.ID, this.userOrgID);
      }
    }
    if (this.userID && changes.user?.currentValue?.ID !== changes.user?.previousValue?.ID) {
      this.userID = this.user.ID
      this.getUserGroupAssignments(this.user.ID, this.userOrgID);
    }
  }

  updateForUserGroupAssignmentType() {
    this.options.filters['xp.Type'] = this.userGroupType;
    this.displayText = GetDisplayText(this.userGroupType);
  }

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async getUserGroups(ID: string): Promise<void> {
    const groups = await this.userPermissionsService.getUserGroups(ID, this.options);
    const groupsInHomeCountry = groups.Items.filter(group => 
    this.isCreatingNew ? group.xp?.Country === this.homeCountry : group.xp?.Country === this.user.xp?.Country);
    this.userGroups = groupsInHomeCountry;
  }

  async getUserGroupAssignments(userID: any, userOrgID: any): Promise<void> {
    const userGroupAssignments = await this.userPermissionsService.listUserAssignments(userID, userOrgID);
    this._userUserGroupAssignmentsStatic = userGroupAssignments.Items;
    this._userUserGroupAssignmentsEditable = userGroupAssignments.Items;
    const match = this._userUserGroupAssignmentsStatic.some(assignedUG => this.userGroups.find(ug => ug.ID === assignedUG.UserGroupID));
    this.hasAssignments.emit(match);
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
      editableAssignment => !this._userUserGroupAssignmentsStatic.some(staticAssignment => staticAssignment.UserGroupID === editableAssignment.UserGroupID)
    );
    this.del = this._userUserGroupAssignmentsStatic.filter(
      staticAssignment => !this._userUserGroupAssignmentsEditable.some(editableAssignment => editableAssignment.UserGroupID === staticAssignment.UserGroupID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
    if (this.isCreatingNew) this.assignmentsToAdd.emit({ UserGroupType: this.userGroupType, Assignments: this.add});
  }

  discardUserUserGroupAssignmentChanges() {
    this._userUserGroupAssignmentsEditable = this._userUserGroupAssignmentsStatic;
    this.checkForUserUserGroupAssignmentChanges();
  }

  async executeUserUserGroupAssignmentRequests(): Promise<void> {
    this.requestedUserConfirmation = false;
    await this.userPermissionsService.updateUserUserGroupAssignments(this.userOrgID, this.add, this.del, this.userGroupType === "BuyerLocation");
    await this.getUserGroupAssignments(this.userID, this.userOrgID);
    this.checkForUserUserGroupAssignmentChanges();
  }
}
