import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { Router } from '@angular/router';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'app-login-wrapper',
  templateUrl: './login-wrapper.component.html',
  styleUrls: ['./login-wrapper.component.scss'],
})
export class LoginWrapperComponent implements OnInit {
  constructor(
    public context: ShopperContextService,
    private authService: AuthService,
    private ocTokenService: OcTokenService,
    private router: Router
  ) {}

  ngOnInit() {}

  async login(event: { username: string; password: string; rememberMe: boolean }) {
    const credentials = await this.authService.profiledLogin(event.username, event.password);
    if (event.rememberMe && credentials.refresh_token) {
      /**
       * set the token duration in the dashboard - https://developer.ordercloud.io/dashboard/settings
       * refresh tokens are configured per clientID and initially set to 0
       * a refresh token of 0 means no refresh token is returned in OAuth response
       */
      this.ocTokenService.SetRefresh(credentials.refresh_token);
      this.authService.setRememberStatus(true);
    }
    this.router.navigateByUrl('/home');
  }
}
