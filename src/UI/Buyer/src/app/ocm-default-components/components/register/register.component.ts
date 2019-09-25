import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AppMatchFieldsValidator } from 'src/app/shared/validators/match-fields/match-fields.validator';
import { ValidateName, ValidatePhone, ValidateEmail, ValidateStrongPassword } from 'src/app/ocm-default-components/validators/validators';
import { OCMComponent } from '../../shopper-context';
import { MeUser } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class OCMRegisterComponent extends OCMComponent implements OnInit, OnChanges {
  form: FormGroup;
  appName: string;

  constructor(private formBuilder: FormBuilder) {
    super();
  }

  // TODO: validation isn't working
  ngOnInit() {
    this.form = this.formBuilder.group(
      {
        Username: ['', Validators.required],
        FirstName: ['', [Validators.required, ValidateName]],
        LastName: ['', [Validators.required, ValidateName]],
        Email: ['', [Validators.required, ValidateEmail]],
        Phone: ['', ValidatePhone],
        Password: ['', [Validators.required, ValidateStrongPassword]],
        ConfirmPassword: ['', [Validators.required, ValidateStrongPassword]],
      },
      {
        validator: AppMatchFieldsValidator('Password', 'ConfirmPassword'),
      }
    );
  }

  ngOnChanges() {
    this.appName = this.context.appSettings.appname;
  }

  // TODO: requires anonymous token, but not checked for here
  async onSubmit() {
    const me = <MeUser>this.form.value;
    me.Active = true;
    await this.context.authentication.register(me);
    this.context.routeActions.toLogin();
  }
}
