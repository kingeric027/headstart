// angular
import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
// angular libs

// ordercloud
import { PasswordReset } from 'ordercloud-javascript-sdk';
import { ValidateStrongPassword, ValidateFieldMatches } from '../../../validators/validators';
import { ToastrService } from 'ngx-toastr';
import { ShopperContextService } from 'marketplace';

@Component({
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class OCMResetPassword implements OnInit {
  form: FormGroup;
  username: string;
  resetCode: string;
  appName: string;

  constructor(private toasterService: ToastrService, private context: ShopperContextService) {}

  ngOnInit(): void {
    // TODO - figure out how to access url.
    // const urlParams = this.activatedRoute.snapshot.queryParams;
    // this.username = urlParams['user'];
    // this.resetCode = urlParams['code'];
    this.appName = this.context.appSettings.appname;
    this.form = new FormGroup({
      password: new FormControl('', [Validators.required, ValidateStrongPassword]),
      passwordConfirm: new FormControl('', [Validators.required, ValidateFieldMatches('password')]),
    });
  }

  async onSubmit(): Promise<void> {
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
