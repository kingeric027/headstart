// angular
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

// ordercloud
import { OcTokenService } from '@ordercloud/angular-sdk';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { ShopperContextService } from 'src/app/shared/services/shopper-context/shopper-context.service';

@Component({
  selector: 'auth-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  isAnon: boolean;
  appName: string;

  constructor(private authService: AuthService, private ocTokenService: OcTokenService, private context: ShopperContextService) {}

  ngOnInit() {
    this.appName = this.context.appSettings.appname;
    this.isAnon = this.context.currentUser.isAnonymous;
    this.form = new FormGroup({
      username: new FormControl(''),
      password: new FormControl(''),
      rememberMe: new FormControl(false),
    });
  }

  async onSubmit() {
    const username = this.form.get('username').value;
    const password = this.form.get('password').value;
    const credentials = await this.authService.profiledLogin(username, password);
    const rememberMe = this.form.get('rememberMe').value;
    if (rememberMe && credentials.refresh_token) {
      /**
       * set the token duration in the dashboard - https://developer.ordercloud.io/dashboard/settings
       * refresh tokens are configured per clientID and initially set to 0
       * a refresh token of 0 means no refresh token is returned in OAuth response
       */
      this.ocTokenService.SetRefresh(credentials.refresh_token);
      this.authService.setRememberStatus(true);
    }
    this.context.routeActions.toHome();
  }

  showRegisterLink(): boolean {
    return this.isAnon && this.context.appSettings.anonymousShoppingEnabled;
  }
}
