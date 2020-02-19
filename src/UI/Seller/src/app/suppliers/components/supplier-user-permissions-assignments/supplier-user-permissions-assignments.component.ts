import { Component, Input, Output, EventEmitter } from '@angular/core';
import { User, MeUser, UserGroup, UserGroupAssignment, OcSupplierUserGroupService, OcSupplierUserService } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';

@Component({
  selector: 'supplier-user-permissions-assignments-component',
  templateUrl: './supplier-user-permissions-assignments.component.html',
  styleUrls: ['./supplier-user-permissions-assignments.component.scss'],
})
export class SupplierUserPermissionsAssignments {
  @Input() set user(user: User) {
    this.supplierID = this.supplierService.getParentResourceID();
    this.supplierID !== '!' && this.getSupplierUserGroups(this.supplierID);
    if (user?.ID) {
      this.userID = user.ID
      this.getSupplierUserGroupAssignments(user.ID, this.supplierID);
    }
  };
  @Input() isCreatingNew: boolean;
  @Output() assignmentsToAdd = new EventEmitter<UserGroupAssignment[]>();

  supplierID: string;
  userID: string;
  supplierUserGroups: UserGroup[];
  add: UserGroupAssignment[];
  del: UserGroupAssignment[];
  _supplierUserUserGroupAssignmentsStatic: UserGroupAssignment[] = [];
  _supplierUserUserGroupAssignmentsEditable: UserGroupAssignment[] = [];
  areChanges = false;
  requestedUserConfirmation: boolean = false;
  faExclamationCircle = faExclamationCircle;

  constructor(
    private ocSupplierUserGroupService: OcSupplierUserGroupService,
    private ocSupplierUserService: OcSupplierUserService,
    private productService: ProductService,
    private supplierService: SupplierService,
    router: Router,
  ) {}

  private readonly options = {
    filters: {
      "xp.Type": "UserPermissions"
    }
  };

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async getSupplierUserGroups(ID: string): Promise<void> {
    const groups = await this.ocSupplierUserGroupService.List(ID, this.options).toPromise();
    this.supplierUserGroups = groups.Items;
  }

  async getSupplierUserGroupAssignments(userID: any, supplierID: any): Promise<void> {
    const supplierUserGroupAssignments = await this.ocSupplierUserGroupService
      .ListUserAssignments(supplierID, { userID: userID})
      .toPromise();
    this._supplierUserUserGroupAssignmentsStatic = supplierUserGroupAssignments.Items;
    this._supplierUserUserGroupAssignmentsEditable = supplierUserGroupAssignments.Items;
  }

  toggleSupplierUserUserGroupAssignment(userGroup: UserGroup): void {
    if (this.isAssigned(userGroup)) {
      this._supplierUserUserGroupAssignmentsEditable = this._supplierUserUserGroupAssignmentsEditable.filter(
        groupAssignment => groupAssignment.UserGroupID !== userGroup.ID
      );
    } else {
      const newSupplierUserUserGroupAssignment = {
        UserID: this.userID,
        UserGroupID: userGroup.ID
      };
      this._supplierUserUserGroupAssignmentsEditable = [
        ...this._supplierUserUserGroupAssignmentsEditable,
        newSupplierUserUserGroupAssignment,
      ];
    }
    this.checkForSupplierUserUserGroupAssignmentChanges();
  }

  addSupplierUserUserGroupAssignment(userGroup: UserGroup): void {
    if (this.isAssigned(userGroup)) {
      this._supplierUserUserGroupAssignmentsEditable = this._supplierUserUserGroupAssignmentsEditable.filter(
        groupAssignment => groupAssignment.UserGroupID !== userGroup.ID
      );
    } else {
      const newSupplierUserUserGroupAssignment = {
        UserID: 'PENDING',
        UserGroupID: userGroup.ID
      };
      this._supplierUserUserGroupAssignmentsEditable = [
        ...this._supplierUserUserGroupAssignmentsEditable,
        newSupplierUserUserGroupAssignment,
      ];
    }
    this.checkForSupplierUserUserGroupAssignmentChanges();
  }

  isAssigned(userGroup: UserGroup) {
    return (
      this._supplierUserUserGroupAssignmentsEditable?.some(
        groupAssignment => groupAssignment.UserGroupID === userGroup.ID
      )
    );
  }

  checkForSupplierUserUserGroupAssignmentChanges() {
    this.add = this._supplierUserUserGroupAssignmentsEditable.filter(
      assignment => !JSON.stringify(this._supplierUserUserGroupAssignmentsStatic).includes(assignment.UserGroupID)
    );
    this.del = this._supplierUserUserGroupAssignmentsStatic.filter(
      assignment => !JSON.stringify(this._supplierUserUserGroupAssignmentsEditable).includes(assignment.UserGroupID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
    if (this.isCreatingNew) this.assignmentsToAdd.emit(this.add);
  }

  discardSupplierUserUserGroupAssignmentChanges() {
    this._supplierUserUserGroupAssignmentsEditable = this._supplierUserUserGroupAssignmentsStatic;
    this.checkForSupplierUserUserGroupAssignmentChanges();
  }

  async executeSupplierUserUserGroupAssignmentRequests(): Promise<void> {
    this.requestedUserConfirmation = false;
    await this.supplierService.updateSupplierUserUserGroupAssignments(this.supplierID, this.add, this.del);
    await this.getSupplierUserGroupAssignments(this.userID, this.supplierID);
    this.checkForSupplierUserUserGroupAssignmentChanges();
  }
}
