import { Injectable, Inject, PLATFORM_ID, Injector } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { OcTokenService } from '@ordercloud/angular-sdk';
import * as jwtDecode from 'jwt-decode';
import { DecodedOrderCloudToken } from 'src/app/shared';
import { applicationConfiguration, AppConfig } from 'src/app/config/app.config';
import { of, Observable } from 'rxjs';
import { flatMap, map } from 'rxjs/operators';
import { CurrentUserService } from 'src/app/shared/services/current-user/current-user.service';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { isPlatformServer } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class HasTokenGuard implements CanActivate {
  constructor(
    private ocTokenService: OcTokenService,
    private router: Router,
    private appAuthService: AuthService,
    private currentUser: CurrentUserService,
    private injector: Injector,
    @Inject(PLATFORM_ID) private platformId: Object,
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
    let isImpersonating: boolean;
    if (isPlatformServer(this.platformId)) {
      const req = this.injector.get('request');
      isImpersonating = req.path === '/impersonation';
    } else {
      isImpersonating = window.location.pathname === '/impersonation';
    }
    if (isImpersonating) {
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
