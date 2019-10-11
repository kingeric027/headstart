import { Injectable, Inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { OcTokenService } from '@ordercloud/angular-sdk';
import { applicationConfiguration } from 'src/app/config/app.config';
import { CurrentUserService } from 'src/app/shared/services/current-user/current-user.service';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { DOCUMENT } from '@angular/common';
import { AppConfig } from 'shopper-context-interface';

@Injectable({
  providedIn: 'root',
})
export class HasTokenGuard implements CanActivate {
  constructor(
    private ocTokenService: OcTokenService,
    private router: Router,
    private appAuthService: AuthService,
    private currentUser: CurrentUserService,
    @Inject(DOCUMENT) private document: any,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {}
  async canActivate(): Promise<boolean> {
    /**
     * very simple test to make sure a token exists,
     * can be parsed and has a valid expiration time.
     *
     * Shouldn't be depended on for actual token validation.
     * The API will block invalid tokens
     * and the client-side refresh-token interceptor will handle it correctly
     */

    // check for impersonation superseeds existing tokens to allow impersonating buyers sequentially.

    const isImpersonating = this.document.location.pathname === '/impersonation';
    if (isImpersonating) {
      const match = /token=([^&]*)/.exec(this.document.location.search);
      if (match) {
        this.appAuthService.setToken(match[1]);
        return true;
      } else {
        alert(`Missing url query param 'token'`);
      }
    }

    const isAccessTokenValid = this.isTokenValid();
    const refreshTokenExists = this.ocTokenService.GetRefresh() && this.appAuthService.getRememberStatus();
    if (!isAccessTokenValid && refreshTokenExists) {
      await this.appAuthService.refresh().toPromise();
      return true;
    }

    // send profiled users to login to get new token
    if (!isAccessTokenValid && !this.appConfig.anonymousShoppingEnabled) {
      this.router.navigate(['/login']);
      return false;
    }
    // get new anonymous token and then let them continue
    if (!isAccessTokenValid && this.appConfig.anonymousShoppingEnabled) {
      await this.appAuthService.anonymousLogin();
      return true;
    }
    this.currentUser.isLoggedIn = true;
    return isAccessTokenValid;
  }

  private isTokenValid(): boolean {
    const token = this.appAuthService.getOCToken();

    if (!token) {
      return false;
    }

    const decodedToken = this.appAuthService.getDecodedOCToken();

    if (!decodedToken) {
      return false;
    }

    const expiresIn = decodedToken.exp * 1000;
    return Date.now() < expiresIn;
  }
}
