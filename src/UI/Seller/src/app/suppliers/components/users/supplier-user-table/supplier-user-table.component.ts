import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { User, UserGroupAssignment } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ValidateEmail } from '@app-seller/validators/validators';
import { SupplierUserService } from '../supplier-user.service';
import { SupplierService } from '../../suppliers/supplier.service';

function createSupplierUserForm(user: User) {
  return new FormGroup({
    Username: new FormControl(user.Username, Validators.required),
    FirstName: new FormControl(user.FirstName, Validators.required),
    LastName: new FormControl(user.LastName, Validators.required),
    Email: new FormControl(user.Email, [Validators.required, ValidateEmail]),
    Active: new FormControl(user.Active),
  });
}
@Component({
  selector: 'app-supplier-user-table',
  templateUrl: './supplier-user-table.component.html',
  styleUrls: ['./supplier-user-table.component.scss'],
})
export class SupplierUserTableComponent extends ResourceCrudComponent<User> {
  userGroupAssignments: UserGroupAssignment[] = [];
  constructor(
    private supplierUserService: SupplierUserService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private supplierService: SupplierService,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, supplierUserService, router, activatedroute, ngZone, createSupplierUserForm);
  }

  captureUserGroupAssignments(event): void {
    this.userGroupAssignments = event;
  }

  async createNewResource() {
    try {
      this.dataIsSaving = true;
      const supplierUser = await this.supplierUserService.createNewResource(this.updatedResource);
      this.userGroupAssignments.forEach(assignment => (assignment.UserID = supplierUser.ID));
      await this.executeSupplierUserSecurityProfileAssignmentRequests();
      this.selectResource(supplierUser);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async executeSupplierUserSecurityProfileAssignmentRequests(): Promise<void> {
    const supplierID = this.supplierService.getParentResourceID();
    await this.supplierService.updateSupplierUserUserGroupAssignments(supplierID, this.userGroupAssignments, []);
  }
}
