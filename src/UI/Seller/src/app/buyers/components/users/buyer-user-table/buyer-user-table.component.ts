import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { User, UserGroupAssignment } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ValidateEmail } from '@app-seller/validators/validators';
import { BuyerUserService } from '../buyer-user.service';
import { BuyerService } from '../../buyers/buyer.service';
import { UserGroupTypes } from '@app-seller/shared/components/user-group-assignments/user-group-assignments.constants';

function createBuyerUserForm(user: User) {
  return new FormGroup({
    Active: new FormControl(user.Active),
    Username: new FormControl(user.Username, Validators.required),
    FirstName: new FormControl(user.FirstName, Validators.required),
    LastName: new FormControl(user.LastName, Validators.required),
    Email: new FormControl(user.Email, [Validators.required, ValidateEmail]),
  });
}

@Component({
  selector: 'app-buyer-user-table',
  templateUrl: './buyer-user-table.component.html',
  styleUrls: ['./buyer-user-table.component.scss'],
})
export class BuyerUserTableComponent extends ResourceCrudComponent<User> {
  permissionUserGroupAssignments: UserGroupAssignment[] = [];
  locationUserGroupAssignments: UserGroupAssignment[] = [];

  constructor(
    private buyerUserService: BuyerUserService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    private buyerService: BuyerService,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, buyerUserService, router, activatedroute, ngZone, createBuyerUserForm);
  }

  captureUserGroupAssignments(event): void {
    if (event.UserGroupType === UserGroupTypes.UserPermissions) {
      this.permissionUserGroupAssignments = event.Assignments;
    }
    if (event.UserGroupType === UserGroupTypes.BuyerLocation) {
      this.locationUserGroupAssignments = event.Assignments;
    }
  }

  async createNewResource() {
    try {
      this.dataIsSaving = true;
      const user = await this.buyerUserService.createNewResource(this.updatedResource);
      await this.executeSupplierUserSecurityProfileAssignmentRequests(user);
      this.selectResource(user);
      this.dataIsSaving = false;
    } catch (ex) {
      this.dataIsSaving = false;
      throw ex;
    }
  }

  async executeSupplierUserSecurityProfileAssignmentRequests(user: User): Promise<void> {
    let assignmentsToMake = [...this.permissionUserGroupAssignments, ...this.locationUserGroupAssignments];
    assignmentsToMake = assignmentsToMake.map(a => {
      a.UserID = user.ID;
      return a;
    });
    const buyerID = this.buyerUserService.getParentResourceID();
    await this.buyerUserService.updateUserUserGroupAssignments(buyerID, assignmentsToMake, []);
  }
}
