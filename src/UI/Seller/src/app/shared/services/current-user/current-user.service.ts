import { Injectable, Inject } from '@angular/core';
import {
  User,
  Supplier,
  OcMeService,
  OcAuthService,
  OcTokenService,
  MeUser,
  OcSupplierService,
} from '@ordercloud/angular-sdk';
import { applicationConfiguration, AppConfig } from '@app-seller/config/app.config';
import { AppAuthService, TokenRefreshAttemptNotPossible } from '@app-seller/auth/services/app-auth.service';
import { AppStateService } from '../app-state/app-state.service';
import { UserContext } from '@app-seller/config/user-context';
import { SELLER } from '@app-seller/shared/models/ordercloud-user.types';
import { MiddlewareAPIService } from '../middleware-api/middleware-api.service';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  me: MeUser;
  mySupplier: Supplier;
  constructor(
    private ocMeService: OcMeService,
    private ocAuthService: OcAuthService,
    @Inject(applicationConfiguration) private appConfig: AppConfig,
    private ocTokenService: OcTokenService,
    private appAuthService: AppAuthService,
    private appStateService: AppStateService,
    private ocSupplierService: OcSupplierService,
    private middleware: MiddlewareAPIService
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
    this.mySupplier = await this.middleware.getMySupplier(this.me.Supplier.ID);
  }

  async getUser(): Promise<MeUser> {
    return this.me ? this.me : await this.ocMeService.Get().toPromise();
  }

  async getMySupplier(): Promise<Supplier> {
    const me = await this.getUser();
    return this.mySupplier ? this.mySupplier : await this.middleware.getMySupplier(me.Supplier.ID);
  }

  async getUserContext(): Promise<UserContext> {
    const UserContext: UserContext = await this.constructUserContext();
    return UserContext;
  }

  async constructUserContext(): Promise<UserContext> {
    const me: MeUser = await this.getUser();
    const userType = await this.appAuthService.getOrdercloudUserType();
    const userRoles = await this.appAuthService.getUserRoles();
    return {
      Me: me,
      UserType: userType,
      UserRoles: userRoles,
    };
  }

  async isSupplierUser() {
    const me = await this.getUser();
    return me.Supplier ? true : false;
  }
}
