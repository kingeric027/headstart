import { Component, OnInit } from '@angular/core';
import {
  ShopperContextService,
  UserGroup,
  User,
  UserGroupAssignment,
  OcUserGroupService,
  ApprovalRule,
  OcApprovalRuleService,
} from 'marketplace';
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
  currentApprovalRule: ApprovalRule = null;
  currentLocationApprovalThresholdStatic = 0;
  currentLocationApprovalThresholdEditable = 0;
  areAllUsersAssignedToNeedsApproval = false;

  constructor(
    private context: ShopperContextService,
    private ocOcUserGroupService: OcUserGroupService,
    private ocApprovalRuleService: OcApprovalRuleService
  ) {}

  ngOnInit() {
    this.fetchUserManagementInformation();
  }

  toggleAllNeedingApproval(): void {
    if (this.areAllUsersAssignedToNeedsApproval) {
      this.setNeedApprovalForNoUsers();
    } else {
      this.setNeedApprovalForAllUsers();
    }
  }

  checkIfAllUsersAreAssignedToNeedsApproval(): void {
    this.areAllUsersAssignedToNeedsApproval = this.users.every(u =>
      this.approvalAssignmentsEditable.some(a => a.UserID === u.ID && a.UserGroupID.includes('NeedsApproval'))
    );
  }

  setNeedApprovalForAllUsers(): void {
    this.setNeedApprovalForNoUsers();
    this.approvalAssignmentsEditable = [
      ...this.approvalAssignmentsEditable,
      ...this.users.map(u => {
        return { UserID: u.ID, UserGroupID: `${this.currentLocation.ID}-NeedsApproval` };
      }),
    ];
    this.checkForChanges();
  }

  setNeedApprovalForNoUsers(): void {
    this.approvalAssignmentsEditable = this.approvalAssignmentsEditable.filter(
      c => !c.UserGroupID.includes('NeedsApproval')
    );
    this.checkForChanges();
  }

  async fetchUserManagementInformation(): Promise<void> {
    this.locations = await this.context.userManagementService.getLocations();
    if (this.locations.length) {
      this.currentLocation = this.locations[0];
      this.users = await this.context.userManagementService.getLocationUsers(this.currentLocation.ID);
      await this.updateAssignments();
      const currentApprovalRule = await this.context.userManagementService.getLocationApprovalRule(
        this.currentLocation.ID
      );
      this.setApprovalRuleValues(currentApprovalRule);
    }
  }

  setApprovalRuleValues(approvalRule: ApprovalRule): void {
    this.currentApprovalRule = approvalRule;
    this.currentLocationApprovalThresholdStatic = Number(this.currentApprovalRule.RuleExpression.split('>')[1]);
    this.currentLocationApprovalThresholdEditable = this.currentLocationApprovalThresholdStatic;
    this.checkIfAllUsersAreAssignedToNeedsApproval();
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

  setThreshold(value: number): void {
    this.currentLocationApprovalThresholdEditable = value;
  }

  setThresholdFromEvent(event: any): void {
    this.currentLocationApprovalThresholdEditable = Number(event.target.value);
  }

  async saveNewThreshold(): Promise<void> {
    const buyerID = this.currentLocation.ID.split('-')[0];
    const newRuleExpression = `${this.currentApprovalRule.RuleExpression.split('>')[0]}>${
      this.currentLocationApprovalThresholdEditable
    }`;
    const newApprovalRule = await this.ocApprovalRuleService
      .Patch(buyerID, this.currentApprovalRule.ID, { RuleExpression: newRuleExpression })
      .toPromise();
    this.setApprovalRuleValues(newApprovalRule);
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
    this.checkIfAllUsersAreAssignedToNeedsApproval();
  }

  discardUserUserGroupAssignmentChanges(): void {
    this.approvalAssignmentsEditable = this.approvalAssignmentsStatic;
    this.checkForChanges();
  }

  requestUserConfirmation(): void {
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