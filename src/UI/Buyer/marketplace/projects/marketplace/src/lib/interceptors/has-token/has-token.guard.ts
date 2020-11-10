import { Injectable, Inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { DOCUMENT } from '@angular/common';
import { TokenHelperService } from '../../services/token-helper/token-helper.service';
import { Tokens } from 'ordercloud-javascript-sdk';
import { AuthService } from '../../services/auth/auth.service';
import { AppConfig } from '../../shopper-context';

@Injectable({
  providedIn: 'root',
})
export class HasTokenGuard implements CanActivate {
  constructor(
    private router: Router,
    private auth: AuthService,
    private tokenHelper: TokenHelperService,
    @Inject(DOCUMENT) private document: any,
    private appConfig: AppConfig
  ) {}

  /**
   * very simple test to make sure a token exists,
   * can be parsed and has a valid expiration time.
   *
   * Shouldn't be depended on for actual token validation.
   * The API will block invalid tokens
   * and the client-side refresh-token interceptor will handle it correctly
   */
  async canActivate(): Promise<boolean> {
    // check for impersonation superseeds existing tokens to allow impersonating buyers sequentially.
    if (this.isImpersonating()) {
      const token = this.getQueryParamToken();
      this.auth.loginWithTokens(token);
      return true;
    } else if (this.isSingleSignOn()) {
      const token = this.getQueryParamToken();
      this.auth.loginWithTokens(token, null, true);
      return true;
    }

    const isAccessTokenValid = this.isTokenValid();
    const refreshTokenExists = Tokens.GetRefreshToken() && this.auth.getRememberStatus();
    if (!isAccessTokenValid && refreshTokenExists) {
      await this.auth.refresh().toPromise();
      return true;
    }

    // send profiled users to login to get new token
    if (!isAccessTokenValid && !this.appConfig.anonymousShoppingEnabled) {
      this.router.navigate(['/login']);
      return false;
    }
    // get new anonymous token and then let them continue
    if (!isAccessTokenValid && this.appConfig.anonymousShoppingEnabled) {
      await this.auth.anonymousLogin();
      return true;
    }
    this.auth.isLoggedIn = true;
    return isAccessTokenValid;
  }

  private isImpersonating(): boolean {
    return this.document.location.pathname === '/impersonation';
  }

  private isSingleSignOn(): boolean {
    return this.document.location.pathname === '/sso';
  }

  private getQueryParamToken(): string {
    const match = /token=([^&]*)/.exec(this.document.location.search);
    if (!match) throw Error(`Missing url query param 'token'`);
    return match[1];
  }

  private isTokenValid(): boolean {
    const decodedToken = this.tokenHelper.getDecodedOCToken();

    if (!decodedToken) {
      return false;
    }

    const expiresIn = decodedToken.exp * 1000;
    return Date.now() < expiresIn;
  }
}
