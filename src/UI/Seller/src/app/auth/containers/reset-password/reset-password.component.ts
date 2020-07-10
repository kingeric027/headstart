// angular
import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// angular libs
import { ToastrService } from 'ngx-toastr';

// ordercloud
import { AppFormErrorService } from '@app-seller/shared';
import { applicationConfiguration, AppConfig } from '@app-seller/config/app.config';
import { OcPasswordResetService, PasswordReset, TokenPasswordReset, OcMeService } from '@ordercloud/angular-sdk';
import { ValidateFieldMatches, ValidateStrongPassword } from '@app-seller/validators/validators';

@Component({
  selector: 'auth-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  username: string;
  token: string;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private toasterService: ToastrService,
    private formBuilder: FormBuilder,
    private ocPasswordResetService: OcPasswordResetService,
    private ocMeService: OcMeService,
    private formErrorService: AppFormErrorService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  ngOnInit() {
    const urlParams = this.activatedRoute.snapshot.queryParams;
    this.token = urlParams['token'];

    this.resetPasswordForm = new FormGroup({
      password: new FormControl('', [Validators.required, ValidateStrongPassword]),
      passwordConfirm: new FormControl('', [Validators.required, ValidateFieldMatches('password')]),
    });
  }

  onSubmit() {
    if (this.resetPasswordForm.status === 'INVALID') {
      return;
    }

    const config: TokenPasswordReset = {
      NewPassword: this.resetPasswordForm.get('password').value,
    };

    this.ocMeService.ResetPasswordByToken(config, { accessToken: this.token }).subscribe(
      () => {
        this.toasterService.success('Password Reset Successfully');
        this.router.navigateByUrl('/login');
      },
      error => {
        throw error;
      }
    );
  }

  // control visibility of password mismatch error
  protected passwordMismatchError = (): boolean =>
    this.formErrorService.hasPasswordMismatchError(this.resetPasswordForm);
}
