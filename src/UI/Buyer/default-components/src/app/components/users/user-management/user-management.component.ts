import { Component, OnInit } from '@angular/core';
import { ShopperContextService, UserGroup, User, UserGroupAssignment, OcUserGroupService } from 'marketplace';
import { ngModuleJitUrl } from '@angular/compiler';

@Component({
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
})
export class OCMUserManagement implements OnInit {
  locations: UserGroup[] = [];
  users: User[] = [];
  approvalAssignmentsStatic: UserGroupAssignment[] = [];
  approvalAssignmentsEditable: UserGroupAssignment[] = [];
  add: UserGroupAssignment[] = [];
  del: UserGroupAssignment[] = [];
  areChanges = false;
  requestedUserConfirmation = false;
  currentLocation: UserGroup = null;

  constructor(private context: ShopperContextService, private ocOcUserGroupService: OcUserGroupService) {}

  ngOnInit() {
    this.fetchUserManagementInformation();
  }

  async fetchUserManagementInformation(): Promise<void> {
    this.locations = await this.context.userManagementService.getLocations();
    this.currentLocation = this.locations[0];
    this.users = await this.context.userManagementService.getLocationUsers(this.currentLocation.ID);
    await this.updateAssignments();
  }

  async updateAssignments(): Promise<void> {
    const approverAssignments = await this.context.userManagementService.getLocationApproverAssignments(
      this.currentLocation.ID
    );
    const needsApprovalAssignments = await this.context.userManagementService.getLocationNeedsApprovalAssignments(
      this.currentLocation.ID
    );
    this.approvalAssignmentsStatic = [...approverAssignments, ...needsApprovalAssignments];
    this.approvalAssignmentsEditable = [...this.approvalAssignmentsStatic];
    this.checkForChanges();
  }

  isAssigned(userID: string, assignmentType: string): boolean {
    return this.approvalAssignmentsEditable.some(n => n.UserID === userID && n.UserGroupID.includes(assignmentType));
  }

  toggleAssignment(userID: string, assignmentType: string): void {
    if (this.isAssigned(userID, assignmentType)) {
      this.approvalAssignmentsEditable = this.approvalAssignmentsEditable.filter(
        n => !(n.UserID === userID && n.UserGroupID.includes(assignmentType))
      );
    } else {
      this.approvalAssignmentsEditable = [
        ...this.approvalAssignmentsEditable,
        { UserID: userID, UserGroupID: `${this.currentLocation.ID}-${assignmentType}` },
      ];
    }
    this.checkForChanges();
  }

  checkForChanges(): void {
    this.add = this.approvalAssignmentsEditable.filter(
      editableAssignment =>
        !this.approvalAssignmentsStatic.some(
          staticAssignment =>
            staticAssignment.UserID === editableAssignment.UserID &&
            staticAssignment.UserGroupID === editableAssignment.UserGroupID
        )
    );
    this.del = this.approvalAssignmentsStatic.filter(
      staticAssignment =>
        !this.approvalAssignmentsEditable.some(
          editableAssignment =>
            staticAssignment.UserID === editableAssignment.UserID &&
            staticAssignment.UserGroupID === editableAssignment.UserGroupID
        )
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
  }

  discardUserUserGroupAssignmentChanges() {
    this.approvalAssignmentsEditable = this.approvalAssignmentsStatic;
    this.checkForChanges();
  }

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async executeUserUserGroupAssignmentRequests(): Promise<void> {
    const buyerID = this.currentLocation.ID.split('-')[0];
    const assignmentRequests = [
      this.add.map(a => this.ocOcUserGroupService.SaveUserAssignment(buyerID, a).toPromise()),
      this.del.map(d => this.ocOcUserGroupService.DeleteUserAssignment(buyerID, d.UserGroupID, d.UserID).toPromise()),
    ];
    await Promise.all(assignmentRequests);
    this.approvalAssignmentsStatic = this.approvalAssignmentsEditable;
    this.requestedUserConfirmation = false;
    this.checkForChanges();
  }
}
