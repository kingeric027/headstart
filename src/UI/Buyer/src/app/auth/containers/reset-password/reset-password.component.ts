// angular
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// angular libs
import { ToastrService } from 'ngx-toastr';

// ordercloud
import { AppMatchFieldsValidator } from 'src/app/shared';
import { OcPasswordResetService, PasswordReset } from '@ordercloud/angular-sdk';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { ValidateStrongPassword } from 'src/app/ocm-default-components/validators/validators';

@Component({
  selector: 'auth-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit {
  form: FormGroup;
  username: string;
  resetCode: string;
  appName: string;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private toasterService: ToastrService,
    private formBuilder: FormBuilder,
    private ocPasswordResetService: OcPasswordResetService,
    private context: ShopperContextService
  ) {}

  ngOnInit() {
    this.appName = this.context.appSettings.appname;
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

  onSubmit() {
    if (this.form.status === 'INVALID') {
      return;
    }

    const config: PasswordReset = {
      ClientID: this.context.appSettings.clientID,
      Password: this.form.get('password').value,
      Username: this.username,
    };

    this.ocPasswordResetService.ResetPasswordByVerificationCode(this.resetCode, config).subscribe(() => {
      this.toasterService.success('Password Reset', 'Success');
      this.router.navigateByUrl('/login');
    });
  }
}
