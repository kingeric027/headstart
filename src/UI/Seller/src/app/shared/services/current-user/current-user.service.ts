import { Injectable, Inject } from '@angular/core';
import { User, Supplier, OcMeService, OcAuthService, OcTokenService } from '@ordercloud/angular-sdk';
import { applicationConfiguration, AppConfig } from '@app-seller/config/app.config';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
import { AppStateService } from '../app-state/app-state.service';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  me: User;
  constructor(
    private ocMeService: OcMeService,
    private ocAuthService: OcAuthService,
    @Inject(applicationConfiguration) private appConfig: AppConfig,
    private ocTokenService: OcTokenService,
    private appAuthService: AppAuthService,
    private appStateService: AppStateService
  ) {}

  async login(username: string, password: string, rememberMe: boolean) {
    const accessToken = await this.ocAuthService
      .Login(username, password, this.appConfig.clientID, this.appConfig.scope)
      .toPromise();

    if (rememberMe && accessToken.refresh_token) {
      /**
       * set the token duration in the dashboard - https://developer.ordercloud.io/dashboard/settings
       * refresh tokens are configured per clientID and initially set to 0
       * a refresh token of 0 means no refresh token is returned in OAuth accessToken
       */
      this.ocTokenService.SetRefresh(accessToken.refresh_token);
      this.appAuthService.setRememberStatus(true);
    }
    this.ocTokenService.SetAccess(accessToken.access_token);
    this.appStateService.isLoggedIn.next(true);
    this.me = await this.ocMeService.Get().toPromise();
  }

  async getUser(): Promise<User> {
    return this.me ? this.me : await this.ocMeService.Get().toPromise();
  }

  getCompany(): Supplier {
    return;
  }
}
