import { Component, OnInit, Inject } from '@angular/core';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';
import { OcPasswordResetService } from '@ordercloud/angular-sdk';
import { DOCUMENT } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'auth-forgot-password-wrapper',
  templateUrl: './forgot-password-wrapper.component.html',
  styleUrls: ['./forgot-password-wrapper.component.scss'],
})
export class ForgotPasswordWrapperComponent implements OnInit {
  constructor(
    public context: ShopperContextService,
    private ocPasswordResetService: OcPasswordResetService,
    @Inject(DOCUMENT) private document: any,
    private router: Router
  ) {}

  ngOnInit() {}

  async forgot(event: { email: string }) {
    await this.ocPasswordResetService
      .SendVerificationCode({
        Email: event.email,
        ClientID: this.context.appSettings.clientID,
        URL: this.document.location.host,
      })
      .toPromise();
    // toastr
    this.router.navigateByUrl('/login');
  }
}
