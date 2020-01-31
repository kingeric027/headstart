import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { User } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { SupplierUserService } from '@app-seller/shared/services/supplier/supplier-user.service';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ValidateEmail } from '@app-seller/validators/validators';

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
}
