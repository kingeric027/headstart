// angular
import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

// angular libs
import { ToastrService } from 'ngx-toastr';

// ordercloud
import { PasswordReset } from '@ordercloud/angular-sdk';
import { ValidateStrongPassword, ValidateFieldMatches } from 'src/app/ocm-default-components/validators/validators';
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

  constructor(private activatedRoute: ActivatedRoute, private toasterService: ToastrService) {
    super();
  }

  ngOnInit() {
    const urlParams = this.activatedRoute.snapshot.queryParams;
    this.username = urlParams['user'];
    this.resetCode = urlParams['code'];
    this.form = new FormGroup({
      password: new FormControl('', [Validators.required, ValidateStrongPassword]),
      passwordConfirm: new FormControl('', [Validators.required, ValidateFieldMatches('password')]),
    });
  }

  ngOnChanges() {
    this.appName = this.context.appSettings.appname;
  }

  async onSubmit() {
    if (this.form.status === 'INVALID') {
      return;
    }

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
