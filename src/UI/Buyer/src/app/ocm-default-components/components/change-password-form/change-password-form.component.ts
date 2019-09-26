import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AppMatchFieldsValidator } from 'src/app/shared/validators/match-fields/match-fields.validator';
import { MeUser } from '@ordercloud/angular-sdk';
import { ValidateStrongPassword } from 'src/app/ocm-default-components/validators/validators';
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
    this.form = this.formBuilder.group(
      {
        currentPassword: ['', Validators.required],
        newPassword: [
          '',
          [
            Validators.required,
            ValidateStrongPassword, // password must include one number, one letter and have min length of 8
          ],
        ],
        confirmNewPassword: [''],
      },
      {
        validator: AppMatchFieldsValidator('newPassword', 'confirmNewPassword'),
      }
    );
  }

  async updatePassword() {
    const { currentPassword, newPassword } = this.form.value;
    // TODO: how is this valid? changing password without validating current on the server?
    await this.context.authentication.changePassword(newPassword);
  }
}
