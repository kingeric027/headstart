// angular
import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

// angular libs
import { ToastrService } from 'ngx-toastr';

// ordercloud
import { OcPasswordResetService } from '@ordercloud/angular-sdk';
import { isPlatformBrowser } from '@angular/common';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'auth-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent implements OnInit {
  resetEmailForm: FormGroup;
  appName: string;

  constructor(
    private ocPasswordResetService: OcPasswordResetService,
    private formBuilder: FormBuilder,
    private toasterService: ToastrService,
    @Inject(PLATFORM_ID) private platformId: Object,
    private context: ShopperContextService
  ) {}

  ngOnInit() {
    this.appName = this.context.appSettings.appname;
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
        ClientID: this.context.appSettings.clientID,
        URL: hostUrl,
      })
      .subscribe(
        () => {
          this.toasterService.success('Password Reset Email Sent!', 'Success');
          this.context.routeActions.toLogin();
        },
        (error) => {
          throw error;
        }
      );
  }
}
