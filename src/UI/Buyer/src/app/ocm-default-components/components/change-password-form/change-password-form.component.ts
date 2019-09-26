import { Component, OnInit } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, Validators, FormControl, FormBuilder } from '@angular/forms';
import { MeUser } from '@ordercloud/angular-sdk';
import { ValidateStrongPassword, ValidateFieldMatches } from 'src/app/ocm-default-components/validators/validators';
import { OCMComponent } from '../../shopper-context';

@Component({
  selector: 'profile-change-password-form',
  templateUrl: './change-password-form.component.html',
  styleUrls: ['./change-password-form.component.scss'],
})
export class OCMChangePasswordFormComponent extends OCMComponent implements OnInit {
  form: FormGroup;
  me: MeUser;
  faTimes = faTimes;

  constructor(private formBuilder: FormBuilder) {
    super();
  }

  ngOnInit() {
    this.setForm();
  }

  setForm() {
    this.form = new FormGroup({
      currentPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', [Validators.required, ValidateStrongPassword]),
      confirmNewPassword: new FormControl('', ValidateFieldMatches('newPassword')),
    });
  }

  async updatePassword() {
    const { currentPassword, newPassword } = this.form.value;
    // TODO: how is this valid? changing password without validating current on the server?
    await this.context.authentication.changePassword(newPassword);
  }
}
