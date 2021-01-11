// angular
import { Component, OnInit, Inject } from '@angular/core'
import { FormGroup, FormControl, Validators } from '@angular/forms'
import { Router, ActivatedRoute } from '@angular/router'

// angular libs
import { ToastrService } from 'ngx-toastr'

// ordercloud
import { AppConfig, AppFormErrorService } from '@app-seller/shared'
import { applicationConfiguration } from '@app-seller/config/app.config'
import { TokenPasswordReset } from '@ordercloud/angular-sdk'
import {
  ValidateFieldMatches,
  ValidateStrongPassword,
} from '@app-seller/validators/validators'
import { HttpClient, HttpHeaders } from '@angular/common/http'

@Component({
  selector: 'auth-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup
  username: string
  token: string

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private toasterService: ToastrService,
    private formErrorService: AppFormErrorService,
    private http: HttpClient,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.resetPasswordForm = new FormGroup({
      password: new FormControl('', [
        Validators.required,
        ValidateStrongPassword,
      ]),
      passwordConfirm: new FormControl('', [
        Validators.required,
        ValidateFieldMatches('password'),
      ]),
    })
  }

  private buildHeaders(): HttpHeaders {
    const urlParams = this.activatedRoute.snapshot.queryParams
    return new HttpHeaders({
      Authorization: `Bearer ${urlParams.token}`,
    })
  }

  onSubmit() {
    if (this.resetPasswordForm.status === 'INVALID') {
      return
    }

    const config: TokenPasswordReset = {
      NewPassword: this.resetPasswordForm.get('password').value,
    }

    const url = `${this.appConfig.orderCloudApiUrl}/${this.appConfig.orderCloudApiVersion}/me/password`
    this.http.post(url, config, { headers: this.buildHeaders() }).subscribe(
      () => {
        this.toasterService.success('Password Reset Successfully')
        this.router.navigateByUrl('/login')
      },
      (error) => {
        throw error
      }
    )
    // TODO: We SHOULD be able to use this function from the SDK, but if you uncomment,
    // ***  you'll see that you are unable to send along an accessToken ...

    // this.ocMeService.ResetPasswordByToken(config, { accessToken: this.token }).subscribe(
    //   () => {
    //     this.toasterService.success('Password Reset Successfully');
    //     this.router.navigateByUrl('/login');
    //   },
    //   error => {
    //     throw error;
    //   }
    // );
  }

  // control visibility of password mismatch error
  protected passwordMismatchError = (): boolean =>
    this.formErrorService.hasPasswordMismatchError(this.resetPasswordForm)
}
