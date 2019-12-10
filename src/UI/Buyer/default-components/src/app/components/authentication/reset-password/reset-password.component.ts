// angular
import { Component, OnInit, OnChanges } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';

// angular libs

// ordercloud
import { PasswordReset } from '@ordercloud/angular-sdk';
import { OCMComponent } from '../../base-component';
import { ValidateStrongPassword, ValidateFieldMatches } from '../../../validators/validators';
import { ToastrService } from 'ngx-toastr';

@Component({
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class OCMResetPassword extends OCMComponent implements OnInit {
  form: FormGroup;
  username: string;
  resetCode: string;
  appName: string;

  constructor(private toasterService: ToastrService) {
    super();
  }

  ngOnInit() {
    // TODO - figure out how to access url.
    // const urlParams = this.activatedRoute.snapshot.queryParams;
    // this.username = urlParams['user'];
    // this.resetCode = urlParams['code'];
    this.form = new FormGroup({
      password: new FormControl('', [Validators.required, ValidateStrongPassword]),
      passwordConfirm: new FormControl('', [Validators.required, ValidateFieldMatches('password')]),
    });
  }

  ngOnContextSet() {
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
    this.context.router.toLogin();
  }
}
