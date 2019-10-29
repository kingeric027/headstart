import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { MeUser } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../base-component';
import {
  ValidateName,
  ValidateEmail,
  ValidatePhone,
  ValidateStrongPassword,
  ValidateFieldMatches,
} from '../../validators/validators';

@Component({
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class OCMRegister extends OCMComponent implements OnInit {
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

  ngOnContextSet() {
    this.appName = this.context.appSettings.appname;
  }

  // TODO: requires anonymous token, but not checked for here
  async onSubmit() {
    const me: MeUser = this.form.value;
    me.Active = true;
    await this.context.authentication.register(me);
    this.context.router.toLogin();
  }
}
