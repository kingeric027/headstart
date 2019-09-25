import { Injectable, Inject } from '@angular/core';
import { Observable, of, BehaviorSubject, from } from 'rxjs';
import { tap, catchError, finalize } from 'rxjs/operators';
import { Router } from '@angular/router';

// 3rd party
import { OcTokenService, OcAuthService, AccessToken, OcMeService } from '@ordercloud/angular-sdk';
import { applicationConfiguration, AppConfig } from 'src/app/config/app.config';
import { CookieService } from '@gorniv/ngx-universal';
import { CurrentUserService } from 'src/app/shared/services/current-user/current-user.service';
import { CurrentOrderService } from 'src/app/shared/services/current-order/current-order.service';
import { IAuthActions } from 'src/app/ocm-default-components/shopper-context';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements IAuthActions {
  private rememberMeCookieName = `${this.appConfig.appname.replace(/ /g, '_').toLowerCase()}_rememberMe`;
  fetchingRefreshToken = false;
  failedRefreshAttempt = false;
  refreshToken: BehaviorSubject<string>;

  constructor(
    private ocTokenService: OcTokenService,
    private ocAuthService: OcAuthService,
    private cookieService: CookieService,
    private router: Router,
    private currentUser: CurrentUserService,
    private currentOrder: CurrentOrderService,
    private ocMeService: OcMeService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    this.refreshToken = new BehaviorSubject<string>('');
  }

  refresh(): Observable<void> {
    this.fetchingRefreshToken = true;
    return from(this.refreshTokenLogin()).pipe(
      tap((token) => {
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

  private async refreshTokenLogin(): Promise<AccessToken> {
    try {
      const refreshToken = this.ocTokenService.GetRefresh();
      const creds = await this.ocAuthService.RefreshToken(refreshToken, this.appConfig.clientID).toPromise();
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

  setToken(token: string) {
    if (!token) return;
    this.ocTokenService.SetAccess(token);
    this.currentUser.isLoggedIn = true;
  }

  async profiledLogin(userName: string, password: string, rememberMe: boolean = false): Promise<AccessToken> {
    const creds = await this.ocAuthService.Login(userName, password, this.appConfig.clientID, this.appConfig.scope).toPromise();
    this.setToken(creds.access_token);
    if (rememberMe && creds.refresh_token) {
      /**
       * set the token duration in the dashboard - https://developer.ordercloud.io/dashboard/settings
       * refresh tokens are configured per clientID and initially set to 0
       * a refresh token of 0 means no refresh token is returned in OAuth response
       */
      this.ocTokenService.SetRefresh(creds.refresh_token);
      this.setRememberMeStatus(true);
    }
    this.router.navigateByUrl('/home');
    return creds;
  }

  async anonymousLogin(): Promise<AccessToken> {
    try {
      const creds = await this.ocAuthService.Anonymous(this.appConfig.clientID, this.appConfig.scope).toPromise();
      this.setToken(creds.access_token);
      return creds;
    } catch (err) {
      this.logout();
      throw new Error(err);
    }
  }

  async logout(): Promise<void> {
    this.ocTokenService.RemoveAccess();
    this.currentUser.isLoggedIn = false;
    if (this.appConfig.anonymousShoppingEnabled) {
      this.router.navigate(['/home']);
      await this.currentUser.reset();
      this.currentOrder.reset();
    } else {
      this.router.navigate(['/login']);
    }
  }

  async changePassword(newPassword: string): Promise<void> {
    await this.ocMeService.ResetPasswordByToken({ NewPassword: newPassword }).toPromise();
  }

  getOrderCloudToken(): string {
    return this.ocTokenService.GetAccess();
  }

  setRememberMeStatus(status: boolean): void {
    this.cookieService.putObject(this.rememberMeCookieName, { status: status });
  }

  getRememberStatus(): boolean {
    const rememberMe = <{ status: string }>this.cookieService.getObject(this.rememberMeCookieName);
    return !!(rememberMe && rememberMe.status);
  }
}
