import { Component, OnInit } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { MeUser } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../base-component';
import { ValidateStrongPassword, ValidateFieldMatches } from '../../validators/validators';

@Component({
  selector: 'profile-change-password-form',
  templateUrl: './change-password-form.component.html',
  styleUrls: ['./change-password-form.component.scss'],
})
export class OCMChangePasswordForm extends OCMComponent implements OnInit {
  form: FormGroup;
  me: MeUser;
  faTimes = faTimes;

  ngOnInit() {
    this.setForm();
  }

  ngOnContextSet() {}

  setForm() {
    this.form = new FormGroup({
      currentPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', [Validators.required, ValidateStrongPassword]),
      confirmNewPassword: new FormControl('', ValidateFieldMatches('newPassword')),
    });
  }

  async updatePassword() {
    const { newPassword } = this.form.value;
    // TODO: how is this valid? changing password without validating current on the server?
    await this.context.authentication.changePassword(newPassword);
  }
}
