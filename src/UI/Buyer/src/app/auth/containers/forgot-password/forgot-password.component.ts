// angular
import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

// angular libs
import { ToastrService } from 'ngx-toastr';

// ordercloud
import { OcPasswordResetService } from '@ordercloud/angular-sdk';
import { DOCUMENT } from '@angular/common';
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
    private toasterService: ToastrService,
    @Inject(DOCUMENT) private document: any,
    private context: ShopperContextService
  ) {}

  ngOnInit() {
    this.appName = this.context.appSettings.appname;
    this.resetEmailForm = new FormGroup({ email: new FormControl('') });
  }

  async onSubmit() {
    const URL = this.document.location.host;
    const Email = this.resetEmailForm.get('email').value;
    const ClientID = this.context.appSettings.clientID;

    await this.ocPasswordResetService.SendVerificationCode({ Email, ClientID, URL }).toPromise();
    this.toasterService.success('Password Reset Email Sent!', 'Success');
    this.context.routeActions.toLogin();
  }
}
