import { Injectable } from '@angular/core';
import { Observable, of, BehaviorSubject, from } from 'rxjs';
import { tap, catchError, finalize } from 'rxjs/operators';
import { Router } from '@angular/router';

// 3rd party
import {
  Tokens,
  AccessToken,
  Me,
  Auth,
  MeUser,
  ForgottenPassword,
  PasswordReset,
  AccessTokenBasic,
} from 'ordercloud-javascript-sdk';
// import { CookieService } from '@gorniv/ngx-universal';
import { CookieService } from 'ngx-cookie';
import { CurrentUserService } from '../current-user/current-user.service';
import { AppConfig } from '../../shopper-context';
import { CurrentOrderService } from '../order/order.service';
import { MarketplaceSDK } from 'marketplace-javascript-sdk';
import { OrdersToApproveStateService } from '../order-history/order-to-approve-state.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  fetchingRefreshToken = false;
  failedRefreshAttempt = false;
  refreshToken: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private rememberMeCookieName = `${this.appConfig.appname.replace(/ /g, '_').toLowerCase()}_rememberMe`;
  private loggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(
    private cookieService: CookieService,
    private router: Router,
    private currentOrder: CurrentOrderService,
    private currentUser: CurrentUserService,
    private ordersToApproveStateService: OrdersToApproveStateService,
    private appConfig: AppConfig
  ) {}

  // All this isLoggedIn stuff is only used in the header wrapper component
  // remove once its no longer needed.

  get isLoggedIn(): boolean {
    return this.loggedInSubject.value;
  }

  set isLoggedIn(value: boolean) {
    this.loggedInSubject.next(value);
  }

  onIsLoggedInChange(callback: (isLoggedIn: boolean) => void): void {
    this.loggedInSubject.subscribe(callback);
  }

  // change all this unreadable observable stuff
  refresh(): Observable<void> {
    this.fetchingRefreshToken = true;
    return from(this.refreshTokenLogin()).pipe(
      tap(token => {
        this.refreshToken.next(token.access_token);
      }),
      catchError(() => {
        // ignore new refresh attempts if a refresh
        // attempt failed within the last 3 seconds
        this.failedRefreshAttempt = true;
        setTimeout(() => {
          this.failedRefreshAttempt = false;
        }, 3000);
        this.logout();
        return of(null);
      }),
      finalize(() => {
        this.fetchingRefreshToken = false;
      })
    );
  }

  setToken(token: string): void {
    if (!token) return;
    Tokens.SetAccessToken(token);
    this.isLoggedIn = true;
  }

  async forgotPasssword(email: string): Promise<void> {
    await ForgottenPassword.SendVerificationCode({
      Email: email,
      ClientID: this.appConfig.clientID,
      URL: this.appConfig.baseUrl,
    });
    this.router.navigateByUrl('/login');
  }

  async register(me: MeUser<any>): Promise<AccessTokenBasic> {
    const token = await Me.Register(me);
    return token;
  }

  async profiledLogin(userName: string, password: string, rememberMe: boolean = false): Promise<AccessToken> {
    const creds = await Auth.Login(userName, password, this.appConfig.clientID, this.appConfig.scope);
    MarketplaceSDK.Tokens.SetAccessToken(creds.access_token);
    this.setToken(creds.access_token);
    if (rememberMe && creds.refresh_token) {
      /**
       * set the token duration in the dashboard - https://developer.ordercloud.io/dashboard/settings
       * refresh tokens are configured per clientID and initially set to 0
       * a refresh token of 0 means no refresh token is returned in OAuth response
       */
      Tokens.SetRefreshToken(creds.refresh_token);
      this.setRememberMeStatus(true);
    }
    this.router.navigateByUrl('/home');
    this.ordersToApproveStateService.alertIfOrdersToApprove();

    return creds;
  }

  async anonymousLogin(): Promise<AccessToken> {
    try {
      const creds = await Auth.Anonymous(this.appConfig.clientID, this.appConfig.scope);
      MarketplaceSDK.Tokens.SetAccessToken(creds.access_token);
      this.setToken(creds.access_token);
      return creds;
    } catch (err) {
      this.logout();
      throw new Error(err);
    }
  }

  async logout(): Promise<void> {
    Tokens.RemoveAccessToken();
    MarketplaceSDK.Tokens.RemoveAccessToken();
    this.isLoggedIn = false;
    if (this.appConfig.anonymousShoppingEnabled) {
      this.router.navigate(['/home']);
      await this.currentUser.reset();
      this.currentOrder.reset(); // TODO - can we get rid of Auth's dependency on current Order and User?
    } else {
      this.router.navigate(['/login']);
    }
  }

  async validateCurrentPasswordAndChangePassword(newPassword: string, currentPassword: string): Promise<void> {
    // reset password route does not require old password, so we are handling that here through a login
    await Auth.Login(this.currentUser.get().Username, currentPassword, this.appConfig.clientID, this.appConfig.scope);
    await Me.ResetPasswordByToken({ NewPassword: newPassword });
  }

  async resetPassword(code: string, config: PasswordReset): Promise<void> {
    await ForgottenPassword.ResetPasswordByVerificationCode(code, config);
  }

  setRememberMeStatus(status: boolean): void {
    this.cookieService.putObject(this.rememberMeCookieName, { status });
  }

  getRememberStatus(): boolean {
    const rememberMe = this.cookieService.getObject(this.rememberMeCookieName) as { status: string };
    return !!(rememberMe && rememberMe.status);
  }

  private async refreshTokenLogin(): Promise<AccessToken> {
    try {
      const refreshToken = Tokens.GetRefreshToken();
      const creds = await Auth.RefreshToken(refreshToken, this.appConfig.clientID);
      this.setToken(creds.access_token);
      return creds;
    } catch (err) {
      if (this.appConfig.anonymousShoppingEnabled) {
        return this.anonymousLogin();
      } else {
        throw new Error(err);
      }
    }
  }
}
