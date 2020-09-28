import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { User, UserGroupAssignment } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { SupplierUserService } from '../supplier-user.service';
import { SupplierService } from '../../suppliers/supplier.service';
@Component({
  selector: 'app-supplier-user-table',
  templateUrl: './supplier-user-table.component.html',
  styleUrls: ['./supplier-user-table.component.scss'],
})
export class SupplierUserTableComponent extends ResourceCrudComponent<User> {
  userGroupAssignments: UserGroupAssignment[] = [];
  constructor(
    private supplierUserService: SupplierUserService,
    public supplierService: SupplierService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, supplierUserService, router, activatedroute, ngZone);
  }

  captureUserGroupAssignments(event): void {
    this.userGroupAssignments = event.Assignments;
  }

  async createNewResource() {
    try {
      this.dataIsSaving = true;
      const supplierUser = await this.supplierUserService.createNewResource(this.updatedResource);
      this.userGroupAssignments.forEach(assignment => (assignment.UserID = supplierUser.ID));
      await this.executeSupplierUserSecurityProfileAssignmentRequests();
      await this.selectResource(supplierUser);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async executeSupplierUserSecurityProfileAssignmentRequests(): Promise<void> {
    const supplierID = await this.supplierUserService.getParentResourceID();
    await this.supplierUserService.updateUserUserGroupAssignments(supplierID, this.userGroupAssignments, []);
  }
}
