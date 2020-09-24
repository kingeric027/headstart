import { Component, Input, Output, EventEmitter } from '@angular/core';
import { get as _get } from 'lodash';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserGroupAssignment, User } from '@ordercloud/angular-sdk';
import { BuyerUserService } from '../buyer-user.service';
import { ValidateEmail } from '@app-seller/validators/validators';
import { SupportedCountries, GeographyConfig } from '@app-seller/shared/models/supported-countries.interface';
@Component({
  selector: 'app-buyer-user-edit',
  templateUrl: './buyer-user-edit.component.html',
  styleUrls: ['./buyer-user-edit.component.scss'],
})
export class BuyerUserEditComponent {
  @Input()
  filterConfig;
  @Input()
  set resourceInSelection(buyerUser: User) {
    this.selectedResource = buyerUser;
    this.createBuyerUserForm(buyerUser);
  }
  @Output()
  updateResource = new EventEmitter<any>();
  @Output()
  userGroupAssignments = new EventEmitter<UserGroupAssignment[]>();
  @Output()
  hitScrollEnd: EventEmitter<any> = new EventEmitter();
  isCreatingNew: boolean;
  resourceForm: FormGroup;
  selectedResource: User;
  countryOptions: SupportedCountries[];
  isUserAssignedToGroups: boolean;
  constructor(public buyerUserService: BuyerUserService) {
    this.isCreatingNew = this.buyerUserService.checkIfCreatingNew();
    this.countryOptions = GeographyConfig.getCountries();
  }

  createBuyerUserForm(user: User) {
    this.resourceForm = new FormGroup({
      Active: new FormControl(user.Active),
      Username: new FormControl(user.Username, Validators.required),
      FirstName: new FormControl(user.FirstName, Validators.required),
      LastName: new FormControl(user.LastName, Validators.required),
      Email: new FormControl(user.Email, [Validators.required, ValidateEmail]),
      Country: new FormControl(user.xp?.Country, Validators.required)
    });
  }

  updateResourceFromEvent(event: any, field: string): void {
    field === 'Active'
      ? this.updateResource.emit({ value: event.target.checked, field })
      : this.updateResource.emit({ value: event.target.value, field });
  }

  addUserGroupAssignments(event): void {
    this.userGroupAssignments.emit(event);
  }

  userHasAssignments(event: boolean): void {
    this.isUserAssignedToGroups = event;
    if (event && !this.isCreatingNew) {
      this.resourceForm.controls.Country.disable();
    }
    if (!event && !this.isCreatingNew) {
      this.resourceForm.controls.Country.enable();
    }
  }
}
