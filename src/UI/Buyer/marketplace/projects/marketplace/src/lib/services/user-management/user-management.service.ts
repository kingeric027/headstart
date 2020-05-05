import { Injectable } from '@angular/core';
import {
  OcUserService,
  UserGroup,
  User,
  OcUserGroupService,
  UserGroupAssignment,
  OcMeService,
  ApprovalRule,
  OcApprovalRuleService,
} from '@ordercloud/angular-sdk';
import { CurrentUserService } from '../current-user/current-user.service';

export interface IUserManagement {
  getLocations(): Promise<UserGroup[]>;
  getLocationUsers(locationID: string): Promise<User[]>;
  getLocationApproverAssignments(locationID: string): Promise<UserGroupAssignment[]>;
  getLocationNeedsApprovalAssignments(locationID: string): Promise<UserGroupAssignment[]>;
  getLocationApprovalRule(locationID: string): Promise<ApprovalRule>;
}

@Injectable({
  providedIn: 'root',
})
export class UserManagementService implements IUserManagement {
  constructor(
    private ocUserGroupService: OcUserGroupService,
    private ocMeService: OcMeService,
    public currentUserService: CurrentUserService,
    public ocApprovalRuleService: OcApprovalRuleService,
    private ocUserService: OcUserService
  ) {}

  async getLocations(): Promise<UserGroup[]> {
    const buyerID = this.currentUserService.get().Buyer.ID;

    // todo accomodate more than 100 locations
    const loctions = await this.ocMeService
      .ListUserGroups({ pageSize: 100, filters: { 'xp.Type': 'BuyerLocation' } })
      .toPromise();
    return loctions.Items;
  }

  async getLocationUsers(locationID: string): Promise<User[]> {
    const buyerID = this.currentUserService.get().Buyer.ID;

    // todo accomodate more than 100 locations
    const locationUsers = await this.ocUserService
      .List(buyerID, { pageSize: 100, userGroupID: locationID })
      .toPromise();
    return locationUsers.Items;
  }

  async getLocationApproverAssignments(locationID: string): Promise<UserGroupAssignment[]> {
    const buyerID = this.currentUserService.get().Buyer.ID;

    // todo accomodate more than 100 locations
    const userGroupID = `${locationID}-OrderApprover`;
    const locationUsers = await this.ocUserGroupService
      .ListUserAssignments(buyerID, { pageSize: 100, userGroupID: userGroupID })
      .toPromise();
    return locationUsers.Items;
  }

  async getLocationNeedsApprovalAssignments(locationID: string): Promise<UserGroupAssignment[]> {
    const buyerID = this.currentUserService.get().Buyer.ID;

    // todo accomodate more than 100 locations
    const userGroupID = `${locationID}-NeedsApproval`;
    const locationUsers = await this.ocUserGroupService
      .ListUserAssignments(buyerID, { pageSize: 100, userGroupID: userGroupID })
      .toPromise();
    return locationUsers.Items;
  }

  async getLocationOrderAccessAssignments(locationID: string): Promise<UserGroupAssignment[]> {
    const buyerID = this.currentUserService.get().Buyer.ID;

    // todo accomodate more than 100 locations
    const userGroupID = `${locationID}-ViewAllLocationOrders`;
    const locationUsers = await this.ocUserGroupService
      .ListUserAssignments(buyerID, { pageSize: 100, userGroupID: userGroupID })
      .toPromise();
    return locationUsers.Items;
  }

  async getLocationApprovalRule(locationID: string): Promise<ApprovalRule> {
    const buyerID = this.currentUserService.get().Buyer.ID;
    const approvalRule = await this.ocApprovalRuleService.Get(buyerID, locationID).toPromise();
    return approvalRule;
  }
}
