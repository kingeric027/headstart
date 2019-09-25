// angular
import { Component, OnInit, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

// angular libs
import { ToastrService } from 'ngx-toastr';

// ordercloud
import { AppMatchFieldsValidator } from 'src/app/shared';
import { PasswordReset } from '@ordercloud/angular-sdk';
import { ValidateStrongPassword } from 'src/app/ocm-default-components/validators/validators';
import { OCMComponent } from '../../shopper-context';

@Component({
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class OCMResetPassword extends OCMComponent implements OnInit, OnChanges {
  form: FormGroup;
  username: string;
  resetCode: string;
  appName: string;

  constructor(private activatedRoute: ActivatedRoute, private toasterService: ToastrService, private formBuilder: FormBuilder) {
    super();
  }

  ngOnInit() {
    const urlParams = this.activatedRoute.snapshot.queryParams;
    this.username = urlParams['user'];
    this.resetCode = urlParams['code'];

    this.form = this.formBuilder.group(
      {
        password: [
          '',
          [
            Validators.required,
            ValidateStrongPassword, // password must include one number, one letter and have min length of 8
          ],
        ],
        passwordConfirm: ['', Validators.required],
      },
      { validator: AppMatchFieldsValidator('password', 'passwordConfirm') }
    );
  }

  ngOnChanges() {
    this.appName = this.context.appSettings.appname;
  }

  async onSubmit() {
    const config: PasswordReset = {
      ClientID: this.context.appSettings.clientID,
      Password: this.form.get('password').value,
      Username: this.username,
    };
    await this.context.authentication.resetPassword(this.resetCode, config);
    this.toasterService.success('Password Reset', 'Success');
    this.context.routeActions.toLogin();
  }
}
