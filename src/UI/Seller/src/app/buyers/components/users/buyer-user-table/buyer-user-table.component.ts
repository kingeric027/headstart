import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { User } from '@ordercloud/angular-sdk';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ValidateEmail } from '@app-seller/validators/validators';
import { BuyerUserService } from '../buyer-user.service';
import { BuyerService } from '../../buyers/buyer.service';

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
}
