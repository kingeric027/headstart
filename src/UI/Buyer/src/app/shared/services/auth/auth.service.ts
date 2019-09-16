import { Injectable, Inject } from '@angular/core';
import { Observable, throwError, of, BehaviorSubject } from 'rxjs';
import { tap, catchError, finalize, map } from 'rxjs/operators';
import { Router } from '@angular/router';

// 3rd party
import { OcTokenService, OcAuthService, AccessToken } from '@ordercloud/angular-sdk';
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';
import { CookieService } from 'ngx-cookie';
import { AppErrorHandler } from '@app-buyer/config/error-handling.config';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';
import { CurrentOrderService } from '@app-buyer/shared/services/current-order/current-order.service';
import { IAuthActions } from '@app-buyer/ocm-default-components/shopper-context';

export const TokenRefreshAttemptNotPossible = 'Token refresh attempt not possible';
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
    private appErrorHandler: AppErrorHandler,
    private currentUser: CurrentUserService,
    private currentOrder: CurrentOrderService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    this.refreshToken = new BehaviorSubject<string>('');
  }

  refresh(): Observable<void> {
    this.fetchingRefreshToken = true;
    return this.fetchRefreshToken().pipe(
      tap((token) => {
        this.currentUser.isLoggedIn = true;
        this.ocTokenService.SetAccess(token);
        this.refreshToken.next(token);
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

  fetchToken(): Observable<string> {
    const accessToken = this.ocTokenService.GetAccess();
    if (accessToken) {
      return of(accessToken);
    }
    return this.fetchRefreshToken();
  }

  fetchRefreshToken(): Observable<string> {
    const refreshToken = this.ocTokenService.GetRefresh();
    if (refreshToken) {
      return this.ocAuthService.RefreshToken(refreshToken, this.appConfig.clientID).pipe(
        map((authResponse) => authResponse.access_token),
        tap((token) => this.ocTokenService.SetAccess(token)),
        catchError((error) => {
          if (this.appConfig.anonymousShoppingEnabled) {
            return this.authAnonymous();
          } else {
            throwError(error);
          }
        })
      );
    }

    if (this.appConfig.anonymousShoppingEnabled) {
      return this.authAnonymous();
    }

    return throwError(TokenRefreshAttemptNotPossible);
  }

  async login(userName: string, password: string): Promise<AccessToken> {
    const creds = await this.ocAuthService.Login(userName, password, this.appConfig.clientID, this.appConfig.scope).toPromise();
    if (!creds) return;
    this.currentUser.isLoggedIn = true;
    this.ocTokenService.SetAccess(creds.access_token);
    return creds;
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

  authAnonymous(): Observable<string> {
    return this.ocAuthService.Anonymous(this.appConfig.clientID, this.appConfig.scope).pipe(
      map((authResponse) => authResponse.access_token),
      tap((token) => this.ocTokenService.SetAccess(token)),
      catchError((ex) => {
        this.appErrorHandler.displayError(ex);
        this.logout();
        return of(null);
      })
    );
  }

  setRememberStatus(status: boolean): void {
    this.cookieService.putObject(this.rememberMeCookieName, { status: status });
  }

  getRememberStatus(): boolean {
    const rememberMe = <{ status: string }>this.cookieService.getObject(this.rememberMeCookieName);
    return !!(rememberMe && rememberMe.status);
  }
}
