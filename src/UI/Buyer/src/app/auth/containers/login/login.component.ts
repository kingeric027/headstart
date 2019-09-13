// angular
import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';

// ordercloud
import { OcTokenService } from '@ordercloud/angular-sdk';
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';
import { AuthService } from '@app-buyer/shared/services/auth/auth.service';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';

@Component({
  selector: 'auth-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  isAnon: boolean;

  constructor(
    private authService: AuthService,
    private ocTokenService: OcTokenService,
    private router: Router,
    private formBuilder: FormBuilder,
    private currentUser: CurrentUserService,
    @Inject(applicationConfiguration) public appConfig: AppConfig
  ) {}

  ngOnInit() {
    this.form = this.formBuilder.group({
      username: '',
      password: '',
      rememberMe: false,
    });
    this.isAnon = this.currentUser.isAnonymous;
  }

  async onSubmit() {
    const username = this.form.get('username').value;
    const password = this.form.get('password').value;
    const credentials = await this.authService.login(username, password);
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
    this.router.navigateByUrl('/home');
  }

  showRegisterLink(): boolean {
    return this.isAnon && this.appConfig.anonymousShoppingEnabled;
  }
}
