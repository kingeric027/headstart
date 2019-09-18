// angular
import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';

// angular libs
import { ToastrService } from 'ngx-toastr';

// ordercloud
import { OcPasswordResetService } from '@ordercloud/angular-sdk';
import { applicationConfiguration, AppConfig } from 'src/app/config/app.config';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'auth-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent implements OnInit {
  resetEmailForm: FormGroup;

  constructor(
    private ocPasswordResetService: OcPasswordResetService,
    private router: Router,
    private formBuilder: FormBuilder,
    private toasterService: ToastrService,
    @Inject(PLATFORM_ID) private platformId: Object,
    @Inject(applicationConfiguration) public appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.resetEmailForm = this.formBuilder.group({ email: '' });
  }

  onSubmit() {
    let hostUrl: string;
    if (isPlatformBrowser(this.platformId)) {
      hostUrl = window.location.origin;
    }
    this.ocPasswordResetService
      .SendVerificationCode({
        Email: this.resetEmailForm.get('email').value,
        ClientID: this.appConfig.clientID,
        URL: hostUrl,
      })
      .subscribe(
        () => {
          this.toasterService.success('Password Reset Email Sent!', 'Success');
          this.router.navigateByUrl('/login');
        },
        (error) => {
          throw error;
        }
      );
  }
}
