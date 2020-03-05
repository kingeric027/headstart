import { Component, Input, Output, EventEmitter } from '@angular/core';
import { User, UserGroup, UserGroupAssignment, OcUserGroupService, OcUserService } from '@ordercloud/angular-sdk';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { ProductService } from '@app-seller/products/product.service';
import { BuyerService } from '../../buyers/buyer.service';
import { BuyerUserService } from '../buyer-user.service';

@Component({
  selector: 'buyer-user-permissions-assignments-component',
  templateUrl: './buyer-user-permissions-assignments.component.html',
  styleUrls: ['./buyer-user-permissions-assignments.component.scss'],
})
export class BuyerUserPermissionsAssignments {
  @Input() set user(user: User) {
    this.buyerID = this.buyerService.getParentResourceID();
    this.buyerID !== '!' && this.getBuyerUserGroups(this.buyerID);
    if (user?.ID) {
      this.userID = user.ID
      this.getBuyerUserGroupAssignments(user.ID, this.buyerID);
    }
  };
  @Input() isCreatingNew: boolean;
  @Output() assignmentsToAdd = new EventEmitter<UserGroupAssignment[]>();

  buyerID: string;
  userID: string;
  buyerUserGroups: UserGroup[];
  add: UserGroupAssignment[];
  del: UserGroupAssignment[];
  _buyerUserUserGroupAssignmentsStatic: UserGroupAssignment[] = [];
  _buyerUserUserGroupAssignmentsEditable: UserGroupAssignment[] = [];
  areChanges = false;
  requestedUserConfirmation = false;
  faExclamationCircle = faExclamationCircle;

  constructor(
    private ocBuyerUserGroupService: OcUserGroupService,
    private ocBuyerUserService: OcUserService,
    private productService: ProductService,
    private buyerService: BuyerService,
    private buyerUserService: BuyerUserService,
    router: Router,
  ) {}

  private readonly options = {
    filters: {
      'xp.Type': 'UserPermissions'
    }
  };

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async getBuyerUserGroups(ID: string): Promise<void> {
    const groups = await this.ocBuyerUserGroupService.List(ID, this.options).toPromise();
    this.buyerUserGroups = groups.Items;
  }

  async getBuyerUserGroupAssignments(userID: any, buyerID: any): Promise<void> {
    const buyerUserGroupAssignments = await this.ocBuyerUserGroupService
      .ListUserAssignments(buyerID, { userID})
      .toPromise();
    this._buyerUserUserGroupAssignmentsStatic = buyerUserGroupAssignments.Items;
    this._buyerUserUserGroupAssignmentsEditable = buyerUserGroupAssignments.Items;
  }

  toggleBuyerUserUserGroupAssignment(userGroup: UserGroup): void {
    if (this.isAssigned(userGroup)) {
      this._buyerUserUserGroupAssignmentsEditable = this._buyerUserUserGroupAssignmentsEditable.filter(
        groupAssignment => groupAssignment.UserGroupID !== userGroup.ID
      );
    } else {
      const newBuyerUserUserGroupAssignment = {
        UserID: this.userID,
        UserGroupID: userGroup.ID
      };
      this._buyerUserUserGroupAssignmentsEditable = [
        ...this._buyerUserUserGroupAssignmentsEditable,
        newBuyerUserUserGroupAssignment,
      ];
    }
    this.checkForBuyerUserUserGroupAssignmentChanges();
  }

  addBuyerUserUserGroupAssignment(userGroup: UserGroup): void {
    if (this.isAssigned(userGroup)) {
      this._buyerUserUserGroupAssignmentsEditable = this._buyerUserUserGroupAssignmentsEditable.filter(
        groupAssignment => groupAssignment.UserGroupID !== userGroup.ID
      );
    } else {
      const newBuyerUserUserGroupAssignment = {
        UserID: 'PENDING',
        UserGroupID: userGroup.ID
      };
      this._buyerUserUserGroupAssignmentsEditable = [
        ...this._buyerUserUserGroupAssignmentsEditable,
        newBuyerUserUserGroupAssignment,
      ];
    }
    this.checkForBuyerUserUserGroupAssignmentChanges();
  }

  isAssigned(userGroup: UserGroup) {
    return (
      this._buyerUserUserGroupAssignmentsEditable?.some(
        groupAssignment => groupAssignment.UserGroupID === userGroup.ID
      )
    );
  }

  checkForBuyerUserUserGroupAssignmentChanges() {
    this.add = this._buyerUserUserGroupAssignmentsEditable.filter(
      assignment => !JSON.stringify(this._buyerUserUserGroupAssignmentsStatic).includes(assignment.UserGroupID)
    );
    this.del = this._buyerUserUserGroupAssignmentsStatic.filter(
      assignment => !JSON.stringify(this._buyerUserUserGroupAssignmentsEditable).includes(assignment.UserGroupID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
    if (this.isCreatingNew) this.assignmentsToAdd.emit(this.add);
  }

  discardBuyerUserUserGroupAssignmentChanges() {
    this._buyerUserUserGroupAssignmentsEditable = this._buyerUserUserGroupAssignmentsStatic;
    this.checkForBuyerUserUserGroupAssignmentChanges();
  }

  async executeBuyerUserUserGroupAssignmentRequests(): Promise<void> {
    this.requestedUserConfirmation = false;
    await this.buyerUserService.updateBuyerUserUserGroupAssignments(this.buyerID, this.add, this.del);
    await this.getBuyerUserGroupAssignments(this.userID, this.buyerID);
    this.checkForBuyerUserUserGroupAssignmentChanges();
  }
}
