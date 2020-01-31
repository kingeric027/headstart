import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { User } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { SellerUserService } from '@app-seller/shared/services/seller-user/seller-user.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ValidateEmail } from '@app-seller/validators/validators';

function createSellerUserForm(user: User) {
  return new FormGroup({
    Username: new FormControl(user.Username, Validators.required),
    FirstName: new FormControl(user.FirstName, Validators.required),
    LastName: new FormControl(user.LastName, Validators.required),
    Email: new FormControl(user.Email, [Validators.required, ValidateEmail]),
    Active: new FormControl(user.Active),
  });
}

@Component({
  selector: 'app-seller-user-table',
  templateUrl: './seller-user-table.component.html',
  styleUrls: ['./seller-user-table.component.scss'],
})
export class SellerUserTableComponent extends ResourceCrudComponent<User> {
  constructor(
    private sellerUserService: SellerUserService,
    changeDetectorRef: ChangeDetectorRef,
    router: Router,
    activatedroute: ActivatedRoute,
    ngZone: NgZone
  ) {
    super(changeDetectorRef, sellerUserService, router, activatedroute, ngZone, createSellerUserForm);
  }
}
