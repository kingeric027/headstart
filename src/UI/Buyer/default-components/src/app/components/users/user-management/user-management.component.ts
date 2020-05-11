import { Component, OnInit, Input } from '@angular/core';
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
import { PromiseType } from 'protractor/built/plugins';

@Component({
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
})
export class OCMUserManagement {
  users: User[] = [];
  permissionAssignmentsStatic: UserGroupAssignment[] = [];
  permissionAssignmentsEditable: UserGroupAssignment[] = [];
  add: UserGroupAssignment[] = [];
  del: UserGroupAssignment[] = [];
  areChanges = false;
  requestedUserConfirmation = false;
  currentLocation: UserGroup = null;
  currentApprovalRule: ApprovalRule = null;
  currentLocationApprovalThresholdStatic = 0;
  currentLocationApprovalThresholdEditable = 0;
  areAllUsersAssignedToNeedsApproval = false;
  _locationID = '';

  @Input() set locationID(value: string) {
    this._locationID = value;
    this.fetchUserManagementInformation();
  }

  constructor(
    private context: ShopperContextService,
    private ocOcUserGroupService: OcUserGroupService,
    private ocApprovalRuleService: OcApprovalRuleService
  ) {}

  toggleAllNeedingApproval(): void {
    if (this.areAllUsersAssignedToNeedsApproval) {
      this.setNeedApprovalForNoUsers();
    } else {
      this.setNeedApprovalForAllUsers();
    }
  }

  checkIfAllUsersAreAssignedToNeedsApproval(): void {
    this.areAllUsersAssignedToNeedsApproval = this.users.every(u =>
      this.permissionAssignmentsEditable.some(a => a.UserID === u.ID && a.UserGroupID.includes('NeedsApproval'))
    );
  }

  setNeedApprovalForAllUsers(): void {
    this.setNeedApprovalForNoUsers();
    this.permissionAssignmentsEditable = [
      ...this.permissionAssignmentsEditable,
      ...this.users.map(u => {
        return { UserID: u.ID, UserGroupID: `${this._locationID}-NeedsApproval` };
      }),
    ];
    this.checkForChanges();
  }

  setNeedApprovalForNoUsers(): void {
    this.permissionAssignmentsEditable = this.permissionAssignmentsEditable.filter(
      c => !c.UserGroupID.includes('NeedsApproval')
    );
    this.checkForChanges();
  }

  async fetchUserManagementInformation(): Promise<void> {
    this.users = await this.context.userManagementService.getLocationUsers(this._locationID);
    await this.updateAssignments();
    const currentApprovalRule = await this.context.userManagementService.getLocationApprovalRule(this._locationID);
    this.setApprovalRuleValues(currentApprovalRule);
  }

  setApprovalRuleValues(approvalRule: ApprovalRule): void {
    this.currentApprovalRule = approvalRule;
    this.currentLocationApprovalThresholdStatic = Number(this.currentApprovalRule.RuleExpression.split('>')[1]);
    this.currentLocationApprovalThresholdEditable = this.currentLocationApprovalThresholdStatic;
    this.checkIfAllUsersAreAssignedToNeedsApproval();
  }

  async updateAssignments(): Promise<void> {
    const approverAssignmentsRequest = await this.context.userManagementService.getLocationApproverAssignments(
      this._locationID
    );
    const needsApprovalAssignmentsRequest = await this.context.userManagementService.getLocationNeedsApprovalAssignments(
      this._locationID
    );
    const orderAccessAssignmentsRequest = await this.context.userManagementService.getLocationOrderAccessAssignments(
      this._locationID
    );
    const responses = await Promise.all([
      approverAssignmentsRequest,
      needsApprovalAssignmentsRequest,
      orderAccessAssignmentsRequest,
    ]);
    this.permissionAssignmentsStatic = responses.reduce((prev, current) => {
      return [...prev, ...current];
    }, []);
    this.permissionAssignmentsEditable = [...this.permissionAssignmentsStatic];
    this.checkForChanges();
  }

  setThreshold(value: number): void {
    this.currentLocationApprovalThresholdEditable = value;
  }

  setThresholdFromEvent(event: any): void {
    this.currentLocationApprovalThresholdEditable = Number(event.target.value);
  }

  async saveNewThreshold(): Promise<void> {
    const buyerID = this._locationID.split('-')[0];
    const newRuleExpression = `${this.currentApprovalRule.RuleExpression.split('>')[0]}>${
      this.currentLocationApprovalThresholdEditable
    }`;
    const newApprovalRule = await this.ocApprovalRuleService
      .Patch(buyerID, this.currentApprovalRule.ID, { RuleExpression: newRuleExpression })
      .toPromise();
    this.setApprovalRuleValues(newApprovalRule);
  }

  isAssigned(userID: string, assignmentType: string): boolean {
    return this.permissionAssignmentsEditable.some(n => n.UserID === userID && n.UserGroupID.includes(assignmentType));
  }

  toggleAssignment(userID: string, assignmentType: string): void {
    if (this.isAssigned(userID, assignmentType)) {
      this.permissionAssignmentsEditable = this.permissionAssignmentsEditable.filter(
        n => !(n.UserID === userID && n.UserGroupID.includes(assignmentType))
      );
    } else {
      this.permissionAssignmentsEditable = [
        ...this.permissionAssignmentsEditable,
        { UserID: userID, UserGroupID: `${this._locationID}-${assignmentType}` },
      ];
    }
    this.checkForChanges();
  }

  checkForChanges(): void {
    this.add = this.permissionAssignmentsEditable.filter(
      editableAssignment =>
        !this.permissionAssignmentsStatic.some(
          staticAssignment =>
            staticAssignment.UserID === editableAssignment.UserID &&
            staticAssignment.UserGroupID === editableAssignment.UserGroupID
        )
    );
    this.del = this.permissionAssignmentsStatic.filter(
      staticAssignment =>
        !this.permissionAssignmentsEditable.some(
          editableAssignment =>
            staticAssignment.UserID === editableAssignment.UserID &&
            staticAssignment.UserGroupID === editableAssignment.UserGroupID
        )
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    this.checkIfAllUsersAreAssignedToNeedsApproval();
  }

  discardUserUserGroupAssignmentChanges(): void {
    this.permissionAssignmentsEditable = this.permissionAssignmentsStatic;
    this.checkForChanges();
  }

  requestUserConfirmation(): void {
    this.requestedUserConfirmation = true;
  }

  async executeUserUserGroupAssignmentRequests(): Promise<void> {
    const buyerID = this._locationID.split('-')[0];
    const assignmentRequests = [
      this.add.map(a => this.ocOcUserGroupService.SaveUserAssignment(buyerID, a).toPromise()),
      this.del.map(d => this.ocOcUserGroupService.DeleteUserAssignment(buyerID, d.UserGroupID, d.UserID).toPromise()),
    ];
    await Promise.all(assignmentRequests);
    this.permissionAssignmentsStatic = this.permissionAssignmentsEditable;
    this.requestedUserConfirmation = false;
    this.checkForChanges();
  }
}
