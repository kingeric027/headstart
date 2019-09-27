import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import {
  ValidateName,
  ValidatePhone,
  ValidateEmail,
  ValidateStrongPassword,
  ValidateFieldMatches,
} from 'src/app/ocm-default-components/validators/validators';
import { OCMComponent } from '../../shopper-context';
import { MeUser } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class OCMRegister extends OCMComponent implements OnInit, OnChanges {
  form: FormGroup;
  appName: string;

  // TODO: validation isn't working
  ngOnInit() {
    this.form = new FormGroup({
      Username: new FormControl('', Validators.required),
      FirstName: new FormControl('', [Validators.required, ValidateName]),
      LastName: new FormControl('', [Validators.required, ValidateName]),
      Email: new FormControl('', [Validators.required, ValidateEmail]),
      Phone: new FormControl('', ValidatePhone),
      Password: new FormControl('', [Validators.required, ValidateStrongPassword]),
      ConfirmPassword: new FormControl('', [Validators.required, ValidateFieldMatches('Password')]),
    });
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
