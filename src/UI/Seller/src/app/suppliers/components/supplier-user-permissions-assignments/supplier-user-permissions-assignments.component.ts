import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Buyer, OcBuyerService, ProductAssignment, ProductCatalogAssignment, OcCatalogService, User, SecurityProfile, SecurityProfileAssignment, OcSecurityProfileService, MeUser, UserGroup, UserGroupAssignment, OcUserGroupService, OcSupplierUserGroupService, OcSupplierUserService } from '@ordercloud/angular-sdk';
import { MarketPlaceProduct } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'supplier-user-permissions-assignments-component',
  templateUrl: './supplier-user-permissions-assignments.component.html',
  styleUrls: ['./supplier-user-permissions-assignments.component.scss'],
})
export class SupplierUserPermissionsAssignments implements OnChanges {
  @Input() user: MeUser;
  @Input() set supplierID(ID: string) {
    console.log('supplierID', ID)
    this.getSupplierUserGroups(ID);
  };
  supplierUserGroups: UserGroup[];
  add: UserGroupAssignment[];
  del: UserGroupAssignment[];
  _supplierUserUserGroupAssignmentsStatic: UserGroupAssignment[];
  _supplierUserUserGroupAssignmentsEditable: UserGroupAssignment[];
  areChanges = false;
  requestedUserConfirmation: boolean = false;
  faExclamationCircle = faExclamationCircle;

  constructor(
    private ocSupplierUserGroupService: OcSupplierUserGroupService,
    private ocSupplierUserService: OcSupplierUserService,
    private productService: ProductService
  ) {}

  private readonly options = {
    filters: {
      "xp.Type": "UserPermissions"
    }
  };

  ngOnChanges(changes: SimpleChanges) {
    console.log(changes.user, changes.supplierID)
    // this.getSupplierUserGroupAssignments(this.user);
  }

  requestUserConfirmation() {
    this.requestedUserConfirmation = true;
  }

  async getSupplierUserGroups(ID: string): Promise<void> {
    const groups = await this.ocSupplierUserGroupService.List(ID, this.options).toPromise();
    this.supplierUserGroups = groups.Items;
  }

  async getSupplierUserGroupAssignments(user: MeUser): Promise<void> {
    console.log('USER',user)
    const supplierUserGroupAssignments = await this.ocSupplierUserGroupService
      .ListUserAssignments(this.supplierID, { userID: user.ID})
      .toPromise();
    this._supplierUserUserGroupAssignmentsStatic = supplierUserGroupAssignments.Items;
    this._supplierUserUserGroupAssignmentsEditable = supplierUserGroupAssignments.Items;
  }

  toggleSupplierUserSecurityProfileAssignment(user: MeUser) {
    if (this.isAssigned(user)) {
      this._supplierUserUserGroupAssignmentsEditable = this._supplierUserUserGroupAssignmentsEditable.filter(
        securityProfileAssignment => securityProfileAssignment.UserID !== user.ID
      );
    } else {
      const newSupplierUserSecurityProfileAssignment = {
        SupplierID: user.Supplier?.ID,
        UserID: user.ID,
        SecurityProfileID: ''
      };
      this._supplierUserUserGroupAssignmentsEditable = [
        ...this._supplierUserUserGroupAssignmentsEditable,
        newSupplierUserSecurityProfileAssignment,
      ];
    }
    this.checkForSupplierUserSecurityProfileAssignmentChanges();
  }

  isAssigned(user: MeUser) {
    return (
      this._supplierUserUserGroupAssignmentsEditable &&
      this._supplierUserUserGroupAssignmentsEditable.some(
        securityProfileAssignment => securityProfileAssignment.UserID === user.ID
      )
    );
  }

  checkForSupplierUserSecurityProfileAssignmentChanges() {
    this.add = this._supplierUserUserGroupAssignmentsEditable.filter(
      assignment => !JSON.stringify(this._supplierUserUserGroupAssignmentsStatic).includes(assignment.UserID)
    );
    this.del = this._supplierUserUserGroupAssignmentsStatic.filter(
      assignment => !JSON.stringify(this._supplierUserUserGroupAssignmentsEditable).includes(assignment.UserID)
    );
    this.areChanges = this.add.length > 0 || this.del.length > 0;
    if (!this.areChanges) this.requestedUserConfirmation = false;
  }

  discardSupplierUserSecurityProfileAssignmentChanges() {
    this._supplierUserUserGroupAssignmentsEditable = this._supplierUserUserGroupAssignmentsStatic;
    this.checkForSupplierUserSecurityProfileAssignmentChanges();
  }

  async executeSupplierUserSecurityProfileAssignmentRequests(): Promise<void> {
    this.requestedUserConfirmation = false;
    // TODO: execute the security profile assignment updates here
    // await this.supplierUserService.updatesupplierUserGroupAssignments(this.add, this.del);
    await this.getSupplierUserGroupAssignments(this.user);
    this.checkForSupplierUserSecurityProfileAssignmentChanges();
  }
}
