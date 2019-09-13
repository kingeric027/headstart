import { Injectable, Inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { OcTokenService } from '@ordercloud/angular-sdk';
import * as jwtDecode from 'jwt-decode';
import { DecodedOrderCloudToken } from '@app-buyer/shared';
import { applicationConfiguration, AppConfig } from '@app-buyer/config/app.config';
import { of, Observable } from 'rxjs';
import { flatMap, map } from 'rxjs/operators';
import { CurrentUserService } from '@app-buyer/shared/services/current-user/current-user.service';
import { AuthService } from '@app-buyer/shared/services/auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class HasTokenGuard implements CanActivate {
  constructor(
    private ocTokenService: OcTokenService,
    private router: Router,
    private appAuthService: AuthService,
    private currentUser: CurrentUserService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}
  canActivate(): Observable<boolean> {
    /**
     * very simple test to make sure a token exists,
     * can be parsed and has a valid expiration time.
     *
     * Shouldn't be depended on for actual token validation.
     * The API will block invalid tokens
     * and the client-side refresh-token interceptor will handle it correctly
     */

    // check for impersonation superseeds existing tokens to allow impersonating buyers sequentially.
    if (window.location.pathname === '/impersonation') {
      const match = /token=([^&]*)/.exec(window.location.search);
      if (match) {
        this.ocTokenService.SetAccess(match[1]);
        this.currentUser.isLoggedIn = true;
        return of(true);
      } else {
        alert(`Missing url query param 'token'`);
      }
    }

    const isAccessTokenValid = this.isTokenValid();
    const refreshTokenExists = this.ocTokenService.GetRefresh() && this.appAuthService.getRememberStatus();
    if (!isAccessTokenValid && refreshTokenExists) {
      return this.appAuthService.refresh().pipe(map(() => true));
    }

    // send profiled users to login to get new token
    if (!isAccessTokenValid && !this.appConfig.anonymousShoppingEnabled) {
      this.router.navigate(['/login']);
      return of(false);
    }
    // get new anonymous token and then let them continue
    if (!isAccessTokenValid && this.appConfig.anonymousShoppingEnabled) {
      return this.appAuthService.authAnonymous().pipe(
        flatMap(() => {
          this.currentUser.isLoggedIn = true;
          return of(true);
        })
      );
    }
    this.currentUser.isLoggedIn = true;
    return of(isAccessTokenValid);
  }

  private isTokenValid(): boolean {
    const token = this.ocTokenService.GetAccess();

    if (!token) {
      return false;
    }

    let decodedToken: DecodedOrderCloudToken;
    try {
      decodedToken = jwtDecode(token);
    } catch (e) {
      decodedToken = null;
    }
    if (!decodedToken) {
      return false;
    }

    const expiresIn = decodedToken.exp * 1000;
    return Date.now() < expiresIn;
  }
}
